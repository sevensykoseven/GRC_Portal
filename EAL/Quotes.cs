using System.Collections.Generic;
using EpicorRestAPI;
using MyUtils;
using System;
using Newtonsoft.Json;

namespace protean.EAL
{
    public class Quotes
    {
        /// <summary>
        /// Get rep open quotes
        /// </summary>
        /// <param name="salesRepCode">Sales Rep Code as a String</param>
        /// <returns>dynamic json object as result</returns>
        public static dynamic GetRepOpenQuotes(string salesRepCode)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("RepOpenQuotes");
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
                    // Right now we get all open quotes in this query so we filter by rep code and expired.
                    dic.Add("$filter", $"GRC_OpenQuotesActiveReps_ExpirationDate gt DateTime'" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "' and GRC_OpenQuotesActiveReps_PrimRepCode eq '" + salesRepCode + "'");
                    //GRC_OpenQuotesActiveReps_ExpirationDate gt DateTime'2020-12-09T00:00:00' and GRC_OpenQuotesActiveReps_PrimRepCode eq 'MID'
                    newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-OpenQuotesTicket9520", dic);
                }

                return newObj;
            }
        }

        /// <summary>
        /// Get rep group open quotes
        /// </summary>
        /// <param name="repGroupId">Rep Group Id as a String</param>
        /// <returns>dynamic json object as result</returns>
        public static dynamic GetRepGroupOpenQuotes(string repGroupId)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("RepGroupOpenQuotes");
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
                    // Right now we get all open quotes in this query so we filter by rep code and expired.
                    dic.Add("$filter", $"GRC_OpenQuotesActiveReps_ExpirationDate gt DateTime'" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "' and GRC_OpenQuotesActiveReps_CodeID eq '" + repGroupId + "'");
                    newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-OpenQuotesTicket9520", dic);
                }

                return newObj;
            }
        }

        public static dynamic GetQuoteOverviewBySalesManager(string managerId)
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
                dic.Add("$filter", $"Calculated_ManagerRepCode eq '" + managerId + "'");
                newObj = EpicorRest.DynamicGet("BaqSvc", "WEB-QuoteOverview", dic);
            }

            return newObj;
        }

        public static dynamic GetQuoteOverviewByRepGroupId(string repGroupId)
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
                dic.Add("$filter", $"Calculated_PrimRepCode eq '" + repGroupId + "'");
                newObj = EpicorRest.DynamicGet("BaqSvc", "WEB-QuoteOverview", dic);
            }

            return newObj;
        }

        //Jasmine 15-11-2021 start

        /// <summary>
        /// Get order details by order num
        /// </summary>
        /// <param name="orderNum">Order number (Id) as integer</param>
        /// <returns>dynamic json object as result</returns>
        public static dynamic GetQuoteDetails(int orderNum)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("QuoteDetails");
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
                    newObj = EpicorRest.DynamicGet("BaqSvc", "IIT_GRC_QuoteDtls/?QuoteNum=" + orderNum, dic);
                }

                return newObj;
            }
        }
        //Jasmine 15-11-2021 end

        //Jasmine 21-11-2021 start
        public static dynamic GetQuotationSvc(int quoteNum, string salesRepCode)
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
                //dic.Add("$filter", $"QuoteNum eq " + quoteNum);
                newObj = EpicorRest.DynamicGet("BaqSvc", "IIT_GRC_QuoteSalesRep/?QuoteNum=" + quoteNum+ "&SalesRep="+ salesRepCode, dic);
            }

            return newObj;
        }
        //Jasmine 21-11-2021 end
    }
}