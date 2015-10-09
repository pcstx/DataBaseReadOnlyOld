using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DataBase
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
             name: "rowsJson",
             url: "Home/{action}/{dbName}/{TableName}",
             defaults: new { controller = "Home", action = "RowsJson", dbName = UrlParameter.Optional, TableName = UrlParameter.Optional }
         );

            routes.MapRoute(
           name: "TablesJson",
           url: "Home/TablesJson/{dbName}",
           defaults: new { controller = "Home", action = "TablesJson", dbName = UrlParameter.Optional }
       );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

         routes.MapRoute(
           name: "Design",
           url: "Design/{action}/{dbName}/{viewName}",
           defaults: new { controller = "Design", action = "ViewDesign", dbName = UrlParameter.Optional, viewName = UrlParameter.Optional }
       ); 

        }
    }
}
