using System;
using System.Web.Mvc;

namespace protean.Infrastructure
{
    /// <summary>
    /// Class created to keep track of what sidebar link should be active / selected / or open when a page/method is used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class SelectedSidebarAttribute : ActionFilterAttribute
    {
        private readonly string _selectedLink;

        /// <summary>
        /// Property for the selected link
        /// </summary>
        /// <param name="selectedLink">Link as a String</param>
        public SelectedSidebarAttribute(string selectedLink)
        {
            _selectedLink = selectedLink;
        }

        #region Methods

        /// <summary>
        /// OnResultingExecuting method
        /// </summary>
        /// <param name="filterContext">ResultExecutingContext</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.SelectedLink = _selectedLink;
        }

        #endregion
    }
}