using System.Web.Mvc;

namespace protean
{
    public class FilterConfig
    {
        /// <summary>
        /// Register Global Filters
        /// </summary>
        /// <param name="filters">GlobalFilterCollection</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // Global filter to force all controllers and actions to be authorized unless AllowAnonymons is set
            filters.Add(new AuthorizeAttribute());
        }
    }
}
