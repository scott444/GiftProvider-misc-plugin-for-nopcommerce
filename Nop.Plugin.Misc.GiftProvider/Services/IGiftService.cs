using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Services;

/// <summary>
/// Gift service interface
/// </summary>
public partial interface IGiftService
{
    #region Gifts

    /// <summary>
    /// Delete gift
    /// </summary>
    /// <param name="gift">Gift</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteGiftAsync(Gift gift);

    /// <summary>
    /// Gets a gift
    /// </summary>
    /// <param name="giftId">Gift identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift
    /// </returns>
    Task<Gift> GetGiftByIdAsync(int giftId);

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
    Task<IList<Gift>> GetAllGiftsAsync(GiftType? giftType = null,
        string couponCode = null, string giftName = null, bool showHidden = false,
        DateTime? startDateUtc = null, DateTime? endDateUtc = null);

    /// <summary>
    /// Inserts a gift
    /// </summary>
    /// <param name="gift">Gift</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertGiftAsync(Gift gift);

    /// <summary>
    /// Updates the gift
    /// </summary>
    /// <param name="gift">Gift</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateGiftAsync(Gift gift);

    /// <summary>
    /// Gets gifts applied to entity
    /// </summary>
    /// <typeparam name="T">Type based on <see cref="GiftMapping" /></typeparam>
    /// <param name="entity">Entity which supports gifts (<see cref="IGiftSupported{T}" />)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of gifts
    /// </returns>
    Task<IList<Gift>> GetAppliedGiftsAsync<T>(IGiftSupported<T> entity) where T : GiftMapping;

    #endregion

    #region Gifts (caching)

    /// <summary>
    /// Gets the gift amount for the specified value
    /// </summary>
    /// <param name="gift">Gift</param>
    /// <param name="amount">Amount</param>
    /// <returns>The gift amount</returns>
    decimal GetGiftAmount(Gift gift, decimal amount);

    /// <summary>
    /// Get preferred gift (with maximum gift value)
    /// </summary>
    /// <param name="gifts">A list of gifts to check</param>
    /// <param name="amount">Amount (initial value)</param>
    /// <param name="giftAmount">Gift amount</param>
    /// <returns>Preferred gift</returns>
    List<Gift> GetPreferredGift(IList<Gift> gifts,
        decimal amount, out decimal giftAmount);

    /// <summary>
    /// Check whether a list of gifts already contains a certain gift instance
    /// </summary>
    /// <param name="gifts">A list of gifts</param>
    /// <param name="gift">Gift to check</param>
    /// <returns>Result</returns>
    bool ContainsGift(IList<Gift> gifts, Gift gift);

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
    Task<IList<GiftRequirement>> GetAllGiftRequirementsAsync(int giftId = 0, bool topLevelOnly = false);

    /// <summary>
    /// Get a gift requirement
    /// </summary>
    /// <param name="giftRequirementId">Gift requirement identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<GiftRequirement> GetGiftRequirementByIdAsync(int giftRequirementId);

    /// <summary>
    /// Gets child gift requirements
    /// </summary>
    /// <param name="giftRequirement">Parent gift requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<IList<GiftRequirement>> GetGiftRequirementsByParentAsync(GiftRequirement giftRequirement);

    /// <summary>
    /// Delete gift requirement
    /// </summary>
    /// <param name="giftRequirement">Gift requirement</param>
    /// <param name="recursively">A value indicating whether to recursively delete child requirements</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteGiftRequirementAsync(GiftRequirement giftRequirement, bool recursively);

    /// <summary>
    /// Inserts a gift requirement
    /// </summary>
    /// <param name="giftRequirement">Gift requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertGiftRequirementAsync(GiftRequirement giftRequirement);

    /// <summary>
    /// Updates a gift requirement
    /// </summary>
    /// <param name="giftRequirement">Gift requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateGiftRequirementAsync(GiftRequirement giftRequirement);

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
    Task<GiftValidationResult> ValidateGiftAsync(Gift gift, Customer customer);

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
    Task<GiftValidationResult> ValidateGiftAsync(Gift gift, Customer customer, string[] couponCodesToValidate);

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
    Task<GiftUsageHistory> GetGiftUsageHistoryByIdAsync(int giftUsageHistoryId);

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
    Task<IPagedList<GiftUsageHistory>> GetAllGiftUsageHistoryAsync(int? giftId = null,
        int? customerId = null, int? orderId = null,
        int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Insert gift usage history record
    /// </summary>
    /// <param name="giftUsageHistory">Gift usage history record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertGiftUsageHistoryAsync(GiftUsageHistory giftUsageHistory);

    /// <summary>
    /// Delete gift usage history record
    /// </summary>
    /// <param name="giftUsageHistory">Gift usage history record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteGiftUsageHistoryAsync(GiftUsageHistory giftUsageHistory);

    #endregion
}