using protean.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using protean.Areas.RepGroupPortal.Models;
using System.Globalization;
using Newtonsoft.Json;

namespace protean.Areas.RepGroupPortal.Controllers
{
    [Authorize(Roles = "repgroupadmin")]
    [SelectedSidebar("repgroupportal")]
    public class RepGroupDBController : Controller
    {
        /// <summary>
        /// Dashboard for rep group
        /// </summary>
        /// <returns>View</returns>        
        [HttpGet]
        public ActionResult Index()
        {
            int openOrderCount = 0;
            decimal openOrderTotal = 0.0M;
            decimal openOrderCommissionTotal = 0.0M;

            //Get rep group open orders
            dynamic newObj = EAL.Orders.GetRepGroupOpenOrders(Current.User.SalesRep.RepGroupId);

            openOrderCount = Convert.ToInt32(newObj.value.Count);

            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    openOrderTotal = openOrderTotal + Convert.ToDecimal(item.GRC_RepGroupOpenOrders_OrderAmt);
                }
            }

            // Get rep group sales order commissions
            newObj = EAL.Orders.GetRepGroupSalesOrderCommissions(Current.User.SalesRep.RepGroupId);

            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    openOrderCommissionTotal = openOrderCommissionTotal + Convert.ToDecimal(item.GRC_SalesOrderCommRpt_RepCom1);
                }
            }

            // Get Quota
            var quota = 0.0M;
            newObj = EAL.Workforce.GetQuoteMetByRepGroup(Current.User.SalesRep.RepGroupId);
            if (newObj.value.Count > 0)
            {
                quota = Math.Round(Convert.ToDecimal(newObj.value[0].Calculated_YTDQuotaPercentageMet), 1);
            }

            ////////////////////////////////////////////////////////////////////
            // Get Bookings Snapshot
            ////////////////////////////////////////////////////////////////////
            newObj = EAL.Workforce.GetBookingsByRepGroup(Current.User.SalesRep.RepGroupId, DateTime.Now.AddMonths(-11), DateTime.Now);
            
            // Build the lists and convert them to json when adding to model.
            //var key = new List<string>();
            //var data = new List<decimal>();
            //if (newObj.value.Count > 0)
            //{
            //    foreach (var item in newObj.value)
            //    {
            //        key.Add(new DateTimeFormatInfo().GetAbbreviatedMonthName(Convert.ToInt16(item.Calculated_MonthOrderDate)));
            //        data.Add(Convert.ToDecimal(item.Calculated_OrderLineSales));
            //    }
            //}
            Dictionary<string, decimal> dic = new Dictionary<string, decimal>();
            // Create empty dictionary
            for (var i = DateTime.Now.AddMonths(-11); i <= DateTime.Now; i = i.AddMonths(1))
            {
                //dic.Add(new DateTimeFormatInfo().GetAbbreviatedMonthName(i), 0.0M);
                //dic.Add(new DateTimeFormatInfo().GetMonthName(i), 0.0M);
                dic.Add(i.Year + "/" + i.Month, 0.0M);
            }
            
            // Replace values in dictionary
            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    //dic[new DateTimeFormatInfo().GetAbbreviatedMonthName(Convert.ToInt16(item.Calculated_MonthOrderDate))] = Convert.ToDecimal(item.Calculated_OrderLineSales);
                    dic[item.Calculated_OrderDate.ToString()] = Convert.ToDecimal(item.Calculated_Total);
                }
            }
            ////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////
            // Get quota contribution
            ////////////////////////////////////////////////////////////////////
            dynamic quotaContribution = EAL.Workforce.GetQuotaContribution(Current.User.SalesRep.RepGroupId, DateTime.Now.Year);
            Dictionary<string, decimal> dic2 = new Dictionary<string, decimal>();
            if (quotaContribution.value.Count > 0)
            {
                foreach (var item in quotaContribution.value)
                {
                    dic2.Add(item.SalesRep_Name.ToString(), Convert.ToDecimal(item.Calculated_OrderLineSales));
                }
            }
            ////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////
            // Get Shipping history
            ////////////////////////////////////////////////////////////////////
            dynamic shippingHistory = EAL.Orders.GetOrdersShippedForRepGroup(Current.User.SalesRep.RepGroupId, DateTime.Now.Year);
            Dictionary<string, int> dic3 = new Dictionary<string, int>();
            if (shippingHistory.value.Count > 0)
            {
                foreach (var item in shippingHistory.value)
                {
                    dic3.Add(new DateTimeFormatInfo().GetAbbreviatedMonthName(Convert.ToInt16(item.FiscalPer_FiscalPeriod)), Convert.ToInt32(item.Calculated_CountOfOrdersShipped));
                }
            }
            ////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////
            //// Get projects  -- REMOVED SECTION 2020-12-17  (COMMUNICATES NO USEFUL INFORMATION)
            //////////////////////////////////////////////////////////////////////
            //dynamic projectsObj = EAL.Workforce.GetProjectsByRepGroup(Current.User.SalesRep.RepGroupId, 7);
            //////////////////////////////////////////////////////////////////////

            var vm = new RepGroupPortalDBViewModel
            {
                ShippedLabel = JsonConvert.SerializeObject(dic3.Keys).ToString(),
                ShippedData = JsonConvert.SerializeObject(dic3.Values).ToString(),
                QuoteContributionLabel = JsonConvert.SerializeObject(dic2.Keys).ToString(),
                QuoteContributionData = JsonConvert.SerializeObject(dic2.Values).ToString(),
                BookingsLabel = JsonConvert.SerializeObject(dic.Keys).ToString(),
                BookingsData = JsonConvert.SerializeObject(dic.Values).ToString(),
                OpenOrderCount = openOrderCount,
                OpenOrderTotal = openOrderTotal,
                OpenOrderCommissionTotal = openOrderCommissionTotal,
                YearQuota = quota,
                QuotaResponseObject = quotaContribution
                //ProjectResponseObject = projectsObj  -- REMOVED SECTION 2020-12-17  (COMMUNICATES NO USEFUL INFORMATION)
            };


            return View(vm);
        }
    }
}