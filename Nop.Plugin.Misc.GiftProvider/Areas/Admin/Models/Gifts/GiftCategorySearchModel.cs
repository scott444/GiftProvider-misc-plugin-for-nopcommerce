using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount category search model
    /// </summary>
    public partial record GiftCategorySearchModel : BaseSearchModel
    {
        #region Properties

        public int GiftId { get; set; }

        #endregion
    }
}