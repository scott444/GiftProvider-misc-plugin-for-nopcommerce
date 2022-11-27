using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts
{
    /// <summary>
    /// Represents a discount requirement rule model
    /// </summary>
    public partial record GiftRequirementRuleModel : BaseNopModel
    {
        #region Ctor

        public GiftRequirementRuleModel()
        {
            ChildRequirements = new List<GiftRequirementRuleModel>();
        }

        #endregion

        #region Properties

        public int GiftRequirementId { get; set; }

        public string RuleName { get; set; }

        public string ConfigurationUrl { get; set; }

        public string InteractionType { get; set; }

        public int? ParentId { get; set; }

        public SelectList AvailableInteractionTypes { get; set; }

        public bool IsGroup { get; set; }

        public bool IsLastInGroup { get; set; }

        public IList<GiftRequirementRuleModel> ChildRequirements { get; set; }

        #endregion
    }
}