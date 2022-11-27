using Nop.Core;

namespace Nop.Plugin.Misc.GiftProvider.Domain;

public abstract partial class GiftMapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the gift identifier
    /// </summary>
    public int GiftId { get; set; }

    /// <summary>
    /// Gets or sets the entity identifier
    /// </summary>
    public abstract int EntityId { get; set; }
}
