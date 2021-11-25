using System;
using System.Collections.Generic;
using EpicorRestAPI;
using MyUtils;
using Newtonsoft.Json;
using protean.Infrastructure;

namespace protean.EAL
{
    public class Workforce
    {
        /// <summary>
        /// Get sales rep information
        /// </summary>
        /// <param name="salesRepCode"></param>
        /// <returns>dynamic json object</returns>
        public static dynamic GetWorkforceBySalesRepCode(string salesRepCode)
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);
            dynamic newObj;

            using (EpicorSession sess = new EpicorSession())
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                newObj = EpicorRest.DynamicGet("BaqSvc", "WEB-Workforce/?WorkForceID=" + salesRepCode, dic);
            }

            return newObj;
        }

        /// <summary>
        /// Get quote met by reg group
        /// </summary>
        /// <param name="repGroupId">Rep group Id</param>
        /// <returns>dynamic json object</returns>
        public static dynamic GetQuoteMetByRepGroup(string repGroupId)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("QuoteMetByRepGroup");
                return JsonConvert.DeserializeObject<dynamic>(obj.Value);
            }
            else
            {
                EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
                EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
                EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
                EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
                EpicorRest.IgnoreCertErrors = true;
                EpicorRest.License = EpicorLicenseType.Default;
                EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);
                dynamic newObj;

                using (EpicorSession sess = new EpicorSession())
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    newObj = EpicorRest.DynamicGet("BaqSvc", "WEB-QuotaPercentageMet/?RepGrpIDParam=" + repGroupId, dic);
                }

                return newObj;
            }
        }

        /// <summary>
        /// Get sales overview for the given year for rep group
        /// </summary>
        /// <param name="repGroupId">Rep group Id</param>
        /// <param name="year">Year as an integer</param>
        /// <returns>dynamic json object</returns>
        public static dynamic GetBookingsByRepGroup(string repGroupId, DateTime startDate, DateTime endDate)
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);
            dynamic newObj;

            using (EpicorSession sess = new EpicorSession())
            {
                // Old method before 2021
                //Dictionary<string, string> dic = new Dictionary<string, string>();
                //newObj = EpicorRest.DynamicGet("BaqSvc", "WEB-RepGroupSalesOverview/?OrderEntryYear=" + year.ToString() + "&RepGroupParam=" + repGroupId, dic);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                newObj = EpicorRest.DynamicGet("BaqSvc", "WEB-RepGroupSalesSummary/?StartDate=" + startDate.ToString() + "&EndDate=" + endDate.ToString() + "&RepGrpID=" + repGroupId, dic);
            }

            return newObj;
        }

        /// <summary>
        /// Get quota contribution
        /// </summary>
        /// <param name="repGroupId">Rep group Id</param>
        /// <param name="year">Year as an Integer</param>
        /// <returns>dynamic json object</returns>
        public static dynamic GetQuotaContribution(string repGroupId, int year)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("QuotaContribution");
                return JsonConvert.DeserializeObject<dynamic>(obj.Value);
            }
            else
            {
                EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
                EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
                EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
                EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
                EpicorRest.IgnoreCertErrors = true;
                EpicorRest.License = EpicorLicenseType.Default;
                EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);
                dynamic newObj;

                using (EpicorSession sess = new EpicorSession())
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    newObj = EpicorRest.DynamicGet("BaqSvc", "WEB-QuotaContribution/?OrderEntryYear=" + year.ToString() + "&RepGroupParam=" + repGroupId, dic);
                }

                return newObj;
            }
        }

        ///// <summary>
        ///// Get projects by rep group  -- REMOVED SECTION 2020-12-17  (COMMUNICATES NO USEFUL INFORMATION)
        ///// </summary>
        ///// <param name="repGroupId">Rep group Id</param>
        ///// <param name="days"></param>
        ///// <returns>Dynamic json object</returns>
        //public static dynamic GetProjectsByRepGroup(string repGroupId, int days)
        //{
        //    EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
        //    EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
        //    EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
        //    EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
        //    EpicorRest.IgnoreCertErrors = true;
        //    EpicorRest.License = EpicorLicenseType.Default;
        //    EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);
        //    dynamic newObj;

        //    using (EpicorSession sess = new EpicorSession())
        //    {
        //        Dictionary<string, string> dic = new Dictionary<string, string>();
        //        newObj = EpicorRest.DynamicGet("BaqSvc", "WEB-Projects/?NumberOfDays=" + days.ToString() + "&RepGroupCode=" + repGroupId, dic);
        //    }

        //    return newObj;
        //}

        /// <summary>
        /// Check to see if Rep is inactive
        /// </summary>
        /// <param name="salesRepCode">Sales Rep Code</param>
        /// <returns>boolean</returns>
        public static bool IsRepInActive(string salesRepCode)
        {
            dynamic obj = GetWorkforceBySalesRepCode(salesRepCode);

            if (obj.value.Count > 0)
            {
                return Convert.ToBoolean(obj.value[0].SalesRep_Inactive);
            }

            return true;
        }

        /// <summary>
        /// Get all work force.  Get only those that are active.
        /// </summary>
        /// <returns>dynamic json object</returns>
        public static dynamic GetAllWorkforce()
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);
            dynamic newObj;

            using (EpicorSession sess = new EpicorSession())
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("$filter", $"SalesRep_InActive eq false");
                newObj = EpicorRest.DynamicGet("BaqSvc", "WEB-AllWorkforce", dic);
            }

            return newObj;
        }

        public static dynamic GetRepGroupSalesForAllRegions()
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);
            dynamic newObj;

            using (EpicorSession sess = new EpicorSession())
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                //dic.Add("$filter", $"GRC_SalesReportRegGroup_RegionCode eq '" + region + "'");
                newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-SalesRptRepGroup", dic);
            }

            return newObj;
        }

        public static dynamic GetRepGroupSalesByRegion(string region)
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);
            dynamic newObj;

            using (EpicorSession sess = new EpicorSession())
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("$filter", $"GRC_SalesReportRegGroup_RegionCode eq '" + region + "'");
                newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-SalesRptRepGroup", dic);
            }

            return newObj;
        }

        public static dynamic GetRepGroupSalesById(string repGroupId)
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);
            dynamic newObj;

            using (EpicorSession sess = new EpicorSession())
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("$filter", $"GRC_SalesReportRegGroup_RepGrpID eq '" + repGroupId + "'") ;
                newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-SalesRptRepGroup", dic);
            }

            return newObj;
        }

        public static dynamic GetAllActiveRegions()
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);
            dynamic newObj;

            using (EpicorSession sess = new EpicorSession())
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("$filter", $"Inactive eq false");
                newObj = EpicorRest.DynamicGet("Erp.BO.RegionSvc", "Regions", dic);
            }

            return newObj;
        }

    }
}