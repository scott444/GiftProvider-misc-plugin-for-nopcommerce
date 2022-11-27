using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.GiftProvider.Services;

/// <summary>
/// Represents a discount requirement plugin manager
/// </summary>
public partial interface IGiftPluginManager : IPluginManager<IGiftRequirementRule>
{
}