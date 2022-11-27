using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a product model to add to the gift
    /// </summary>
    public partial record AddProductToGiftModel : BaseNopModel
    {
        #region Ctor

        public AddProductToGiftModel()
        {
            SelectedProductIds = new List<int>();
        }
        #endregion

        #region Properties

        public int GiftId { get; set; }

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }
}