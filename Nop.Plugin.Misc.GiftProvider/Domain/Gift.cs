using System;
using Nop.Core;

namespace Nop.Plugin.Misc.GiftProvider.Domain;

/// <summary>
/// Represents a gift
/// </summary>
public partial class Gift : BaseEntity
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the admin comment
    /// </summary>
    public string AdminComment { get; set; }

    /// <summary>
    /// Gets or sets the discount type identifier
    /// </summary>
    public int GiftTypeId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use percentage
    /// </summary>
    public bool UsePercentage { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage
    /// </summary>
    public decimal GiftPercentage { get; set; }

    /// <summary>
    /// Gets or sets the discount amount
    /// </summary>
    public decimal GiftAmount { get; set; }

    /// <summary>
    /// Gets or sets the maximum discount amount (used with "UsePercentage")
    /// </summary>
    public decimal? MaximumGiftAmount { get; set; }

    /// <summary>
    /// Gets or sets the discount start date and time
    /// </summary>
    public DateTime? StartDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the discount end date and time
    /// </summary>
    public DateTime? EndDateUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether discount requires coupon code
    /// </summary>
    public bool RequiresCouponCode { get; set; }

    /// <summary>
    /// Gets or sets the coupon code
    /// </summary>
    public string CouponCode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether discount can be used simultaneously with other discounts (with the same discount type)
    /// </summary>
    public bool IsCumulative { get; set; }

    /// <summary>
    /// Gets or sets the discount limitation identifier
    /// </summary>
    public int GiftLimitationId { get; set; }

    /// <summary>
    /// Gets or sets the discount limitation times (used when Limitation is set to "N Times Only" or "N Times Per Customer")
    /// </summary>
    public int LimitationTimes { get; set; }

    /// <summary>
    /// Gets or sets the maximum product quantity which could be discounted
    /// Used with "Assigned to products" or "Assigned to categories" type
    /// </summary>
    public int? MaximumGiftedQuantity { get; set; }

    /// <summary>
    /// Gets or sets value indicating whether it should be applied to all subcategories or the selected one
    /// Used with "Assigned to categories" type only.
    /// </summary>
    public bool AppliedToSubCategories { get; set; }

    /// <summary>
    /// Gets or sets the discount type
    /// </summary>
    public GiftType GiftType
    {
        get => (GiftType)GiftTypeId;
        set => GiftTypeId = (int)value;
    }

    /// <summary>
    /// Gets or sets the discount limitation
    /// </summary>
    public GiftLimitationType GiftLimitation
    {
        get => (GiftLimitationType)GiftLimitationId;
        set => GiftLimitationId = (int)value;
    }
}
