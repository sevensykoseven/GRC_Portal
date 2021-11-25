using protean.Areas.Employee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using protean.Infrastructure;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using protean.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Security;
using OfficeOpenXml;
using Newtonsoft.Json;
using System.Data;
using OfficeOpenXml.Table;
using System.IO;
using FastMember;

namespace protean.Areas.Employee.Controllers
{
    [Authorize(Roles = "grc")]
    public class SalesRepsController : Controller
    {
        #region Private Members

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        #endregion

        #region Class Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SalesRepsController()
        {
        }

        /// <summary>
        /// Constructor with user Manager and signin manager
        /// </summary>
        /// <param name="userManager">ApplicationUserManager</param>
        /// <param name="signInManager">ApplicationSignInManager</param>
        public SalesRepsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        /// Default index.  Could be a dashboard.  Right now not really used
        /// </summary>
        /// <returns>view</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get all work force records from Epicor
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult Import()
        {
            dynamic newObj;
            newObj = EAL.Workforce.GetAllWorkforce();

            var vm = new ImportSalesRepViewModel();
            vm.ResponseObject = newObj;

            return View(vm);
        }

        /// <summary>
        /// Create sales rep by importing the rep from Epicor
        /// </summary>
        /// <param name="id">ID of epicor user to import</param>
        /// <returns>Json object</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(string id)
        {
            dynamic obj = EAL.Workforce.GetWorkforceBySalesRepCode(id);
            if (obj.value.Count > 0)
            {

                var user = new ApplicationUser();

                var userId = obj.value[0].SalesRep_EMailAddress.ToString();
                var names = obj.value[0].SalesRep_Name.ToString().Split(' ');
                user.UserName = userId;
                user.Email = userId;
                user.FirstName = names.Length != 0 ? names[0] : "";
                user.LastName = names.Length >= 2 ? names[1] : "";
                user.Title = obj.value[0].SalesRep_SalesRepTitle.ToString();
                user.BadgePath = "";
                //user.SalesRepCode = obj.value[0].SalesRep_ShortChar01.ToString();
                user.SalesRepCode = obj.value[0].SalesRep_SalesRepCode.ToString();  // fix 2021-06-10
                user.IsEnabled = !Convert.ToBoolean(obj.value[0].SalesRep_InActive);

                var result = UserManager.Create(user, Membership.GeneratePassword(30, 1));

                // Add basic role
                if (result.Succeeded)
                {
                    UserManager.AddToRoles(user.Id, new string[] { "salesrep", "user" });

                    // Add secess message
                    TempData["response"] = user.FullName + " has been added.";

                    return Json(new { success = true, id = user.Id }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all active workforce as excel
        /// </summary>
        /// <returns>Excel file</returns>
        [HttpGet]
        public FileStreamResult GetAllActiveWorkforce_Excel()
        {
            // Get latest open order commissions
            dynamic newObj = EAL.Workforce.GetAllWorkforce();

            // set license
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // create an excel doc
            using (var pck = new ExcelPackage())
            {
                // convert data first into a dataset and then into a datatable
                // there is probably a better way to do with but I just couldn't get it to work
                // any other way                
                string json = "{'value':" + newObj.value.ToString() + "}";
                var ds = JsonConvert.DeserializeObject<DataSet>(json);
                var dt = new DataTable();
                dt = ds.Tables[0];

                // Now customize what the excel sheet looks like (column names, and columns)
                var customizedData = dt
                    .AsEnumerable()
                    .Select(x => new
                    {
                        Name = x.Field<string>("SalesRep_Name"),
                        SalesRepId = x.Field<string>("SalesRep_SalesRepCode"),
                        Title = x.Field<string>("SalesRep_SalesRepTitle"),
                        Address1 = x.Field<string>("SalesRep_Address1"),
                        Address2 = x.Field<string>("SalesRep_Address2"),
                        Address3 = x.Field<string>("SalesRep_Address3"),
                        City = x.Field<string>("SalesRep_City"),
                        State = x.Field<string>("SalesRep_State"),
                        Zip = x.Field<string>("SalesRep_Zip"),
                        Country = x.Field<string>("SalesRep_Country"),
                        Email = x.Field<string>("SalesRep_EMailAddress"),
                        Region = x.Field<string>("SalesRep_ShortChar02"),
                        Group = x.Field<string>("UDCodes_CodeDesc"),
                        GroupId = x.Field<string>("UDCodes_CodeID"),
                        OfficeNum = x.Field<string>("SalesRep_OfficePhoneNum"),
                        CellNum = x.Field<string>("SalesRep_CellPhoneNum"),
                        FaxNum = x.Field<string>("SalesRep_FaxPhoneNum")
                    }); ;

                /// Create worksheet
                var ws = pck.Workbook.Worksheets.Add("Active Epicor Workforce");

                //Load the datatable and set the number formats...
                ws.Cells["A1"].LoadFromCollection(customizedData, true, TableStyles.Medium9);
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                /// Create stream and send file
                var memoryStream = new MemoryStream();

                pck.SaveAs(memoryStream);
                memoryStream.Position = 0;

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = String.Format("Active Epicor Workforce {0}.xlsx", DateTime.Today.ToString("yyyy-MM-dd")),
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        /// <summary>
        /// Edit user
        /// </summary>
        /// <param name="id">ID of user</param>
        /// <returns>User or redirects back to list of users if not found</returns>
        [HttpGet]
        public ActionResult Edit(string id)
        {
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

            var user = UserManager.FindById(id);
            if (user == null || user.Id != form.Id)
            {
                return HttpNotFound();
            }

            user.Email = form.Email;
            user.UserName = form.Email;
            user.FirstName = form.FirstName;
            user.LastName = form.LastName;
            user.Title = form.Title;
            user.SalesRepCode = form.SalesRepCode;

            var result = await UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["response"] = user.FullName + " has been saved.";
                return RedirectToAction(nameof(RepList));
            }

            return View(form);
        }

        /// <summary>
        /// Get all users that have a rep role.  If roles are added or changed, this will also need to change.
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult RepList()
        {
            var users = new List<ApplicationUser>();

            foreach (var user in UserManager.Users.ToList())
            {
                if (UserManager.IsInRole(user.Id, "salesrep") || UserManager.IsInRole(user.Id, "repgroupprincipal") || UserManager.IsInRole(user.Id, "repgroupadmin"))
                {
                    users.Add(user);
                }
            }

            return View(users.OrderBy(x => x.LastName));
        }

        /// <summary>
        /// Get the list of sales reps in system to excel
        /// </summary>
        /// <returns>Excel file stream</returns>
        [HttpGet]
        public FileStreamResult GetRepList_Excel()
        {
            // set license
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // create an excel doc
            using (var pck = new ExcelPackage())
            {
                var users = new List<ApplicationUser>();

                foreach (var user in UserManager.Users.ToList())
                {
                    if (UserManager.IsInRole(user.Id, "salesrep") || UserManager.IsInRole(user.Id, "repgroupprincipal") || UserManager.IsInRole(user.Id, "repgroupadmin"))
                    {
                        users.Add(user);
                    }
                }

                var dt = new DataTable();
                using (var reader = ObjectReader.Create(users, "LastName", "FirstName", "Email", "Title", "SalesRepCode", "LastAuth", "IsEnabled", "Id"))
                {
                    dt.Load(reader);
                }

                // Now customize what the excel sheet looks like (column names, and columns)
                var customizedData = dt
                    .AsEnumerable()
                    .Select(x => new
                    {
                        LastName = x.Field<string>("LastName"),
                        FirstName = x.Field<string>("FirstName"),
                        Email = x.Field<string>("Email"),
                        Title = x.Field<string>("Title"),
                        SalesRepId = x.Field<string>("SalesRepCode"),
                        Roles = String.Join(", ", UserManager.GetRoles(x.Field<string>("Id")).Select(i => i.ToString()).ToArray()),
                        LastAuthentication = Utils.HandleDatesInJson(x.Field<dynamic>("LastAuth")),
                        IsEnabled = x.Field<bool>("IsEnabled")                        
                    });

                /// Create worksheet
                var ws = pck.Workbook.Worksheets.Add("Reps In Portal");
                ws.Name = "Reps In Portal";

                //Load the datatable and set the number formats...
                ws.Cells["A1"].LoadFromCollection(customizedData, true, TableStyles.Medium9);
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                /// Create stream and send file
                var memoryStream = new MemoryStream();

                pck.SaveAs(memoryStream);
                memoryStream.Position = 0;

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = String.Format("Reps In Portal {0}.xlsx", DateTime.Today.ToString("yyyy-MM-dd")),
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        /// <summary>
        /// Edit Roles
        /// </summary>
        /// <param name="id">User Id as string</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditRoles(string id)
        {
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
            List<IdentityRole> roles = roleManager.Roles.Where(x => x.Name == "salesrep" || x.Name == "repgroupadmin" || x.Name == "repgroupprincipal").ToList();
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
            return RedirectToAction(nameof(RepList));
        }
    }
}