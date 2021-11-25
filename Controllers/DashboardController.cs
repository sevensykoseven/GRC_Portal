using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using protean.Infrastructure;
using protean.Models;

namespace protean.Controllers
{
    [SelectedSidebar("dashboard")]
    public class DashboardController : Controller
    {

        #region Private Members

        private ApplicationUserManager _userManager;

        #endregion

        #region Public Members
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        /// <summary>
        /// Default Dashboard
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This was created to fix a strange issue where when the user was logging in shortly after logging off, the User.IsInRoles was empty
        /// </summary>
        /// <returns>Redirect Action to correct starting page for user role.</returns>
        public ActionResult Auth()
        {

            //Add ability to record last authentication activity
            var user = this.UserManager.FindById(Current.User.Id);
            if (user != null)
            {
                user.LastAuth = System.DateTime.Now.ToUniversalTime();
                this.UserManager.Update(user);
            }

            // Set default dashboard based on role type
            if (User.IsInRole("super") || User.IsInRole("grc"))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            if (User.IsInRole("repgroupprincipal") || User.IsInRole("repgroupadmin"))
            {
                // if this is a sales rep and they are inactive, do not let them log in.
                if (EAL.Workforce.IsRepInActive(Current.User.SalesRepCode))
                {
                    FormsAuthentication.SignOut();
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction(FormsAuthentication.LoginUrl);
                }
                return RedirectToAction("Index", "RepGroupDB", new { area = "RepGroupPortal" });
            }

            if (User.IsInRole("salesrep"))
            {
                // if this is a sales rep and they are inactive, do not let them log in.
                if (EAL.Workforce.IsRepInActive(Current.User.SalesRepCode))
                {
                    FormsAuthentication.SignOut();
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction(FormsAuthentication.LoginUrl);
                }
                return RedirectToAction("Index", "RepDB", new { area = "RepPortal" });
            }

            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// AuthenticationManager
        /// </summary>
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}