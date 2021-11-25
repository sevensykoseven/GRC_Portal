using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using protean.Infrastructure;
using protean.Areas.Admin.Models;
using MyUtils;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using protean.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Security;

namespace protean.Areas.Admin.Controllers
{
    [Authorize(Roles = "super")]
    [SelectedSidebar("usersandroles")]
    public class UsersController : Controller
    {
        #region Private Members

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        #endregion

        #region Class Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        public UsersController()
        {
        }

        /// <summary>
        /// Constructor with user Manager and signin manager
        /// </summary>
        /// <param name="userManager">ApplicationUserManager</param>
        /// <param name="signInManager">ApplicationSignInManager</param>
        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        /// <summary>
        /// GET: Admin/Users
        /// </summary>
        /// <returns>UserStore</returns>
        public ActionResult Index()
        {
            var userStore = new UserStore<ApplicationUser>(Database.ActiveContext);
            return View(userStore.Users.OrderBy(x => x.LastName));
        }

        /// <summary>
        /// Reset password for user
        /// </summary>
        /// <param name="id">Id of user to reset</param>
        /// <returns>ResetPasswordVM</returns>
        public ActionResult ResetPassword(string id)
        {
            // Check to see if the user actually exists.  Could indicate someone is trying to mess with the url.
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }
            //var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("Protean");
            //UserManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser>(provider.Create("PasswordChange"));

            var ResetPasswordVM = new protean.Areas.Admin.Models.ResetPasswordViewModel
            {
                User = user,
                UserId = user.Id
                //Code = UserManager.GeneratePasswordResetToken(user.Id)
            };

            return View(ResetPasswordVM);
        }

        /// <summary>
        /// Changes password
        /// </summary>
        /// <param name="form">ResetPasswordViewModel</param>
        /// <returns>Redirect to personal information with a message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(protean.Areas.Admin.Models.ResetPasswordViewModel form)
        {
            // Validate input
            if (!ModelState.IsValid)
            {
                form.User = UserManager.FindById(form.UserId);
                return View(form);
            }

            var user = await UserManager.FindByIdAsync(form.UserId);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            form.User = UserManager.FindById(form.UserId);

            //var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("Protean");
            //UserManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser>(provider.Create("PasswordChange"));
            string resetToken = await UserManager.GeneratePasswordResetTokenAsync(form.UserId);
            var result = await UserManager.ResetPasswordAsync(user.Id, resetToken, form.NewPassword);
            if (result.Succeeded)
            {
                TempData["response"] = "Password has been reset";
                return RedirectToAction("Edit", new { id = user.Id });
            }

            ModelState.AddModelError("", result.Errors.FirstOrDefault().ToString());
            TempData["failure"] = result.Errors.FirstOrDefault().ToString();

            return View(form);
        }

        /// <summary>
        /// GET: Admin/Users/Add
        /// </summary>
        /// <returns></returns>        
        public ActionResult Add()
        {
            var vm = new NewUserViewModel
            {
                IsEnabled = true
            };

            // If we are using local, populate with some data
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestData")))
            {
                vm.FirstName = "Frodo";
                vm.LastName = "Baggins";
                vm.Email = $"test{Convert.ToString(new Random().Next(1000))}@itnecessity.com";
            }

            return View(vm);
        }

        /// <summary>
        /// POST: Admin/Users/Add
        /// Create new user
        /// </summary>
        /// <param name="form">NewUser</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(NewUserViewModel form)
        {
            // Validate input
            if (!ModelState.IsValid)
                return View(form);

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));

            var user = new ApplicationUser
            {
                UserName = form.Email,
                Email = form.Email,
                IsEnabled = form.IsEnabled,
                FirstName = form.FirstName,
                LastName = form.LastName,
                Title = form.Title,
                BadgePath = ""
            };
            var result = await userManager.CreateAsync(user, form.Password);

            // Add basic role
            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(user.Id, "user");
                TempData["response"] = user.FullName + " has been created.";
                return RedirectToAction("EditRoles", new { id = user.Id });
            }

            return HttpNotFound();
        }

        /// <summary>
        /// GET: Admin/Users/Edit
        /// </summary>
        /// <param name="id">Id of user to find</param>
        /// <returns>User or redirects back to list of users if not found</returns>
        public ActionResult Edit(string id)
        {
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));

            // Check to see if the user actually exists.  Could indicate someone is trying to mess with the url.
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        /// <summary>
        /// Update the selected user
        /// </summary>
        /// <param name="id">User Id as string</param>
        /// <param name="form">Selected user as ApplicationUser</param>
        /// <returns>Redirect to Index if successful, or posts back if not</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, ApplicationUser form)
        {
            // Validate input
            if (!ModelState.IsValid)
                return View(form);

            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));

            var user = UserManager.FindById(id);
            if (user == null || user.Id != form.Id)
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
            user.SalesRepCode = form.SalesRepCode;

            var result = await UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["response"] = user.FullName + " has been saved.";
                return RedirectToAction(nameof(Index));
            }

            return View(form);
        }

        /// <summary>
        /// Edit Roles
        /// </summary>
        /// <param name="id">User Id as string</param>
        /// <returns></returns>
        public ActionResult EditRoles(string id)
        {
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));

            // Check to see if the user actually exists.  Could indicate someone is trying to mess with the url.
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Create the edit roles view model object
            var editRolesVM = new EditRolesViewModel
            {
                User = user,
                UserId = user.Id
            };

            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();

            // Get a list of roles from identity manager
            List<IdentityRole> roles = roleManager.Roles.ToList();
            var userRoles = user.RoleNames();

            // For each role in the list of roles, check to see if the user is in that role, if so then
            // mark it as selected
            foreach (var role in roles)
            {
                editRolesVM.Roles.Add(new SelectRoleEditor
                {
                    RoleName = role.Name,
                    Selected = userRoles.Where(r => r.ToString() == role.Name).Any()
                }); ;

            }

            return View(editRolesVM);
        }

        /// <summary>
        /// Go through the roles returned from the client and make the changes to the user
        /// </summary>
        /// <param name="form"EditRolesViewModel></param>
        /// <returns>RedirectAction</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRoles(EditRolesViewModel form)
        {
            // Validate input
            if (!ModelState.IsValid)
                return View(form);

            // Make sure user is in a role
            if (!form.Roles.Any(x => x.Selected))
            {
                ModelState.AddModelError("", "Error: You must select at least one role");
                return View(form);
            }

            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));

            var user = UserManager.FindById(form.UserId);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Go through the list of roles returned from the client and make the changes to the system
            foreach (var role in form.Roles)
            {
                if (role.Selected)
                {
                    UserManager.AddToRoles(user.Id, role.RoleName);
                }
                else
                {
                    UserManager.RemoveFromRole(user.Id, role.RoleName);
                }
            }

            TempData["response"] = user.FullName + " roles have been changed.";
            return RedirectToAction(nameof(Edit));
        }

        //[HttpGet]
        //public ActionResult Import()
        //{
        //    dynamic newObj;
        //    newObj = EAL.Workforce.GetAllWorkforce();

        //    //Utils.WriteDebugLog(newObj.ToString());
        //    var vm = new ImportSalesRepViewModel();
        //    vm.ResponseObject = newObj;

        //    return View(vm);
        //}

        ///// <summary>
        ///// Create sales rep by importing the rep from Epicor
        ///// </summary>
        ///// <param name="id">ID of epicor user to import</param>
        ///// <returns>Json object</returns>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Import(string id)
        //{

        //    dynamic obj = EAL.Workforce.GetWorkforceBySalesRepCode(id);
        //    if (obj.value.Count > 0)
        //    {
        //        //Utils.WriteDebugLog(obj.ToString());

        //        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));

        //        var user = new ApplicationUser();

        //        var userId = obj.value[0].SalesRep_EMailAddress.ToString();
        //        var names = obj.value[0].SalesRep_Name.ToString().Split(' ');
        //        user.UserName = userId;
        //        user.Email = userId;
        //        user.FirstName = names.Length != 0 ? names[0] : "";
        //        user.LastName = names.Length >= 2 ? names[1] : "";
        //        user.Title = obj.value[0].SalesRep_SalesRepTitle.ToString();
        //        user.BadgePath = "";
        //        user.SalesRepCode = obj.value[0].SalesRep_ShortChar01.ToString();
        //        user.IsEnabled = !Convert.ToBoolean(obj.value[0].SalesRep_InActive);

        //        var result = UserManager.Create(user, Membership.GeneratePassword(12, 1));

        //        // Add basic role
        //        if (result.Succeeded)
        //        {
        //            userManager.AddToRoles(user.Id, new string[] { "salesrep", "user" });

        //            // Add secess message
        //            TempData["response"] = user.FullName + " has been added.";

        //            return Json(new { success = true, id = user.Id }, JsonRequestBehavior.AllowGet);
        //        }
        //    }

        //    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult SalesReps()
        //{
        //    var users = new List<ApplicationUser>();

        //    foreach (var user in UserManager.Users.ToList())
        //    {
        //        if (UserManager.IsInRole(user.Id, "salesrep") || UserManager.IsInRole(user.Id, "repgroupprincipal") || UserManager.IsInRole(user.Id, "repgroupadmin"))
        //        {
        //            Utils.WriteDebugLog(user.FirstName);
        //            users.Add(user);
        //        }
        //    }

        //    return View(users.OrderBy(x => x.LastName));

        //    //var userStore = new UserStore<ApplicationUser>(Database.ActiveContext);
        //    //return View(userStore.Users.Where(u => u.SalesRepCode != null && u.SalesRepCode != string.Empty).OrderBy(x => x.LastName));
        //}

        #region AJAX Calls

        /// <summary>
        /// This disables the user
        /// </summary>
        /// <param name="id">Id of user to disable</param>
        /// <returns>Json object</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disable(string id)
        {
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));

            var user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.IsEnabled = false;

            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["response"] = user.FullName + " has been disabled.";
                return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, responseText = "Something when wrong" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This enables the user
        /// </summary>
        /// <param name="id">Id of user to enable</param>
        /// <returns>Json Object</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Enable(string id)
        {
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));

            var user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.IsEnabled = true;

            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["response"] = user.FullName + " has been enabled.";
                return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, responseText = "Something when wrong" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));

            var user = UserManager.FindById(id);
            if (user == null)
            {
                Json(new { success = false, responseText = "Something when wrong" }, JsonRequestBehavior.AllowGet);
            }
            var name = user.FullName;
            var result = await UserManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["response"] = name + " has been deleted permanently.";
                return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { success = false, responseText = "Something when wrong" }, JsonRequestBehavior.AllowGet);
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

    }
}