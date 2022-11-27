using Nop.Core.Caching;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Services;

/// <summary>
/// Represents default values related to discounts services
/// </summary>
public static partial class NopGiftDefaults
{
    /// <summary>
    /// Gets the query parameter name to retrieve discount coupon code from URL
    /// </summary>
    public static string GiftCouponQueryParameter => "giftcoupon";

    #region Caching defaults

    /// <summary>
    /// Key for discount requirement of a certain discount
    /// </summary>
    /// <remarks>
    /// {0} : discount id
    /// </remarks>
    public static CacheKey GiftRequirementsByGiftCacheKey => new("Nop.giftrequirement.bydiscount.{0}");

    /// <summary>
    /// Key for caching
    /// </summary>
    /// <remarks>
    /// {0} : show hidden records?
    /// {1} : coupon code
    /// {2} : discount name
    /// </remarks>
    public static CacheKey GiftAllCacheKey => new("Nop.gift.all.{0}-{1}-{2}", NopEntityCacheDefaults<Gift>.AllPrefix);

    /// <summary>
    /// Key for caching
    /// </summary>
    /// <remarks>
    /// {0} - entity type
    /// {1} - entity id
    /// </remarks>
    public static CacheKey AppliedGiftsCacheKey => new("Nop.gift.applied.{0}-{1}", AppliedGiftsCachePrefix);

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string AppliedGiftsCachePrefix => "Nop.gift.applied.";

    /// <summary>
    /// Key for category IDs of a discount
    /// </summary>
    /// <remarks>
    /// {0} : discount id
    /// {1} : roles of the current user
    /// {2} : current store ID
    /// </remarks>
    public static CacheKey CategoryIdsByGiftCacheKey => new("Nop.gift.categoryids.bydiscount.{0}-{1}-{2}", CategoryIdsByGiftPrefix, CategoryIdsPrefix);

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    /// <remarks>
    /// {0} : discount id
    /// </remarks>
    public static string CategoryIdsByGiftPrefix => "Nop.gift.categoryids.bydiscount.{0}";

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string CategoryIdsPrefix => "Nop.gift.categoryids.bydiscount.";

    /// <summary>
    /// Key for manufacturer IDs of a discount
    /// </summary>
    /// <remarks>
    /// {0} : discount id
    /// {1} : roles of the current user
    /// {2} : current store ID
    /// </remarks>
    public static CacheKey ManufacturerIdsByGiftCacheKey => new("Nop.gift.manufacturerids.bygift.{0}-{1}-{2}", ManufacturerIdsByGiftPrefix, ManufacturerIdsPrefix);

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    /// <remarks>
    /// {0} : discount id
    /// </remarks>
    public static string ManufacturerIdsByGiftPrefix => "Nop.gift.manufacturerids.bygift.{0}";

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string ManufacturerIdsPrefix => "Nop.gift.manufacturerids.bygift.";

    public static string GiftCouponCodeAttribute => "GiftCouponCode";
    #endregion
}