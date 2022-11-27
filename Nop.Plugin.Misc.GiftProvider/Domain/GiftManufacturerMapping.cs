namespace Nop.Plugin.Misc.GiftProvider.Domain;

/// <summary>
/// Represents a discount-manufacturer mapping class
/// </summary>
public partial class GiftManufacturerMapping : GiftMapping
{
    /// <summary>
    /// Gets or sets the manufacturer identifier
    /// </summary>
    public override int EntityId { get; set; }
}