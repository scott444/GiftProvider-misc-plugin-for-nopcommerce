using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount manufacturer search model
    /// </summary>
    public partial record GiftManufacturerSearchModel : BaseSearchModel
    {
        #region Properties

        public int GiftId { get; set; }

        #endregion
    }
}