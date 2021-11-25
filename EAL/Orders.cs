using System.Collections.Generic;
using EpicorRestAPI;
using MyUtils;
using System;
using protean.Infrastructure;
using protean.Models;
using Newtonsoft.Json;

namespace protean.EAL
{
    public class Orders
    {
        /// <summary>
        /// Get open orders for rep by sales rep code
        /// </summary>
        /// <param name="SalesRepCode">Sales rep code as string</param>
        /// <returns>dynamic json object as result</returns>
        public static dynamic GetRepOpenOrders(string salesRepCode)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("RepOpenOrders");
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
                    dic.Add("$filter", $"GRC_RepGroupOpenOrders_SalesRepCode eq '" + salesRepCode + "'");
                    newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-RepGroupOpenOrders", dic);
                }

                return newObj;
            }
        }

        /// <summary>
        /// Get order details by order num
        /// </summary>
        /// <param name="orderNum">Order number (Id) as integer</param>
        /// <returns>dynamic json object as result</returns>
        public static dynamic GetOrderDetails(int orderNum)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("OrderDetails");
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
                    newObj = EpicorRest.DynamicGet("BaqSvc", "WEB-OrderDtls/?OrderNumParam=" + orderNum, dic);
                }

                return newObj;
            }
        }

        /// <summary>
        /// Return Sales order commissions for a sales rep
        /// </summary>
        /// <param name="salesRepCode">Sales rep code as a string</param>
        /// <returns>dynamic json object as result</returns>
        public static dynamic GetSalesOrderCommissions(string salesRepCode)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("SalesOrderCommissions");
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
                    dic.Add("$filter", $"GRC_SalesOrderCommRpt_SalesRepCode1 eq '" + salesRepCode + "'");
                    newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-SalesOrderCommRpt", dic);
                }

                return newObj;
            }
        }

        /// <summary>
        /// Get order history for rep by year
        /// </summary>
        /// <param name="salesRepCode">Sales rep code as string</param>
        /// <param name="year">Year as integer</param>
        /// <returns>dynamic json object</returns>
        public static dynamic GetOrderHistoryBySalesRep(string salesRepCode, int year)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("OrderHistoryBySalesRep");
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
                    newObj = EpicorRest.DynamicGet("BaqSvc", "web-OrderHistorySalesRepCode/?SalesRepCodeParam=" + salesRepCode + "&Year=" + year, dic);
                }

                return newObj;
            }
        }

        /// <summary>
        /// Get open orders for entire rep group
        /// </summary>
        /// <param name="repGroupId">Rep group Id as a String</param>
        /// <returns>dynamic json object as result</returns>
        public static dynamic GetRepGroupOpenOrders(string repGroupId)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("RepGroupOpenOrders");
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
                    dic.Add("$filter", $"GRC_RepGroupOpenOrders_RepGroupID eq '" + repGroupId + "'");
                    newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-RepGroupOpenOrders", dic);
                }
                return newObj;
            }
        }

        /// <summary>
        /// Return Sales order commissions for a sales rep
        /// </summary>
        /// <param name="salesRepCode">Sales rep code as a string</param>
        /// <returns>dynamic json object as result</returns>
        public static dynamic GetRepGroupSalesOrderCommissions(string repGroupId)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("RepGroupSalesOrderCommissions");
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
                    dic.Add("$filter", $"GRC_SalesOrderCommRpt_CodeID1 eq '" + repGroupId + "'");
                    newObj = EpicorRest.DynamicGet("BaqSvc", "GRC-SalesOrderCommRpt", dic);
                }

                return newObj;
            }
        }

        /// <summary>
        /// Get order history for rep group by year
        /// </summary>
        /// <param name="salesRepCode">Sales rep code as string</param>
        /// <param name="year">Year as integer</param>
        /// <returns>dynamic json object</returns>
        public static dynamic GetOrderHistoryByRepGroup(string repGroupId, int year)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("PopulateTestObjects")))
            {
                var obj = Database.ActiveContext.TestData.Find("OrderHistoryByRepGroup");
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
                    newObj = EpicorRest.DynamicGet("BaqSvc", "web-OrderHistoryRepGroup/?RepGroupParam=" + repGroupId + "&Year=" + year, dic);
                }

                return newObj;
            }
        }

        /// <summary>
        /// Get tracking number for order
        /// </summary>
        /// <param name="orderNum">Order Num as a String</param>
        /// <returns>dynamic Json object</returns>
        public static dynamic GetTrackingNumbers(int orderNum)
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
                newObj = EpicorRest.DynamicGet("BaqSvc", "web-OrderTrackingNumbers/?OrderNumParam=" + orderNum.ToString(), dic);
            }

            return newObj;
        }

        ///Jasmine 26-10-2021
        /// <summary>
        /// Get ship details for order
        /// </summary>
        /// <param name="orderNum">Order Num as a String</param>
        /// <returns>dynamic Json object</returns>
        public static dynamic GetShipDetails(int orderNum)
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
                newObj = EpicorRest.DynamicGet("BaqSvc", "IIT_GRC_ShipDetails/?OrderNum=" + orderNum.ToString(), dic);
            }

            return newObj;
        }

        ///Jasmine 02-11-2021
        /// <summary>
        /// Get ship details for order
        /// </summary>
        /// <param name="SalesRep">Sales rep as a String</param>
        /// <returns>dynamic Json object</returns>
        public static dynamic GetTotalOrderByRep(string salesRepId)
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
                newObj = EpicorRest.DynamicGet("BaqSvc", "IIT_GRC_TotalOrderByRep/?SalesRep=" + salesRepId.ToString(), dic);
            }

            return newObj;
        }

        /// <summary>
        /// Get commission overview report for the rep.  Amounts are grouped together by month.
        /// </summary>
        /// <param name="salesRepId">Sales rep Id</param>
        /// <returns>Dynamic json object.</returns>
        public static dynamic GetCommisionOverviewForRep(string salesRepId)
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
                newObj = EpicorRest.DynamicGet("BaqSvc", "web-CommOverview/?SalesRepCode=" + salesRepId.ToString(), dic);
            }

            return newObj;
        }

        /// <summary>
        /// Get orders shipped for rep group
        /// </summary>
        /// <param name="repGroupId">Rep group Id</param>
        /// <param name="year">year</param>
        /// <returns></returns>
        public static dynamic GetOrdersShippedForRepGroup(string repGroupId, int year)
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
                newObj = EpicorRest.DynamicGet("BaqSvc", "WEB-OrdersShipped/?RepGroupCodeParam=" + repGroupId + "&YearShipDateParam=" + year.ToString(), dic);
            }

            return newObj;
        }

        public static dynamic GetSalesOrderSvc(int orderNum)
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
                dic.Add("$filter", $"OrderNum eq " + orderNum);
                newObj = EpicorRest.DynamicGet("Erp.BO.SalesOrderSvc", "SalesOrders", dic);
            }

            return newObj;
        }
    }
}