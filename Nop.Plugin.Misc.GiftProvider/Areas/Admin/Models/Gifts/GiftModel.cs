using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount model
    /// </summary>
    public partial record GiftModel : BaseNopEntityModel
    {
        #region Ctor

        public GiftModel()
        {
            AvailableGiftRequirementRules = new List<SelectListItem>();
            AvailableRequirementGroups = new List<SelectListItem>();
            GiftUsageHistorySearchModel = new GiftUsageHistorySearchModel();
            GiftProductSearchModel = new GiftProductSearchModel();
            GiftCategorySearchModel = new GiftCategorySearchModel();
            GiftManufacturerSearchModel = new GiftManufacturerSearchModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.AdminComment")]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.GiftType")]
        public int GiftTypeId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.GiftType")]
        public string GiftTypeName { get; set; }

        //used for the list page
        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.TimesUsed")]
        public int TimesUsed { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.UsePercentage")]
        public bool UsePercentage { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.GiftPercentage")]
        public decimal GiftPercentage { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.GiftAmount")]
        public decimal GiftAmount { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.MaximumGiftAmount")]
        [UIHint("DecimalNullable")]
        public decimal? MaximumGiftAmount { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.StartDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? StartDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.EndDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? EndDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.RequiresCouponCode")]
        public bool RequiresCouponCode { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.GiftUrl")]
        public string GiftUrl { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.CouponCode")]
        public string CouponCode { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.IsCumulative")]
        public bool IsCumulative { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.GiftLimitation")]
        public int GiftLimitationId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.LimitationTimes")]
        public int LimitationTimes { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.MaximumGiftedQuantity")]
        [UIHint("Int32Nullable")]
        public int? MaximumGiftedQuantity { get; set; }
        
        [NopResourceDisplayName("Admin.Promotions.Gifts.Fields.AppliedToSubCategories")]
        public bool AppliedToSubCategories { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Requirements.GiftRequirementType")]
        public string AddGiftRequirement { get; set; }

        public IList<SelectListItem> AvailableGiftRequirementRules { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Requirements.GroupName")]
        public string GroupName { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.Requirements.RequirementGroup")]
        public int RequirementGroupId { get; set; }

        public IList<SelectListItem> AvailableRequirementGroups { get; set; }

        public GiftUsageHistorySearchModel GiftUsageHistorySearchModel { get; set; }

        public GiftProductSearchModel GiftProductSearchModel { get; set; }

        public GiftCategorySearchModel GiftCategorySearchModel { get; set; }

        public GiftManufacturerSearchModel GiftManufacturerSearchModel { get; set; }

        #endregion
    }
}