using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a category list model to add to the gift
    /// </summary>
    public partial record AddCategoryToGiftListModel : BasePagedListModel<CategoryModel>
    {
    }
}