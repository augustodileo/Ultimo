using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ultimo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Null User Index",
                url: "User/Index/",
                defaults: new { controller = "User", action = "NullIndex" }
            );
            routes.MapRoute(
                name: "Null Apunte Index",
                url: "Apunte/Index/",
                defaults: new { controller = "Apunte", action = "NullIndex"}
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
