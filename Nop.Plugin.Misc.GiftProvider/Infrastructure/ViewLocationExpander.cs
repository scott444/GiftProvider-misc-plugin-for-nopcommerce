using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Web.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Misc.GiftProvider.Infrastructure;

public class ViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        const string prefix = "/Plugins/Misc.GiftProvider";
        string adminViews = $"{prefix}/Areas/Admin/Views";
        string publicViews = $"{prefix}/Views/Public";

        if (context.AreaName == AreaNames.Admin)
        {
            viewLocations = new[] { $"{adminViews}/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
            viewLocations = new[] { $"{adminViews}/Shared/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }
        else
        {
            viewLocations = new[] { $"{publicViews}/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
            viewLocations = new[] { $"{publicViews}/Shared/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }

        return viewLocations;
    }
}
