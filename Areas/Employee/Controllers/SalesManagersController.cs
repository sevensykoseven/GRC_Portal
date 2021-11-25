using protean.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using protean.Areas.Employee.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace protean.Areas.Employee.Controllers
{
    [SelectedSidebar("dashboard")]
    [Authorize(Roles = "grcsalesmanager")]
    public class SalesManagersController : Controller
    {
        /// <summary>
        /// Get Bookings Overview
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult Index()
        {
            decimal regionBookingsTotalYTD = 0.0M;
            decimal regionBookingsQuotaYTD = 0.0M;
            decimal regionBookingsTotalMTD = 0.0M;
            decimal regionBookingsQuotaM = 0.0M;
            List<string> regions = new List<string>();

            // Get Rep Group Sales for all regions.  Wish we didn't have to do this but querying the BAQ on the managers 
            // Sales rep Id causes a timeout in Epicor.  Must have something to do with indexing and the view.  Querying the view
            // directly in sql runs for 58 seconds.  Same query on RepGroupId or Region takes a few seconds.  Don't really understand.
            var obj = EAL.Workforce.GetRepGroupSalesForAllRegions();
            JObject o = JObject.Parse(obj.ToString());
            List<int> toRemove = new List<int>();

            if (o.Count > 0)
            {
                for (var i = 0; i < o["value"].Count(); i++)
                {
                    // Create totals for this sales manager, if it is not their region, then remove from list
                    if (o["value"][i]["GRC_SalesReportRegGroup_ManagerRepCode"].ToString() == Current.User.SalesRepCode.ToString())
                    {
                        regionBookingsTotalMTD = regionBookingsTotalMTD + Convert.ToDecimal(o["value"][i]["GRC_SalesReportRegGroup_CurMonthSales"]);
                        regionBookingsQuotaM = regionBookingsQuotaM + Convert.ToDecimal(o["value"][i]["GRC_SalesReportRegGroup_CurMonthQuota"]);

                        regionBookingsTotalYTD = regionBookingsTotalYTD + Convert.ToDecimal(o["value"][i]["GRC_SalesReportRegGroup_CurYTDSales"]);
                        regionBookingsQuotaYTD = regionBookingsQuotaYTD + Convert.ToDecimal(o["value"][i]["GRC_SalesReportRegGroup_CurYTDQuota"]);
                    }
                    else
                    {
                        toRemove.Add(i);
                    }
                }

                // Sort the list in reverse and remove by location.  need to do this when removing through a list where
                // you are removing items at the same time.
                foreach (var i in toRemove.OrderByDescending(x => x))
                {
                    o["value"][i].Remove();
                }

                // Get the region
                if (!regions.Contains(o["value"][0]["GRC_SalesReportRegGroup_Region"].ToString()))
                {
                    regions.Add(o["value"][0]["GRC_SalesReportRegGroup_Region"].ToString());
                }
            }

            var vm = new RegionSalesViewModel();
            vm.ResponseObject = o;
            vm.RegionBookingsTotalMTD = regionBookingsTotalMTD;
            vm.RegionBookingsQuotaM = regionBookingsQuotaM;
            vm.RegionQuotaMPercent = Math.Round(regionBookingsTotalMTD / regionBookingsQuotaM * 100, 1);
            //vm.RegionQuotaMPercent = Math.Round(regionBookingsTotalMTD / (regionBookingsQuotaM / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) * DateTime.Now.Day) * 100, 1);
            vm.RegionBookingsTotalYTD = regionBookingsTotalYTD;
            vm.RegionBookingsQuotaYTD = regionBookingsQuotaYTD;
            vm.RegionQuotaYTDPercent = Math.Round(regionBookingsTotalYTD / regionBookingsQuotaYTD * 100, 1);
            vm.Region = string.Join(",", regions);

            return View(vm);
        }

        /// <summary>
        /// Booking detail about a specific rep group
        /// </summary>
        /// <param name="Id">Rep Group Id</param>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult RepGroupDetails(string Id)
        {
            var obj = EAL.Workforce.GetRepGroupSalesById(Id);
            var vm = new RepGroupDetails();

            if (obj.value.Count > 0)
            {
                vm.RepGroupName = obj.value[0].GRC_SalesReportRegGroup_RepGrp.ToString();
                vm.BookingsTotalMonth = Math.Round(Convert.ToDecimal(obj.value[0].GRC_SalesReportRegGroup_CurMonthSales.ToString()), 2);
                vm.BookingsQuotaMonth = Math.Round(Convert.ToDecimal(obj.value[0].GRC_SalesReportRegGroup_CurMonthQuota.ToString()), 2);
                vm.QuotaMonthPercent = Math.Round(vm.BookingsTotalMonth / vm.BookingsQuotaMonth * 100, 1);
                vm.BookingsTotalYTD = Math.Round(Convert.ToDecimal(obj.value[0].GRC_SalesReportRegGroup_CurYTDSales.ToString()), 2);
                vm.BookingsQuotaYTD = Math.Round(Convert.ToDecimal(obj.value[0].GRC_SalesReportRegGroup_CurYTDQuota.ToString()), 2);
                vm.QuotaYTDPercent = Math.Round(vm.BookingsTotalYTD / vm.BookingsQuotaYTD * 100, 1);
                vm.PriorMonthBookings = Math.Round(Convert.ToDecimal(obj.value[0].GRC_SalesReportRegGroup_PriorMonthSales.ToString()), 2);
                vm.PriorYTDBookings = Math.Round(Convert.ToDecimal(obj.value[0].GRC_SalesReportRegGroup_PriorYTDSales.ToString()), 2);
                vm.PriorYearBookings = Math.Round(Convert.ToDecimal(obj.value[0].GRC_SalesReportRegGroup_PriorYearSales.ToString()), 2);
                vm.PriorYearQuota = Math.Round(Convert.ToDecimal(obj.value[0].GRC_SalesReportRegGroup_PriorYearQuota.ToString()), 2);
                vm.PriorYTDQuota = Math.Round(Convert.ToDecimal(obj.value[0].GRC_SalesReportRegGroup_PriorYTDQuota.ToString()), 2);
                vm.PriorYTDPercent = vm.PriorYTDQuota == 0 ? 0.0M : Math.Round(vm.PriorYTDBookings / vm.PriorYTDQuota * 100, 1);
                vm.PriorYearPercent = vm.PriorYearQuota == 0 ? 0.0M : Math.Round(vm.PriorYearBookings / vm.PriorYearQuota * 100, 1);
            }

            ////////////////////////////////////////////////////////////////////
            // Get Bookings Snapshot
            ////////////////////////////////////////////////////////////////////
            var newObj = EAL.Workforce.GetBookingsByRepGroup(Id, DateTime.Now.AddMonths(-11), DateTime.Now);
            Dictionary<string, decimal> dic = new Dictionary<string, decimal>();
            // Create empty dictionary
            for (var i = DateTime.Now.AddMonths(-11); i <= DateTime.Now; i = i.AddMonths(1))
            {
                dic.Add(i.Year + "/" + i.Month, 0.0M);
            }

            // Replace values in dictionary
            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    dic[item.Calculated_OrderDate.ToString()] = Convert.ToDecimal(item.Calculated_Total);
                }
            }

            var previousYear = EAL.Workforce.GetBookingsByRepGroup(Id, DateTime.Now.AddMonths(-23), DateTime.Now.AddMonths(-12));
            Dictionary<string, decimal> dic3 = new Dictionary<string, decimal>();
            List<string> labels = new List<string>();
            // Create empty dictionary
            for (var i = DateTime.Now.AddMonths(-23); i <= DateTime.Now.AddMonths(-12); i = i.AddMonths(1))
            {
                dic3.Add(i.Year + "/" + i.Month, 0.0M);
                labels.Add(new DateTimeFormatInfo().GetAbbreviatedMonthName(i.Month));
            }

            // Replace values in dictionary
            if (previousYear.value.Count > 0)
            {
                foreach (var item in previousYear.value)
                {
                    dic3[item.Calculated_OrderDate.ToString()] = Convert.ToDecimal(item.Calculated_Total);
                }
            }
            ////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////
            // Get quota contribution
            ////////////////////////////////////////////////////////////////////
            dynamic quotaContribution = EAL.Workforce.GetQuotaContribution(Id, DateTime.Now.Year);
            Dictionary<string, decimal> dic2 = new Dictionary<string, decimal>();
            if (quotaContribution.value.Count > 0)
            {
                foreach (var item in quotaContribution.value)
                {
                    dic2.Add(item.SalesRep_Name.ToString(), Convert.ToDecimal(item.Calculated_OrderLineSales));
                }
            }
            vm.QuoteContributionLabel = JsonConvert.SerializeObject(dic2.Keys).ToString();
            vm.QuoteContributionData = JsonConvert.SerializeObject(dic2.Values).ToString();
            vm.QuotaResponseObject = quotaContribution;
            ////////////////////////////////////////////////////////////////////

            vm.BookingsLabel1 = "Current";
            vm.BookingsLabel2 = "Previous";

            vm.BookingsLabel = JsonConvert.SerializeObject(labels).ToString();
            vm.BookingsData = JsonConvert.SerializeObject(dic.Values).ToString();
            vm.PrevBookingsData = JsonConvert.SerializeObject(dic3.Values).ToString();

            return View(vm);
        }

        public ActionResult Quotes()
        {

            var newObj = EAL.Quotes.GetQuoteOverviewBySalesManager(Current.User.SalesRepCode);

            Dictionary<string, int> dicCount = new Dictionary<string, int>();
            Dictionary<string, decimal> dicSales = new Dictionary<string, decimal>();
            List<string> labels = new List<string>();

            List<string> regions = new List<string>();

            // Create empty dictionary
            for (var i = DateTime.Now.AddMonths(-5); i <= DateTime.Now; i = i.AddMonths(1))
            {
                dicCount.Add(i.Year + "/" + i.Month, 0);
                labels.Add(new DateTimeFormatInfo().GetAbbreviatedMonthName(i.Month));
                dicSales.Add(i.Year + "/" + i.Month, 0.0M);
            }

            // Replace values in dictionary
            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    if (dicCount.ContainsKey(item.Calculated_Year.ToString() + "/" + item.Calculated_Month.ToString()))
                    {
                        dicCount[item.Calculated_Year.ToString() + "/" + item.Calculated_Month.ToString()] += Convert.ToInt32(item.Calculated_Count);
                        dicSales[item.Calculated_Year.ToString() + "/" + item.Calculated_Month.ToString()] += Convert.ToDecimal(item.Calculated_QuoteTotal);

                        if (!regions.Contains(item.Calculated_RegionCode.ToString()))
                        {
                            regions.Add(item.Calculated_RegionCode.ToString());
                        }
                    }
                }
            }

            var vm = new RegionQuoteOverviewViewModel();

            vm.CountData1 = dicCount;
            vm.CountLabel1 = JsonConvert.SerializeObject(labels).ToString();
            vm.SalesData1 = dicSales;
            vm.SalesLabel1 = JsonConvert.SerializeObject(labels).ToString();
            vm.Region = string.Join(",", regions);
            vm.ResponseObject = "";

            return View(vm);
        }
    }
}