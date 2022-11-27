using System;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount usage history model
    /// </summary>
    public partial record GiftUsageHistoryModel : BaseNopEntityModel
    {
        #region Properties

        public int GiftId { get; set; }

        public int OrderId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.History.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.History.OrderTotal")]
        public string OrderTotal { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Gifts.History.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}