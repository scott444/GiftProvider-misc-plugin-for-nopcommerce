using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a manufacturer search model to add to the gift
    /// </summary>
    public partial record AddManufacturerToGiftSearchModel : BaseSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Manufacturers.List.SearchManufacturerName")]
        public string SearchManufacturerName { get; set; }

        #endregion
    }
}