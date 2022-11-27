namespace Nop.Plugin.Misc.GiftProvider.Domain;

/// <summary>
/// Represents an entity which supports discounts
/// </summary>
public partial interface IGiftSupported<T> where T : GiftMapping
{
    int Id { get; set; }
}