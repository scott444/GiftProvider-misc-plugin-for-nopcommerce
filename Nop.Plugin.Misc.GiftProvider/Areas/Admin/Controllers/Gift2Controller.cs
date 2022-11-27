using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models;
using Nop.Plugin.Misc.GiftProvider.Domain;
using Nop.Plugin.Misc.GiftProvider.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
[Area(AreaNames.Admin)]
public class Gift2Controller : BasePluginController
{
        #region Fields

        private readonly IAddressService _addressService;   
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;        
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        #endregion

        #region Ctor 

        public Gift2Controller(
          IAddressService addressService,
            ICustomerActivityService customerActivityService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            _addressService = addressService;
            _customerActivityService = customerActivityService;
            _exportManager = exportManager;
            _importManager = importManager;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
        }

        #endregion

        #region Configure

        public async Task<IActionResult> ConfigureAsync()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<GiftSettings>(storeScope);

        
            var model = new ConfigurationModel()
            {
                ActiveStoreScopeConfiguration = storeScope             
            };

            return View($"{Constants.PLUGIN_ADMIN_VIEW_PATH}/Shared/Configure.cshtml", model);
        }


        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();


            var settings = await _settingService.LoadSettingAsync<GiftSettings>(storeScope);
      

            // now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await ConfigureAsync();
        }
        #endregion
    }
