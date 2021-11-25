using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using MyUtils;
using protean.Infrastructure;

namespace protean
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Run on application begin request
        /// </summary>
        protected void Application_BeginRequest()
        {
            //if (User == null)
            //    Response.Redirect(FormsAuthentication.LoginUrl, true);

            var context = new HttpContextWrapper(Context);
            if (context.Request.IsAjaxRequest())
                context.Response.SuppressFormsAuthenticationRedirect = true;

            //// Check for the environment type and encrypt the connection string on Live and Staging
            //var environmentType = EnvironmentSettings.AppSettings("EnvironmentType").ToLower();
            //if (environmentType == "production" || environmentType == "staging")
            //   MyUtils.ConfigManager.EncryptConnString("connectionStrings");

            // If request is for http,  redirect request to https
            if (FormsAuthentication.RequireSSL && !Request.IsSecureConnection)
                Response.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));

            // If url is missing www, then reroute user to site with www.  This is done to keep everything consistent
            // and also help with any SSL issues if we decide not to have a wildcard cert or go with a cert authority
            // that does not provide a cert with both options without additional cost.
            //if (Request.Url.Host.ToLower() == "portal.grandrapidschair.com")
            //{
            //    Response.Clear();
            //    Response.Status = "301 Moved Permanently";
            //    Response.StatusCode = 301;
            //    Response.AddHeader("Location", "https://portal.grandrapidschair.com" + Request.RawUrl);
            //}

            Database.OpenSession();
        }

        /// <summary>
        /// Run on application end request
        /// </summary>
        protected void Application_EndRequest()
        {
            Database.CloseSession();

            var context = new HttpContextWrapper(Context);
            if (context.Request.IsAjaxRequest())
                context.Response.SuppressFormsAuthenticationRedirect = true;
            // If we're an ajax request and forms authentication caused a 302, then we actually need to do a 401
            if (FormsAuthentication.IsEnabled && context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
            {
                context.Response.Clear();
                context.Response.StatusCode = 401;
            }
        }

        /// <summary>
        /// Run on application error
        /// </summary>
        protected void Application_Error()
        {
            var ex = Server.GetLastError();

            // Record the error to error log and send out email
            WebErrorHandler.HandleError(HttpContext.Current.Request.Url.ToString(), ex);

            if (ex is HttpAntiForgeryException)
            {
                Response.Clear();
                Server.ClearError();
                Response.Redirect("/", true);
            }
        }

        /// <summary>
        /// Run at application start
        /// </summary>
        protected void Application_Start()
        {
            var environmentType = System.Configuration.ConfigurationManager.AppSettings["EnvironmentType"].ToLower();
            if (environmentType == "production" || environmentType == "staging" || environmentType == "development4")
            {
               //MyUtils.ConfigManager.EncryptConnString("connectionStrings");
               MyUtils.ConfigManager.DecryptConnString("connectionStrings");
            }
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(string), new TrimModelBinder());

            //Only use the RazorEngine. - http://blogs.msdn.com/b/marcinon/archive/2011/08/16/optimizing-mvc-view-lookup-performance.aspx
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

        /// <summary>
        /// try to keep session and forms auth cookies in sync
        /// </summary>
        protected void Session_Start()
        {
            if (User.Identity.IsAuthenticated)
            {
                //Utils.WriteDebugLog("Session starting but user still authenticated");
                //FormsAuthentication.SignOut();
                //Response.Redirect(FormsAuthentication.LoginUrl, true);
            }
        }
    }
}
