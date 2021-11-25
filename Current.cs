using protean.Models;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace protean
{
    public static class Current
    {

        #region Private Members

        private const string UserKey = "protean.Current.UserKey";

        #endregion Private Members

        #region Public Methods

        /// <summary>
        /// Cached current user
        /// We do this so we don't have to call frequently for commonly referenced properties during one webrequest
        /// </summary>
        public static ApplicationUser User
        {
            get
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return null;
                }
                               
                var user = HttpContext.Current.Items[UserKey] as ApplicationUser;

                if (user == null)
                {
                    //ApplicationDbContext context = new ApplicationDbContext();
                    //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));
                    user = userManager.FindById(HttpContext.Current.User.Identity.GetUserId());

                    if (user != null)
                    {
                        HttpContext.Current.Items[UserKey] = user;
                    }
                }

                return user;
            }
        }

        /// <summary>
        /// Clear cached data
        /// </summary>
        public static void Clear()
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return;
            }

            HttpContext.Current.Items[UserKey] = null;
        }

        #endregion Public Methods

    }
}