using System;
using Nop.Core;

namespace Nop.Plugin.Misc.GiftProvider.Domain;

/// <summary>
/// Represents a discount usage history entry
/// </summary>
public partial class GiftUsageHistory : BaseEntity
{
    /// <summary>
    /// Gets or sets the discount identifier
    /// </summary>
    public int GiftId { get; set; }

    /// <summary>
    /// Gets or sets the order identifier
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}
