using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount usage history list model
    /// </summary>
    public partial record GiftUsageHistoryListModel : BasePagedListModel<GiftUsageHistoryModel>
    {
    }
}