using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;
using System.Linq;

namespace Nop.Plugin.Misc.GiftProvider;

public class GiftProvider : BasePlugin, IMiscPlugin, IAdminMenuPlugin
{

    #region Admin Menu
    public Task ManageSiteMapAsync(SiteMapNode rootNode)
    {
        var promotionsNode = rootNode.ChildNodes.First(x => x.SystemName == "Promotions");

        var giftsMenuItem = new SiteMapNode()
        {
            SystemName = "Gifts",
            Title = "Gifts",
            ControllerName = "GiftController",
            ActionName = "List",
            Visible = true,
            IconClass = "far fa-dot",
            RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
        };

        promotionsNode.ChildNodes.Add(giftsMenuItem);

        return Task.CompletedTask;
    }


    #endregion
}