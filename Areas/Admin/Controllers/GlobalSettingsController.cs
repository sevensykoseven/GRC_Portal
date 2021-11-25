using System.Web.Mvc;
using protean.Infrastructure;

namespace protean.Areas.Admin.Controllers
{
    [SelectedSidebar("global")]
    [Authorize(Roles = "super")]
    public class GlobalSettingsController : Controller
    {
        /// <summary>
        /// GET: Index
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}