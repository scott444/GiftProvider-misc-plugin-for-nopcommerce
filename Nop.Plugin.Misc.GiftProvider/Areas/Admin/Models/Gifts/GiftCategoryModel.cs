using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a gift category model
    /// </summary>
    public partial record GiftCategoryModel : BaseNopEntityModel
    {
        #region Properties

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        #endregion
    }
}