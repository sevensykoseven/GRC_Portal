using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using protean.Infrastructure;
using MyUtils;
using protean.Areas.RepPortal.Models;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace protean.Areas.RepPortal.Controllers
{
    [Authorize(Roles = "salesrep")]
    [SelectedSidebar("repportal")]
    public class RepDBController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {

            int openOrderCount = 0;
            decimal openOrderTotal = 0.0M;
            decimal openOrderCommissionTotal = 0.0M;

            // Get open orders for this rep
            dynamic newObj = EAL.Orders.GetRepOpenOrders(Current.User.SalesRepCode);

            openOrderCount = Convert.ToInt32(newObj.value.Count);

            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    openOrderTotal = openOrderTotal + Convert.ToDecimal(item.GRC_RepGroupOpenOrders_OrderAmt);
                }
            }

            // Get sales order commissions
            newObj = EAL.Orders.GetSalesOrderCommissions(Current.User.SalesRepCode);

            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    openOrderCommissionTotal = openOrderCommissionTotal + Convert.ToDecimal(item.GRC_SalesOrderCommRpt_RepCom1);
                }
            }

            // Get commissions for this rep by month
            newObj = EAL.Orders.GetCommisionOverviewForRep(Current.User.SalesRepCode);


            //var key = new List<string>();
            //var data = new List<decimal>();
            //if (newObj.value.Count > 0)
            //{
            //    foreach (var item in newObj.value)
            //    {
            //        key.Add(new DateTimeFormatInfo().GetAbbreviatedMonthName(Convert.ToInt16(item.Calculated_Month)));
            //        data.Add(Convert.ToDecimal(item.Calculated_TotalCommissions));
            //    }
            //}

            // Build the dictonary
            Dictionary<string, decimal> dic = new Dictionary<string, decimal>(); 
            // Create empty dictionary
            for(var i = 1; i <= DateTime.Now.Month; i++)
            {
                dic.Add(new DateTimeFormatInfo().GetAbbreviatedMonthName(i), 0.0M);
            }
            // Replace values in dictionary
            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    dic[new DateTimeFormatInfo().GetAbbreviatedMonthName(Convert.ToInt16(item.Calculated_Month))] = Convert.ToDecimal(item.Calculated_TotalCommissions);                    
                }
            }
            //Jasmine 02-11-2021 start
            //Get order total  by sales rep
            decimal orderTotal = 0.0M;
            newObj = EAL.Orders.GetTotalOrderByRep(Current.User.SalesRepCode);

            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    orderTotal = Convert.ToDecimal(item.Calculated_TotalOrderAmt);
                }
            }
            //Jasmine 02-11-2021 end

            // Populate the model
            var vm = new RepPortalDBViewModel
            {
                CommissionLabel = JsonConvert.SerializeObject(dic.Keys).ToString(),// String.Format("[{0}]", String.Join(", ", key.ToArray())),
                CommissionData = JsonConvert.SerializeObject(dic.Values).ToString(),//String.Format("[{0}]", String.Join(", ", data.ToArray())),
                OpenOrderCount = openOrderCount,
                OpenOrderTotal = openOrderTotal,
                OpenOrderCommissionTotal = openOrderCommissionTotal,
                OrderTotal = orderTotal //Jasmine 02-11-2021
            };

            return View(vm);
            }
    }
}