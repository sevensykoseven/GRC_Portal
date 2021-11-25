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
    [Authorize(Roles = "user")]
    public class AccountController : Controller
    {
        #region Private Members

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        #endregion

        #region Class Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AccountController()
        {
        }

        /// <summary>
        /// Constructor with user Manager and signin manager
        /// </summary>
        /// <param name="userManager">ApplicationUserManager</param>
        /// <param name="signInManager">ApplicationSignInManager</param>
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        /// <summary>
        /// Public Property
        /// </summary>
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

        /// <summary>
        /// Public Property
        /// </summary>
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

        #region Anonymous Methods

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="returnUrl">URL to redirect user to after successful login</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Return = returnUrl;
                return RedirectToLocal(returnUrl);
            }
          
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="model">LoginViewModel</param>
        /// <param name="returnUrl">URL to redirect user to after successful login</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    //Utils.WriteDebugLog(model.Email.ToString());                    
                    return RedirectToLocal(returnUrl);
                //case SignInStatus.LockedOut:
                //    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = true });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Login failed.");
                    return View(model);
                    
            }
            
        }

        /// <summary>
        /// Forgot password view
        /// </summary>
        /// <returns>View</returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Send forgot password email.
        /// </summary>
        /// <param name="model">ForgotPasswordViewModel</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                var sb = new StringBuilder();
                sb.AppendLine("A password password reset request has been made for your account on Grand Rapids Chair Co portal.");
                sb.AppendLine("<br />");
                sb.AppendLine("<br />");
                sb.AppendLine("Please reset your password by clicking <a href='" + callbackUrl + "'>here</a>  or copy and paste the following into your address bar:");
                sb.AppendLine("<br />");
                sb.AppendLine("<br />");
                sb.AppendLine(callbackUrl);
                sb.AppendLine("<br />");
                sb.AppendLine("<br />");
                sb.AppendLine("Should you have any concerns, please reach out to your Account Manager.");
                await UserManager.SendEmailAsync(user.Id, "Reset Password", sb.ToString());
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Get Forgot password confirmation view
        /// </summary>
        /// <returns>view</returns>
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Initial Reset Password 
        /// </summary>
        /// <param name="code">Code from email</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            var vm = new ResetPasswordViewModel();
            vm.Code = code;
            if (code == null)
            {
                return RedirectToAction("Login");
            }
            return View(vm);
        }

        /// <summary>
        /// Reset the password for this user
        /// </summary>
        /// <param name="model">ResetPasswordViewModel</param>
        /// <returns>view</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        /// <summary>
        /// Reset password confirmation
        /// </summary>
        /// <returns>view</returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Log out user
        /// </summary>
        /// <returns>RedirectToAction</returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction(FormsAuthentication.LoginUrl);
        }

        #endregion

        #region Protected Events

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">Boolean</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Handle sending user back to page they are intending before login
        /// </summary>
        /// <param name="returnUrl">Return URL</param>
        /// <returns>RedirectToAction</returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            //// Set default dashboard based on role type
            //if (User.IsInRole("super"))
            //{
            //    return RedirectToAction("Index", "Users", new { area = "Admin" });
            //}

            //if (User.IsInRole("repgroupprincipal") || User.IsInRole("repgroupadmin"))
            //{
            //    // if this is a sales rep and they are inactive, do not let them log in.
            //    if (EAL.Workforce.IsRepInActive(Current.User.SalesRepCode))
            //    {
            //        FormsAuthentication.SignOut();
            //        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //        return RedirectToAction(FormsAuthentication.LoginUrl);
            //    }
            //    return RedirectToAction("Index", "RepGroupDB", new { area = "RepGroupPortal" });
            //}

            //if (User.IsInRole("salesrep"))
            //{
            //    // if this is a sales rep and they are inactive, do not let them log in.
            //    if (EAL.Workforce.IsRepInActive(Current.User.SalesRepCode))
            //    {
            //        FormsAuthentication.SignOut();
            //        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //        return RedirectToAction(FormsAuthentication.LoginUrl);
            //    }
            //    return RedirectToAction("Index", "RepDB", new { area = "RepPortal" });
            //}

            //return RedirectToAction("Index", "Dashboard");

            return RedirectToAction("Auth", "Dashboard");
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
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion

    }

}