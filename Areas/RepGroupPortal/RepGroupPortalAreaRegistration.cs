using System.Web.Mvc;

namespace protean.Areas.RepGroupPortal
{
    public class RepGroupPortalAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RepGroupPortal";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RepGroupPortal_default",
                "RepGroupPortal/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}