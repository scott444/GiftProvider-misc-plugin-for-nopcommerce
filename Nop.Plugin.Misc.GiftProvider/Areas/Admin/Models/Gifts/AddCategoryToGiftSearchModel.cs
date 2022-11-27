using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a category search model to add to the discount
    /// </summary>
    public partial record AddCategoryToGiftSearchModel : BaseSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchCategoryName")]
        public string SearchCategoryName { get; set; }

        #endregion
    }
}