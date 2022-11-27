using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a category model to add to the gift
    /// </summary>
    public partial record AddCategoryToGiftModel : BaseNopModel
    {
        #region Ctor

        public AddCategoryToGiftModel()
        {
            SelectedCategoryIds = new List<int>();
        }
        #endregion

        #region Properties

        public int GiftId { get; set; }

        public IList<int> SelectedCategoryIds { get; set; }

        #endregion
    }
}