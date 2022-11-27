using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount search model
    /// </summary>
    public partial record GiftSearchModel : BaseSearchModel
    {
        #region Ctor

        public GiftSearchModel()
        {
            AvailableGiftTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Promotions.Gifts.List.SearchGiftCouponCode")]
        public string SearchGiftCouponCode { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.List.SearchGiftName")]
        public string SearchGiftName { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.List.SearchGiftType")]
        public int SearchGiftTypeId { get; set; }

        public IList<SelectListItem> AvailableGiftTypes { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.List.SearchStartDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchStartDate { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.List.SearchEndDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchEndDate { get; set; }

        #endregion
    }
}