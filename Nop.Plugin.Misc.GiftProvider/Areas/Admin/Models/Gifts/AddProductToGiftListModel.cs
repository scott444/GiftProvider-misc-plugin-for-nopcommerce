using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a product list model to add to the discount
    /// </summary>
    public partial record AddProductToGiftListModel : BasePagedListModel<ProductModel>
    {
    }
}