
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Misc.GiftProvider.Domain;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;

namespace Nop.Plugin.Misc.GiftProvider.Services;

/// <summary>
/// Gift service
/// </summary>
public partial class GiftService : IGiftService
{
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IGiftPluginManager _giftPluginManager;
    private readonly ILocalizationService _localizationService;
    private readonly IProductService _productService;
    private readonly IRepository<Gift> _giftRepository;
    private readonly IRepository<GiftRequirement> _giftRequirementRepository;
    private readonly IRepository<GiftUsageHistory> _giftUsageHistoryRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public GiftService(ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IGiftPluginManager giftPluginManager,
        ILocalizationService localizationService,
        IProductService productService,
        IRepository<Gift> giftRepository,
        IRepository<GiftRequirement> giftRequirementRepository,
        IRepository<GiftUsageHistory> giftUsageHistoryRepository,
        IRepository<Order> orderRepository,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext)
    {
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _giftPluginManager = giftPluginManager;
        _localizationService = localizationService;
        _productService = productService;
        _giftRepository = giftRepository;
        _giftRequirementRepository = giftRequirementRepository;
        _giftUsageHistoryRepository = giftUsageHistoryRepository;
        _orderRepository = orderRepository;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get gift validation result
    /// </summary>
    /// <param name="requirements">Collection of gift requirement</param>
    /// <param name="groupInteractionType">Interaction type within the group of requirements</param>
    /// <param name="customer">Customer</param>
    /// <param name="errors">Errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the rue if result is valid; otherwise false
    /// </returns>
    protected async Task<bool> GetValidationResultAsync(IEnumerable<GiftRequirement> requirements,
        RequirementGroupInteractionType groupInteractionType, Customer customer, List<string> errors)
    {
        var result = false;

        foreach (var requirement in requirements)
        {
            if (requirement.IsGroup)
            {
                var childRequirements = await GetGiftRequirementsByParentAsync(requirement);
                //get child requirements for the group
                var interactionType = requirement.InteractionType ?? RequirementGroupInteractionType.And;
                result = await GetValidationResultAsync(childRequirements, interactionType, customer, errors);
            }
            else
            {
                //or try to get validation result for the requirement
                var store = await _storeContext.GetCurrentStoreAsync();
                var requirementRulePlugin = await _giftPluginManager
                    .LoadPluginBySystemNameAsync(requirement.GiftRequirementRuleSystemName, customer, store.Id);
                if (requirementRulePlugin == null)
                    continue;

                var ruleResult = await requirementRulePlugin.CheckRequirementAsync(new GiftRequirementValidationRequest
                {
                    GiftRequirementId = requirement.Id,
                    Customer = customer,
                    Store = store
                });

                //add validation error
                if (!ruleResult.IsValid)
                {
                    var userError = !string.IsNullOrEmpty(ruleResult.UserError)
                        ? ruleResult.UserError
                        : await _localizationService.GetResourceAsync("ShoppingCart.Gift.CannotBeUsed");
                    errors.Add(userError);
                }

                result = ruleResult.IsValid;
            }

            //all requirements must be met, so return false
            if (!result && groupInteractionType == RequirementGroupInteractionType.And)
                return false;

            //any of requirements must be met, so return true
            if (result && groupInteractionType == RequirementGroupInteractionType.Or)
                return true;
        }

        return result;
    }

    #endregion

    #region Methods

    #region Gifts

    /// <summary>
    /// Delete gift
    /// </summary>
    /// <param name="gift">Gift</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteGiftAsync(Gift gift)
    {
        //first, delete related gift requirements
        await _giftRequirementRepository.DeleteAsync(await GetAllGiftRequirementsAsync(gift.Id));

        //then delete the gift
        await _giftRepository.DeleteAsync(gift);
    }

    /// <summary>
    /// Gets a gift
    /// </summary>
    /// <param name="giftId">Gift identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift
    /// </returns>
    public virtual async Task<Gift> GetGiftByIdAsync(int giftId)
    {
        return await _giftRepository.GetByIdAsync(giftId, cache => default);
    }

    /// <summary>
    /// Gets all gifts
    /// </summary>
    /// <param name="giftType">Gift type; pass null to load all records</param>
    /// <param name="couponCode">Coupon code to find (exact match); pass null or empty to load all records</param>
    /// <param name="giftName">Gift name; pass null or empty to load all records</param>
    /// <param name="showHidden">A value indicating whether to show expired and not started gifts</param>
    /// <param name="startDateUtc">Gift start date; pass null to load all records</param>
    /// <param name="endDateUtc">Gift end date; pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gifts
    /// </returns>
    public virtual async Task<IList<Gift>> GetAllGiftsAsync(GiftType? giftType = null,
        string couponCode = null, string giftName = null, bool showHidden = false,
        DateTime? startDateUtc = null, DateTime? endDateUtc = null)
    {
        //we load all gifts, and filter them using "giftType" parameter later (in memory)
        //we do it because we know that this method is invoked several times per HTTP request with distinct "giftType" parameter
        //that's why let's access the database only once
        var gifts = (await _giftRepository.GetAllAsync(query =>
        {
            if (!showHidden)
                query = query.Where(gift =>
                    (!gift.StartDateUtc.HasValue || gift.StartDateUtc <= DateTime.UtcNow) &&
                    (!gift.EndDateUtc.HasValue || gift.EndDateUtc >= DateTime.UtcNow));

            //filter by coupon code
            if (!string.IsNullOrEmpty(couponCode))
                query = query.Where(gift => gift.CouponCode == couponCode);

            //filter by name
            if (!string.IsNullOrEmpty(giftName))
                query = query.Where(gift => gift.Name.Contains(giftName));

            query = query.OrderBy(gift => gift.Name).ThenBy(gift => gift.Id);

            return query;
        }, cache => cache.PrepareKeyForDefaultCache(NopGiftDefaults.GiftAllCacheKey, 
            showHidden, couponCode ?? string.Empty, giftName ?? string.Empty)))
        .AsQueryable();

        //we know that this method is usually invoked multiple times
        //that's why we filter gifts by type and dates on the application layer
        if (giftType.HasValue)
            gifts = gifts.Where(gift => gift.GiftType == giftType.Value);

        //filter by dates
        if (startDateUtc.HasValue)
            gifts = gifts.Where(gift =>
                !gift.StartDateUtc.HasValue || gift.StartDateUtc >= startDateUtc.Value);
        if (endDateUtc.HasValue)
            gifts = gifts.Where(gift =>
                !gift.EndDateUtc.HasValue || gift.EndDateUtc <= endDateUtc.Value);

        return gifts.ToList();
    }

    /// <summary>
    /// Gets gifts applied to entity
    /// </summary>
    /// <typeparam name="T">Type based on <see cref="GiftMapping" /></typeparam>
    /// <param name="entity">Entity which supports gifts (<see cref="IGiftSupported{T}" />)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of gifts
    /// </returns>
    public virtual async Task<IList<Gift>> GetAppliedGiftsAsync<T>(IGiftSupported<T> entity) where T : GiftMapping
    {
        var giftMappingRepository = EngineContext.Current.Resolve<IRepository<T>>();

        var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(NopGiftDefaults.AppliedGiftsCacheKey, entity.GetType().Name, entity);

        var appliedGifts= await _staticCacheManager.GetAsync(cacheKey,
            async () =>
            {
                return await (from d in _giftRepository.Table
                    join ad in giftMappingRepository.Table on d.Id equals ad.GiftId
                    where ad.EntityId == entity.Id
                    select d).ToListAsync();
            });

        return appliedGifts;
    }

    /// <summary>
    /// Inserts a gift
    /// </summary>
    /// <param name="gift">Gift</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertGiftAsync(Gift gift)
    {
        await _giftRepository.InsertAsync(gift);
    }

    /// <summary>
    /// Updates the gift
    /// </summary>
    /// <param name="gift">Gift</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateGiftAsync(Gift gift)
    {
        await _giftRepository.UpdateAsync(gift);
    }

    #endregion

    #region Gifts (caching)
    
    /// <summary>
    /// Gets the gift amount for the specified value
    /// </summary>
    /// <param name="gift">Gift</param>
    /// <param name="amount">Amount</param>
    /// <returns>The gift amount</returns>
    public virtual decimal GetGiftAmount(Gift gift, decimal amount)
    {
        if (gift == null)
            throw new ArgumentNullException(nameof(gift));

        //calculate gift amount
        decimal result;
        if (gift.UsePercentage)
            result = (decimal)((float)amount * (float)gift.GiftPercentage / 100f);
        else
            result = gift.GiftAmount;

        //validate maximum gift amount
        if (gift.UsePercentage &&
            gift.MaximumGiftAmount.HasValue &&
            result > gift.MaximumGiftAmount.Value)
            result = gift.MaximumGiftAmount.Value;

        if (result < decimal.Zero)
            result = decimal.Zero;

        return result;
    }

    /// <summary>
    /// Get preferred gift (with maximum gift value)
    /// </summary>
    /// <param name="gifts">A list of gifts to check</param>
    /// <param name="amount">Amount (initial value)</param>
    /// <param name="giftAmount">Gift amount</param>
    /// <returns>Preferred gift</returns>
    public virtual List<Gift> GetPreferredGift(IList<Gift> gifts,
        decimal amount, out decimal giftAmount)
    {
        if (gifts == null)
            throw new ArgumentNullException(nameof(gifts));

        var result = new List<Gift>();
        giftAmount = decimal.Zero;
        if (!gifts.Any())
            return result;

        //first we check simple gifts
        foreach (var gift in gifts)
        {
            var currentGiftValue = GetGiftAmount(gift, amount);
            if (currentGiftValue <= giftAmount)
                continue;

            giftAmount = currentGiftValue;

            result.Clear();
            result.Add(gift);
        }
        //now let's check cumulative gifts
        //right now we calculate gift values based on the original amount value
        //please keep it in mind if you're going to use gifts with "percentage"
        var cumulativeGifts = gifts.Where(x => x.IsCumulative).OrderBy(x => x.Name).ToList();
        if (cumulativeGifts.Count <= 1)
            return result;

        var cumulativeGiftAmount = cumulativeGifts.Sum(d => GetGiftAmount(d, amount));
        if (cumulativeGiftAmount <= giftAmount)
            return result;

        giftAmount = cumulativeGiftAmount;

        result.Clear();
        result.AddRange(cumulativeGifts);

        return result;
    }

    /// <summary>
    /// Check whether a list of gifts already contains a certain gift instance
    /// </summary>
    /// <param name="gifts">A list of gifts</param>
    /// <param name="gift">Gift to check</param>
    /// <returns>Result</returns>
    public virtual bool ContainsGift(IList<Gift> gifts, Gift gift)
    {
        if (gifts == null)
            throw new ArgumentNullException(nameof(gifts));

        if (gift == null)
            throw new ArgumentNullException(nameof(gift));

        return gifts.Any(dis1 => gift.Id == dis1.Id);
    }

    #endregion

    #region Gift requirements

    /// <summary>
    /// Get all gift requirements
    /// </summary>
    /// <param name="giftId">Gift identifier</param>
    /// <param name="topLevelOnly">Whether to load top-level requirements only (without parent identifier)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the requirements
    /// </returns>
    public virtual async Task<IList<GiftRequirement>> GetAllGiftRequirementsAsync(int giftId = 0, bool topLevelOnly = false)
    {
        return await _giftRequirementRepository.GetAllAsync(query =>
        {
            //filter by gift
            if (giftId > 0)
                query = query.Where(requirement => requirement.GiftId == giftId);

            //filter by top-level
            if (topLevelOnly)
                query = query.Where(requirement => !requirement.ParentId.HasValue);

            query = query.OrderBy(requirement => requirement.Id);

            return query;
        });
    }

    /// <summary>
    /// Get a gift requirement
    /// </summary>
    /// <param name="giftRequirementId">Gift requirement identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<GiftRequirement> GetGiftRequirementByIdAsync(int giftRequirementId)
    {
        return await _giftRequirementRepository.GetByIdAsync(giftRequirementId, cache => default);
    }

    /// <summary>
    /// Gets child gift requirements
    /// </summary>
    /// <param name="giftRequirement">Parent gift requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<IList<GiftRequirement>> GetGiftRequirementsByParentAsync(GiftRequirement giftRequirement)
    {
        if (giftRequirement is null)
            throw new ArgumentNullException(nameof(giftRequirement));

        return await _giftRequirementRepository.Table
            .Where(dr => dr.ParentId == giftRequirement.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Delete gift requirement
    /// </summary>
    /// <param name="giftRequirement">Gift requirement</param>
    /// <param name="recursive">A value indicating whether to recursively delete child requirements</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteGiftRequirementAsync(GiftRequirement giftRequirement, bool recursive = false)
    {
        if (giftRequirement == null)
            throw new ArgumentNullException(nameof(giftRequirement));

        if (recursive && await GetGiftRequirementsByParentAsync(giftRequirement) is IList<GiftRequirement> children && children.Any())
            foreach (var child in children)
                await DeleteGiftRequirementAsync(child, true);

        await _giftRequirementRepository.DeleteAsync(giftRequirement);
    }

    /// <summary>
    /// Inserts a gift requirement
    /// </summary>
    /// <param name="giftRequirement">Gift requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertGiftRequirementAsync(GiftRequirement giftRequirement)
    {
        await _giftRequirementRepository.InsertAsync(giftRequirement);
    }

    /// <summary>
    /// Updates a gift requirement
    /// </summary>
    /// <param name="giftRequirement">Gift requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateGiftRequirementAsync(GiftRequirement giftRequirement)
    {
        await _giftRequirementRepository.UpdateAsync(giftRequirement);
    }

    #endregion

    #region Validation
    
    /// <summary>
    /// Validate gift
    /// </summary>
    /// <param name="gift">Gift</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift validation result
    /// </returns>
    public virtual async Task<GiftValidationResult> ValidateGiftAsync(Gift gift, Customer customer)
    {
        if (gift == null)
            throw new ArgumentNullException(nameof(gift));

        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        var couponCodesToValidate = await ParseAppliedGiftCouponCodesAsync(customer);
        
        return await ValidateGiftAsync(gift, customer, couponCodesToValidate);
    }

    /// <summary>
    /// Gets coupon codes
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the coupon codes
    /// </returns>
    public virtual async Task<string[]> ParseAppliedGiftCouponCodesAsync(Customer customer)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(customer, NopGiftDefaults.GiftCouponCodeAttribute);

        var couponCodes = new List<string>();
        if (string.IsNullOrEmpty(existingCouponCodes))
            return couponCodes.ToArray();

        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(existingCouponCodes);

            var nodeList1 = xmlDoc.SelectNodes(@"//GiftCouponCodes/CouponCode");
            foreach (XmlNode node1 in nodeList1)
            {
                if (node1.Attributes?["Code"] == null)
                    continue;
                var code = node1.Attributes["Code"].InnerText.Trim();
                couponCodes.Add(code);
            }
        }
        catch
        {
            // ignored
        }

        return couponCodes.ToArray();
    }

    /// <summary>
    /// Validate gift
    /// </summary>
    /// <param name="gift">Gift</param>
    /// <param name="customer">Customer</param>
    /// <param name="couponCodesToValidate">Coupon codes to validate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift validation result
    /// </returns>
    public virtual async Task<GiftValidationResult> ValidateGiftAsync(Gift gift, Customer customer, string[] couponCodesToValidate)
    {
        if (gift == null)
            throw new ArgumentNullException(nameof(gift));

        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        //invalid by default
        var result = new GiftValidationResult();

        //check coupon code
        if (gift.RequiresCouponCode)
        {
            if (string.IsNullOrEmpty(gift.CouponCode))
                return result;

            if (couponCodesToValidate == null)
                return result;

            if (!couponCodesToValidate.Any(x => x.Equals(gift.CouponCode, StringComparison.InvariantCultureIgnoreCase)))
                return result;
        }

        //Do not allow gifts applied to order subtotal or total when a customer has gift cards in the cart.
        //Otherwise, this customer can purchase gift cards with gift and get more than paid ("free money").
        if (gift.GiftType == GiftType.AssignedToOrderSubTotal ||
            gift.GiftType == GiftType.AssignedToOrderTotal)
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            //do not inject IShoppingCartService via constructor because it'll cause circular references
            var shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
            var cart = await shoppingCartService.GetShoppingCartAsync(customer,
                ShoppingCartType.ShoppingCart, storeId: store.Id);

            var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();
            
            if (await _productService.HasAnyGiftCardProductAsync(cartProductIds))
            {
                result.Errors = new List<string> {await _localizationService.GetResourceAsync("ShoppingCart.Gift.CannotBeUsedWithGiftCards") };
                return result;
            }
        }

        //check date range
        var now = DateTime.UtcNow;
        if (gift.StartDateUtc.HasValue)
        {
            var startDate = DateTime.SpecifyKind(gift.StartDateUtc.Value, DateTimeKind.Utc);
            if (startDate.CompareTo(now) > 0)
            {
                result.Errors = new List<string> {await _localizationService.GetResourceAsync("ShoppingCart.Gift.NotStartedYet") };
                return result;
            }
        }

        if (gift.EndDateUtc.HasValue)
        {
            var endDate = DateTime.SpecifyKind(gift.EndDateUtc.Value, DateTimeKind.Utc);
            if (endDate.CompareTo(now) < 0)
            {
                result.Errors = new List<string> {await _localizationService.GetResourceAsync("ShoppingCart.Gift.Expired") };
                return result;
            }
        }

        //gift limitation
        switch (gift.GiftLimitation)
        {
            case GiftLimitationType.NTimesOnly:
                {
                    var usedTimes = (await GetAllGiftUsageHistoryAsync(gift.Id, null, null, 0, 1)).TotalCount;
                    if (usedTimes >= gift.LimitationTimes)
                        return result;
                }

                break;
            case GiftLimitationType.NTimesPerCustomer:
                {
                    if (await _customerService.IsRegisteredAsync(customer))
                    {
                        var usedTimes = (await GetAllGiftUsageHistoryAsync(gift.Id, customer.Id, null, 0, 1)).TotalCount;
                        if (usedTimes >= gift.LimitationTimes)
                        {
                            result.Errors = new List<string> {await _localizationService.GetResourceAsync("ShoppingCart.Gift.CannotBeUsedAnymore") };
                            
                            return result;
                        }
                    }
                }

                break;
            case GiftLimitationType.Unlimited:
            default:
                break;
        }

        //gift requirements
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopGiftDefaults.GiftRequirementsByGiftCacheKey, gift);

        var requirements = await _staticCacheManager.GetAsync(key, async () => await GetAllGiftRequirementsAsync(gift.Id, true));

        //get top-level group
        var topLevelGroup = requirements.FirstOrDefault();
        if (topLevelGroup == null || (topLevelGroup.IsGroup && !(await GetGiftRequirementsByParentAsync(topLevelGroup)).Any()) || !topLevelGroup.InteractionType.HasValue)
        {
            //there are no requirements, so gift is valid
            result.IsValid = true;

            return result;
        }

        //requirements exist, let's check them
        var errors = new List<string>();

        result.IsValid = await GetValidationResultAsync(requirements, topLevelGroup.InteractionType.Value, customer, errors);

        //set errors if result is not valid
        if (!result.IsValid)
            result.Errors = errors;

        return result;
    }

    #endregion

    #region Gift usage history

    /// <summary>
    /// Gets a gift usage history record
    /// </summary>
    /// <param name="giftUsageHistoryId">Gift usage history record identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift usage history
    /// </returns>
    public virtual async Task<GiftUsageHistory> GetGiftUsageHistoryByIdAsync(int giftUsageHistoryId)
    {
        return await _giftUsageHistoryRepository.GetByIdAsync(giftUsageHistoryId);
    }

    /// <summary>
    /// Gets all gift usage history records
    /// </summary>
    /// <param name="giftId">Gift identifier; null to load all records</param>
    /// <param name="customerId">Customer identifier; null to load all records</param>
    /// <param name="orderId">Order identifier; null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift usage history records
    /// </returns>
    public virtual async Task<IPagedList<GiftUsageHistory>> GetAllGiftUsageHistoryAsync(int? giftId = null,
        int? customerId = null, int? orderId = null, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        return await _giftUsageHistoryRepository.GetAllPagedAsync(query =>
        {
            //filter by gift
            if (giftId.HasValue && giftId.Value > 0)
                query = query.Where(historyRecord => historyRecord.GiftId == giftId.Value);

            //filter by customer
            if (customerId.HasValue && customerId.Value > 0)
                query = from duh in query
                    join order in _orderRepository.Table on duh.OrderId equals order.Id
                    where order.CustomerId == customerId
                    select duh;

            //filter by order
            if (orderId.HasValue && orderId.Value > 0)
                query = query.Where(historyRecord => historyRecord.OrderId == orderId.Value);

            //ignore deleted orders
            query = from duh in query
                join order in _orderRepository.Table on duh.OrderId equals order.Id
                where !order.Deleted
                select duh;

            //order
            query = query.OrderByDescending(historyRecord => historyRecord.CreatedOnUtc)
                .ThenBy(historyRecord => historyRecord.Id);

            return query;
        }, pageIndex, pageSize);
    }

    /// <summary>
    /// Insert gift usage history record
    /// </summary>
    /// <param name="giftUsageHistory">Gift usage history record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertGiftUsageHistoryAsync(GiftUsageHistory giftUsageHistory)
    {
        await _giftUsageHistoryRepository.InsertAsync(giftUsageHistory);
    }

    /// <summary>
    /// Delete gift usage history record
    /// </summary>
    /// <param name="giftUsageHistory">Gift usage history record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteGiftUsageHistoryAsync(GiftUsageHistory giftUsageHistory)
    {
        await _giftUsageHistoryRepository.DeleteAsync(giftUsageHistory);
    }

    #endregion

    #endregion
}
