using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a manufacturer list model to add to the gift
    /// </summary>
    public partial record AddManufacturerToGiftListModel : BasePagedListModel<ManufacturerModel>
    {
    }
}