using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount usage history search model
    /// </summary>
    public partial record GiftUsageHistorySearchModel : BaseSearchModel
    {
        #region Properties

        public int GiftId { get; set; }

        #endregion
    }
}