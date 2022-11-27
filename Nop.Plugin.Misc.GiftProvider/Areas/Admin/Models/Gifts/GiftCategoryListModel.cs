using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount category list model
    /// </summary>
    public partial record GiftCategoryListModel : BasePagedListModel<GiftCategoryModel>
    {
    }
}