using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a manufacturer model to add to the discount
    /// </summary>
    public partial record AddManufacturerToGiftModel : BaseNopModel
    {
        #region Ctor

        public AddManufacturerToGiftModel()
        {
            SelectedManufacturerIds = new List<int>();
        }
        #endregion

        #region Properties

        public int GiftId { get; set; }

        public IList<int> SelectedManufacturerIds { get; set; }

        #endregion
    }
}