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
    [Authorize(Roles = "grcexecutive")]
    public class ExecutiveController : Controller
    {
        /// <summary>
        /// Booking overview for all regions
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            decimal AllRegionBookingsTotalYTD = 0.0M;
            decimal AllRegionBookingsQuotaYTD = 0.0M;
            decimal AllRegionBookingsTotalMTD = 0.0M;
            decimal AllRegionBookingsQuotaM = 0.0M;

            // Get Rep Group Sales for all regions.  Wish we didn't have to do this but querying the BAQ on the managers 
            // Sales rep Id causes a timeout in Epicor.  Must have something to do with indexing and the view.  Querying the view
            // directly in sql runs for 58 seconds.  Same query on RepGroupId or Region takes a few seconds.  Don't really understand.
            var obj = EAL.Workforce.GetRepGroupSalesForAllRegions();
            JObject o = JObject.Parse(obj.ToString());

            // Region totals
            Dictionary<string, decimal> regionYTDBookings = new Dictionary<string, decimal>();
            Dictionary<string, decimal> regionYTDQuota = new Dictionary<string, decimal>();
            var regionsObj = EAL.Workforce.GetAllActiveRegions();

            if (regionsObj.value.Count > 0)
            {
                foreach (var item in regionsObj.value)
                {
                    regionYTDBookings.Add(item.Description.ToString(), 0.0M);
                    regionYTDQuota.Add(item.Description.ToString(), 0.0M);
                }
            }

            if (o.Count > 0)
            {
                for (var i = 0; i < o["value"].Count(); i++)
                {
                    AllRegionBookingsTotalMTD = AllRegionBookingsTotalMTD + Convert.ToDecimal(o["value"][i]["GRC_SalesReportRegGroup_CurMonthSales"]);
                    AllRegionBookingsQuotaM = AllRegionBookingsQuotaM + Convert.ToDecimal(o["value"][i]["GRC_SalesReportRegGroup_CurMonthQuota"]);

                    AllRegionBookingsTotalYTD = AllRegionBookingsTotalYTD + Convert.ToDecimal(o["value"][i]["GRC_SalesReportRegGroup_CurYTDSales"]);
                    AllRegionBookingsQuotaYTD = AllRegionBookingsQuotaYTD + Convert.ToDecimal(o["value"][i]["GRC_SalesReportRegGroup_CurYTDQuota"]);

                    // sum region totals and quotas
                    if (regionYTDBookings.ContainsKey(o["value"][i]["GRC_SalesReportRegGroup_Region"].ToString()))
                    {
                        var region = o["value"][i]["GRC_SalesReportRegGroup_Region"].ToString();
                        regionYTDBookings[region] = regionYTDBookings[region] + Convert.ToDecimal(o["value"][i]["GRC_SalesReportRegGroup_CurYTDSales"]);
                        regionYTDQuota[region] = regionYTDQuota[region] + Convert.ToDecimal(o["value"][i]["GRC_SalesReportRegGroup_CurYTDQuota"]);
                    }
                }
            }

            Dictionary<string, decimal> regionPercents = new Dictionary<string, decimal>();
            foreach (var item in regionYTDQuota)
            {
                regionPercents.Add(item.Key, item.Value == 0 ? 0.0M : Math.Round(regionYTDBookings[item.Key] / item.Value * 100, 1));
            }            

            var vm = new RegionSalesExecViewModel();
            vm.ResponseObject = o;
            vm.RegionBookingsTotalMTD = AllRegionBookingsTotalMTD;
            vm.RegionBookingsQuotaM = AllRegionBookingsQuotaM;
            vm.RegionQuotaMPercent = AllRegionBookingsQuotaM == 0 ? 0.0M : Math.Round(AllRegionBookingsTotalMTD / AllRegionBookingsQuotaM * 100, 1);
            //vm.RegionQuotaMPercent = Math.Round(regionBookingsTotalMTD / (regionBookingsQuotaM / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) * DateTime.Now.Day) * 100, 1);
            vm.RegionBookingsTotalYTD = AllRegionBookingsTotalYTD;
            vm.RegionBookingsQuotaYTD = AllRegionBookingsQuotaYTD;
            vm.RegionQuotaYTDPercent = AllRegionBookingsQuotaYTD == 0 ? 0.0M : Math.Round(AllRegionBookingsTotalYTD / AllRegionBookingsQuotaYTD * 100, 1);
            vm.Region = "All"; //string.Join(",", regions);
            vm.RegionPercents = regionPercents.Where(x => x.Value > 0).ToDictionary(i => i.Key, i => i.Value);
            vm.Contributions = regionYTDBookings.Where(x => x.Value > 0).ToDictionary(i => i.Key, i => i.Value);
            vm.regionYTDBookings = regionYTDBookings;
            vm.regionYTDQuota = regionYTDQuota;

            return View(vm);
        }

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
    }
}