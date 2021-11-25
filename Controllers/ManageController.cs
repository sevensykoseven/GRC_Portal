using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using protean.Models;

namespace protean.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        #region Private Members

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        #endregion

        #region Public Members

        /// <summary>
        /// Constructor
        /// </summary>
        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

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

        #region MyProfile

        /// <summary>
        /// Load current user's profile
        /// </summary>
        /// <returns>returns User</returns>
        public ActionResult MyProfile()
        {
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));

            // Check to see if the user actually exists.  Could indicate someone is trying to mess with the url.
            var user = this.UserManager.FindById(Current.User.Id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        /// <summary>
        /// Update the selected user
        /// </summary>
        /// <param name="form">Selected user as ApplicationUser</param>
        /// <returns>Redirect to Index if successful, or posts back if not</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MyProfile(ApplicationUser form)
        {
            // Validate input
            if (!ModelState.IsValid)
                return View(form);

            var user = this.UserManager.FindById(Current.User.Id);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.Email = form.Email;
            user.UserName = form.Email;
            user.FirstName = form.FirstName;
            user.LastName = form.LastName;
            //user.PhoneNumber = form.PhoneNumber;
            user.Title = form.Title;
            //user.BadgePath = form.BadgePath;

            var result = await this.UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["response"] = "Save successful.";
                return RedirectToAction("MyProfile");
            }

            return View(form);
        }

        #endregion          

        #region Change Password

        /// <summary>
        /// Get the current user information
        /// </summary>
        /// <returns>View</returns>
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirect to action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await this.UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await this.SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: false);
                }
                TempData["response"] = "Password has been changed.";
                return RedirectToAction("MyProfile");
            }
            
            return View(model);
        }
        #endregion

        #region Protected Members

        /// <summary>
        /// Override disposing
        /// </summary>
        /// <param name="disposing">Boolean</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}