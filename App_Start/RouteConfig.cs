using System.Web.Mvc;
using System.Web.Routing;

namespace protean
{
    public class RouteConfig
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routes">RouteCollection</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            // Login route
            routes.MapRoute(
                name: "login",
                url: "login",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );

            // MyProfile route
            routes.MapRoute(
                name: "profile",
                url: "profile",
                defaults: new { controller = "Manage", action = "MyProfile" }
            );

            // Settings route
            routes.MapRoute(
                name: "settings",
                url: "settings",
                defaults: new { controller = "Account", action = "Settings", id = UrlParameter.Optional }
            );

            // Home route
            routes.MapRoute(
                name: "home",
                url: "dashboard",
                defaults: new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );

            // Logout route
            routes.MapRoute(
                name: "logout",
                url: "logout",
                defaults: new { controller = "Account", action = "Logout", id = UrlParameter.Optional }
            );

            // Login route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
       
        }
    }
}
