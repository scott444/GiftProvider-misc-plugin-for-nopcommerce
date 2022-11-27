namespace Nop.Plugin.Misc.GiftProvider.Domain;

/// <summary>
/// Represents a Gift-category mapping class
/// </summary>
public partial class GiftCategoryMapping : GiftMapping
{
    /// <summary>
    /// Gets or sets the category identifier
    /// </summary>
    public override int EntityId { get; set; }
}