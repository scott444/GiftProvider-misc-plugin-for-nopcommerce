using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount product list model
    /// </summary>
    public partial record GiftProductListModel : BasePagedListModel<GiftProductModel>
    {
    }
}