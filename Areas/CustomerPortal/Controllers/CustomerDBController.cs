using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using protean.Infrastructure;
using EpicorRestAPI;
using MyUtils;
using protean.Areas.CustomerPortal.Models;

namespace protean.Areas.CustomerPortal.Controllers
{
    [SelectedSidebar("customerportal")]
    public class CustomerDBController : Controller
    {
        // GET: CustomerPortal/CustomerDB
        public ActionResult Index()
        {

            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);

            int openOrderCount = 0;
            decimal openOrderTotal = 0.0M;
            using (EpicorSession sess = new EpicorSession())
            {
                // Get the Open Order data
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("$filter", $"GRC_RepGroupOpenOrders_SalesRepCode eq '" + Current.User.SalesRepCode + "'");
                dynamic newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-RepGroupOpenOrders", dic);

                openOrderCount = Convert.ToInt32(newObj.value.Count);

                if (newObj.value.Count > 0)
                {
                    foreach (var item in newObj.value)
                    {
                        openOrderTotal = openOrderTotal + Convert.ToDecimal(item.GRC_RepGroupOpenOrders_OrderAmt);
                    }
                }
            }

            return View(new CustomerPortalDBViewModel { OpenOrderCount = openOrderCount, OpenOrderTotal = openOrderTotal });
        }

        [SelectedSidebar("customerreports")]
        public ActionResult OpenOrders()
        {

            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);

            dynamic newObj;
            int openOrderCount = 0;
            decimal openOrderTotal = 0.0M;
            using (EpicorSession sess = new EpicorSession())
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("$filter", $"GRC_RepGroupOpenOrders_SalesRepCode eq '" + Current.User.SalesRepCode + "'");
                newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-RepGroupOpenOrders", dic);
                openOrderCount = Convert.ToInt32(newObj.value.Count);

                if (newObj.value.Count > 0)
                {
                    foreach( var item in newObj.value)
                    {
                        openOrderTotal = openOrderTotal + Convert.ToDecimal(item.GRC_RepGroupOpenOrders_OrderAmt);
                    }                    
                }
            }

            return View(new CustomerPortalOOViewModel { OpenOrderCount = openOrderCount, ResponseObject = newObj, OpenOrderTotal = openOrderTotal });
        }

        [SelectedSidebar("customerreports")]
        public ActionResult OpenOrdersCommissions()
        {

            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("$filter", $"GRC_SalesOrderCommRpt_SalesRepCode1 eq '" + Current.User.SalesRepCode + "'");
            dynamic newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-SalesOrderCommRpt", dic);

            return View(newObj);
        }
    }
}
