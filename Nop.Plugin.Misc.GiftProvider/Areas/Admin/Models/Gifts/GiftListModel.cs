using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount list model
    /// </summary>
    public partial record GiftListModel : BasePagedListModel<GiftModel>
    {
    }
}