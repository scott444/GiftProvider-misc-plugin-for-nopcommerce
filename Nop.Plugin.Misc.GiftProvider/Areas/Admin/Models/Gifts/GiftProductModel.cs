using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount product model
    /// </summary>
    public partial record GiftProductModel : BaseNopEntityModel
    {
        #region Properties

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        #endregion
    }
}