using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount manufacturer model
    /// </summary>
    public partial record GiftManufacturerModel : BaseNopEntityModel
    {
        #region Properties

        public int ManufacturerId { get; set; }

        public string ManufacturerName { get; set; }

        #endregion
    }
}