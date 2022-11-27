namespace Nop.Plugin.Misc.GiftProvider.Domain;

/// <summary>
/// Represents a discount-product mapping class
/// </summary>
public partial class GiftProductMapping : GiftMapping
{
    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public override int EntityId { get; set; }
}