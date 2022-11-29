using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;
using System;
using Nop.Plugin.Misc.GiftProvider.Services;
using Nop.Services.Installation;
using Nop.Services.Localization;
using System.IO;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Misc.GiftProvider;

public class GiftProvider : BasePlugin, IMiscPlugin, IAdminMenuPlugin
{
    private readonly ILanguageService _languageService;
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly INopFileProvider _fileProvider;

    public GiftProvider(ILanguageService languageService, ILocalizationService localizationService, IPermissionService permissionService, INopFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
        _languageService = languageService;
        _localizationService = localizationService;
        _permissionService = permissionService;
    }


    #region Admin Menu
    public Task ManageSiteMapAsync(SiteMapNode rootNode)
    {
        var promotionsNode = rootNode.ChildNodes.First(x => x.SystemName == "Promotions");

        var giftsMenuItem = new SiteMapNode()
        {
            SystemName = "Gifts",
            Title = "Gifts",
            ControllerName = "Gift",
            ActionName = "List",
            Visible = true,
            IconClass = "far fa-dot-circle",
            RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
        };

        promotionsNode.ChildNodes.Add(giftsMenuItem);

        return Task.CompletedTask;
    }
    #endregion

    public async override Task InstallAsync()
    {
        await InstallPermissionsAsync();
        await InstallLocaleResourcesAsync();

        await base.InstallAsync();
    }

    public async override Task UninstallAsync()
    {
        await UninstallPermissionsAsync();
        await UninstallLocaleResourcesAsync();

        await base.UninstallAsync();
    }

    #region Permissions
    /// <summary>
    /// Install permissions
    /// </summary>
    /// <param name="permissionProvider">Permission provider</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InstallPermissionsAsync()
    {
        //register gift permissions
        var permissionProviders = new List<Type> { typeof(GiftPermissionProvider) };
        foreach (var providerType in permissionProviders)
        {
            var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
            await _permissionService.InstallPermissionsAsync(provider);
        }
    }

    /// <summary>
    /// Uninstall permissions
    /// </summary>
    /// <param name="permissionProvider">Permission provider</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UninstallPermissionsAsync()
    {
        //register default permissions
        var permissionProviders = new List<Type> { typeof(GiftPermissionProvider) };
        foreach (var providerType in permissionProviders)
        {
            var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
            await _permissionService.UninstallPermissionsAsync(provider);
        }
    }

    #endregion

    public async Task InstallLocaleResourcesAsync()
    {
        var directoryPath = _fileProvider.MapPath("/Plugins/Misc.GiftProvider/Localization/Installation");
        var pattern = $"*.{NopInstallationDefaults.LocalizationResourcesFileExtension}";
        var languages = await _languageService.GetAllLanguagesAsync();
        var defaultLanguage = languages.FirstOrDefault(l => l.Name == "EN");
        if (defaultLanguage != null)
        {
            foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, pattern))
            {
                using var streamReader = new StreamReader(filePath);
                await _localizationService.ImportResourcesFromXmlAsync(defaultLanguage, streamReader);
            }
        }
    }
    public async Task UninstallLocaleResourcesAsync()
    {
        var languages = await _languageService.GetAllLanguagesAsync();
        var defaultLanguage = languages.FirstOrDefault(l => l.Name == "EN");
        await _localizationService.DeleteLocaleResourcesAsync("Admin.Promotions.Gifts", defaultLanguage.Id);
    }
}


