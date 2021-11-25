using protean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace protean
{
    public static class Database
    {
        private const string ContextKey = "protean.Database.DbContext";

        /// <summary>
        /// Get DbContext
        /// </summary>
        public static ApplicationDbContext ActiveContext
        {
            get
            {
                return (HttpContext.Current.Items[ContextKey] as ApplicationDbContext);
            }
        }

        /// <summary>
        /// Open DbContext and store it in the items collection
        /// </summary>
        public static void OpenSession()
        {
            HttpContext.Current.Items[ContextKey] = new ApplicationDbContext();
        }

        /// <summary>
        /// Find DbContext in items collection and dispose it.
        /// </summary>
        public static void CloseSession()
        {
            var dbContext = HttpContext.Current.Items[ContextKey] as ApplicationDbContext;
            if (dbContext != null)
            {
                dbContext.Dispose();
            }
        }
    }
}