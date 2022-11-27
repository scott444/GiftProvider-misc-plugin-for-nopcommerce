using Nop.Services.Customers;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.GiftProvider.Services;

/// <summary>
/// Represents a discount requirement plugin manager implementation
/// </summary>
public partial class GiftPluginManager : PluginManager<IGiftRequirementRule>, IGiftPluginManager
{
    #region Ctor

    public GiftPluginManager(ICustomerService customerService,
        IPluginService pluginService) : base(customerService, pluginService)
    {
    }

    #endregion
}