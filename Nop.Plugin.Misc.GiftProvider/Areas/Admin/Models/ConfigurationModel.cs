using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models;

public record ConfigurationModel : BaseNopModel
{
    #region Ctor

    public ConfigurationModel()
    {
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    

    #endregion
}

