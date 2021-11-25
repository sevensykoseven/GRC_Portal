using System.Collections.Generic;
using EpicorRestAPI;
using MyUtils;
using System;
using System.Net;
using System.Text;
using System.Windows.Interop;
using protean.Infrastructure;

namespace protean.EAL
{
    public class RPT
    {
        /// <summary>
        /// Send order acknowledgements by order num and sales rep code.
        /// Keep in mind this currently only works for open orders
        /// </summary>
        /// <param name="id">Order Num</param>
        /// <param name="email">Email address</param>
        /// <param name="salesRepCode">Sales Rep Code</param>
        /// <returns>HttpStatusCode</returns>
        public static HttpStatusCode SendSalesOrderAckBySalesRep(int id, string email, string salesRepCode)
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);

            CallContextHeader callContext = new CallContextHeader();
            EpicorRestStatusCode statusCode = new EpicorRestStatusCode();

            // Build mail body
            var sb = new StringBuilder();
            sb.AppendLine("Hello and thank you for your order!");
            sb.AppendLine();
            sb.AppendLine("Attached you will find your order acknowledgement.");
            sb.AppendLine();
            sb.AppendLine("As always we appreciate your business!");
            sb.AppendLine();
            sb.AppendLine(String.Format("Should you have any concerns, please feel free to reach out to " +
                "us at {0} and ask for customer service.", EnvironmentSettings.AppSettings("PhoneNumber")));

            using (EpicorSession sess = new EpicorSession())
            {
                var subject = "";

                // Get order details
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("$filter", $"GRC_RepGroupOpenOrders_SalesRepCode eq '" + salesRepCode + "'");
                dynamic orderObj = EpicorRest.DynamicGet("BaqSvc", "GRC-RepGroupOpenOrders", dic);
                dic.Clear();

                if (orderObj.value.Count > 0)
                {
                    foreach (var item in orderObj.value)
                    {
                        if (item.GRC_RepGroupOpenOrders_Ordernum == id)
                        {
                            subject = String.Format("GRCC Order Acknowledgement: PO#: {0} / SO#: {1}", item.GRC_RepGroupOpenOrders_PONum, id);
                        }
                    }
                }
                else
                {
                    // if nothing was found then we need to return nothing so junk is not sent to the task agent
                    return HttpStatusCode.NotFound;
                }

                // Get Dynamic object to modify
                dynamic obj = EpicorRest.DynamicPost("Erp.RPT.SalesOrderAckSvc", "GetNewParameters", dic); ;

                // Populate parameter values
                foreach (var item in obj.ds.SalesOrderAckParam)
                {
                    item.OrderNum = id;
                    item.SysRowID = Guid.Parse("00000000-0000-0000-0000-000000000000");
                    item.AutoAction = "SSRSPrint";
                    item.AgentSchedNum = 0;
                    item.AgentID = "SystemTaskAgent";
                    item.AgentTaskNum = 0;
                    item.RecurringTask = false;
                    item.RptPageSettings = "";
                    item.RptPrinterSettings = "";
                    item.RptVersion = "";
                    item.ReportStyleNum = 1005;
                    item.WorkstationID = "";
                    item.TaskNote = "";
                    item.ArchiveCode = 0;
                    item.DateFormat = "m/d/yyyy";
                    item.NumericFormat = ",.";
                    item.AgentCompareString = "";
                    item.ProcessID = "";
                    item.ProcessTaskNum = 0;
                    item.DecimalsGeneral = 0;
                    item.DecimalsCost = 0;
                    item.DecimalsPrice = 0;
                    item.GlbDecimalsGeneral = 0;
                    item.GlbDecimalsCost = 0;
                    item.GlbDecimalsPrice = 0;
                    item.FaxSubject = subject;
                    item.FaxTo = "";
                    item.FaxNumber = "";
                    item.EMailTo = email;
                    item.EMailBody = sb.ToString();

                    item.AttachmentType = "PDF";
                    item.ReportCurrencyCode = "USD";
                    item.ReportCultureCode = "en-US";
                    item.SSRSRenderFormat = "PDF";
                    item.SSRSEnableRouting = true;
                    item.RowMod = "A";
                }

                // Make request to send report
                EpicorRest.DynamicPost("Erp.RPT.SalesOrderAckSvc", "RunDirect", obj, true, statusCode,callContext);
            }

            return statusCode.RestCallStatusCode;
        }

        /// <summary>
        /// Send order acknowledgements by order num and sales rep code.
        /// </summary>
        /// <param name="id">Order Num</param>
        /// <param name="email">Email address</param>
        /// <param name="salesRepCode">Sales Rep Code</param>
        /// <returns>HttpStatusCode</returns>
        public static HttpStatusCode SendSalesOrderAckByOrderNum(int id, string email, string salesRepCode)
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);

            CallContextHeader callContext = new CallContextHeader();
            EpicorRestStatusCode statusCode = new EpicorRestStatusCode();

            // Build mail body
            var sb = new StringBuilder();
            sb.AppendLine("Hello and thank you for your order!");
            sb.AppendLine();
            sb.AppendLine("Attached you will find your order acknowledgement.");
            sb.AppendLine();
            sb.AppendLine("As always we appreciate your business!");
            sb.AppendLine();
            sb.AppendLine(String.Format("Should you have any concerns, please feel free to reach out to " +
                "us at {0} and ask for customer service.", EnvironmentSettings.AppSettings("PhoneNumber")));

            using (EpicorSession sess = new EpicorSession())
            {
                var subject = "";

                // Get order details
                dynamic orderObj = EAL.Orders.GetSalesOrderSvc(id);

                // Check to see if this order can be requested by this sales rep id.  If not, give unauthorized message.
                var dtl = orderObj.value[0];
                if (dtl.OrderNum == id && dtl.SalesRepList.ToString().Contains(salesRepCode))
                {
                    subject = String.Format("GRCC Order Acknowledgement: PO#: {0} / SO#: {1}", dtl.PONum, id);
                }
                else
                {
                    return HttpStatusCode.Unauthorized;
                }

                // Get Dynamic object to modify
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dynamic obj = EpicorRest.DynamicPost("Erp.RPT.SalesOrderAckSvc", "GetNewParameters", dic); ;

                // Populate parameter values
                foreach (var item in obj.ds.SalesOrderAckParam)
                {
                    item.OrderNum = id;
                    item.SysRowID = Guid.Parse("00000000-0000-0000-0000-000000000000");
                    item.AutoAction = "SSRSPrint";
                    item.AgentSchedNum = 0;
                    item.AgentID = "SystemTaskAgent";
                    item.AgentTaskNum = 0;
                    item.RecurringTask = false;
                    item.RptPageSettings = "";
                    item.RptPrinterSettings = "";
                    item.RptVersion = "";
                    item.ReportStyleNum = 1005;
                    item.WorkstationID = "";
                    item.TaskNote = "";
                    item.ArchiveCode = 0;
                    item.DateFormat = "m/d/yyyy";
                    item.NumericFormat = ",.";
                    item.AgentCompareString = "";
                    item.ProcessID = "";
                    item.ProcessTaskNum = 0;
                    item.DecimalsGeneral = 0;
                    item.DecimalsCost = 0;
                    item.DecimalsPrice = 0;
                    item.GlbDecimalsGeneral = 0;
                    item.GlbDecimalsCost = 0;
                    item.GlbDecimalsPrice = 0;
                    item.FaxSubject = subject;
                    item.FaxTo = "";
                    item.FaxNumber = "";
                    item.EMailTo = email;
                    item.EMailBody = sb.ToString();

                    item.AttachmentType = "PDF";
                    item.ReportCurrencyCode = "USD";
                    item.ReportCultureCode = "en-US";
                    item.SSRSRenderFormat = "PDF";
                    item.SSRSEnableRouting = true;
                    item.RowMod = "A";
                }

                // Make request to send report
                EpicorRest.DynamicPost("Erp.RPT.SalesOrderAckSvc", "RunDirect", obj, true, statusCode, callContext);
            }

            return statusCode.RestCallStatusCode;
        }

        #region Private Methods

        ///// <summary>
        ///// Get blank parameter data set
        ///// </summary>
        ///// <returns>dynamic json object</returns>
        //private static dynamic GetNewParameters()
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
        //        newObj = EpicorRest.DynamicPost("Erp.RPT.SalesOrderAckSvc", "GetNewParameters", dic);
        //    }

        //    return newObj;
        //}

        #endregion

        /// Jasmine 21-11-2021 Start
        /// <summary>
        /// Send quotation copy by quote num and sales rep code.
        /// Keep in mind this currently only works for open quotes
        /// </summary>
        /// <param name="id">Quote Num</param>
        /// <param name="email">Email address</param>
        /// <param name="salesRepCode">Sales Rep Code</param>
        /// <returns>HttpStatusCode</returns>
        public static HttpStatusCode SendQuotePrintBySalesRep(int id, string email, string salesRepCode)
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);

            CallContextHeader callContext = new CallContextHeader();
            EpicorRestStatusCode statusCode = new EpicorRestStatusCode();

            // Build mail body
            var sb = new StringBuilder();
            sb.AppendLine("Hello and thank you for your quotation!");
            sb.AppendLine();
            sb.AppendLine("Attached you will find your quotation copy.");
            sb.AppendLine();
            sb.AppendLine("As always we appreciate your business!");
            sb.AppendLine();
            sb.AppendLine(String.Format("Should you have any concerns, please feel free to reach out to " +
                "us at {0} and ask for customer service.", EnvironmentSettings.AppSettings("PhoneNumber")));

            using (EpicorSession sess = new EpicorSession())
            {
                var subject = "";

                // Get Quote details
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("$filter", $"GRC_OpenQuotesActiveReps_PrimRepCode eq '" + salesRepCode + "'");
                dynamic quoteObj = EpicorRest.DynamicGet("BaqSvc", "GRC-OpenQuotesTicket9520", dic);
                dic.Clear();

                if (quoteObj.value.Count > 0)
                {
                    foreach (var item in quoteObj.value)
                    {
                        if (item.GRC_OpenQuotesActiveReps_QuoteNum == id)
                        {
                            subject = String.Format("GRC Quote Reference: {0}", item.GRC_OpenQuotesActiveReps_QuoteNum);
                        }
                    }
                }
                else
                {
                    // if nothing was found then we need to return nothing so junk is not sent to the task agent
                    return HttpStatusCode.NotFound;
                }

                // Get Dynamic object to modify
                dynamic obj = EpicorRest.DynamicPost("Erp.RPT.QuotFormSvc", "GetNewParameters", dic); 

                // Populate parameter values
                foreach (var item in obj.ds.QuoteFormParam)
                {
                    item.QuoteNum = id;
                    item.SysRowID = Guid.Parse("00000000-0000-0000-0000-000000000000");
                    item.AutoAction = "SSRSPrint";
                    item.AgentSchedNum = 0;
                    item.AgentID = "SystemTaskAgent";
                    item.AgentTaskNum = 0;
                    item.RecurringTask = false;
                    item.RptPageSettings = "";
                    item.RptPrinterSettings = "";
                    item.RptVersion = "";
                    item.ReportStyleNum = 2; // 1002;
                    item.WorkstationID = "";
                    item.TaskNote = "";
                    item.ArchiveCode = 0;
                    item.DateFormat = "m/d/yyyy";
                    item.NumericFormat = ",.";
                    item.AgentCompareString = "";
                    item.ProcessID = "";
                    item.ProcessTaskNum = 0;
                    item.DecimalsGeneral = 0;
                    item.DecimalsCost = 0;
                    item.DecimalsPrice = 0;
                    item.GlbDecimalsGeneral = 0;
                    item.GlbDecimalsCost = 0;
                    item.GlbDecimalsPrice = 0;
                    item.FaxSubject = subject;
                    item.FaxTo = "";
                    item.FaxNumber = "";
                    item.EMailTo = email;
                    item.EMailBody = sb.ToString();

                    item.AttachmentType = "PDF";
                    item.ReportCurrencyCode = "USD";
                    item.ReportCultureCode = "en-US";
                    item.SSRSRenderFormat = "PDF";
                    item.SSRSEnableRouting = true;
                    item.RowMod = "A";
                }

                // Make request to send report
                EpicorRest.DynamicPost("Erp.RPT.QuotFormSvc", "RunDirect", obj, true, statusCode, callContext);
            }

            return statusCode.RestCallStatusCode;
        }

        /// <summary>
        /// Send quotation copy by quote num and sales rep code.
        /// </summary>
        /// <param name="id">Quote Num</param>
        /// <param name="email">Email address</param>
        /// <param name="salesRepCode">Sales Rep Code</param>
        /// <returns>HttpStatusCode</returns>
        public static HttpStatusCode SendQuotePrintByOrderNum(int id, string email, string salesRepCode)
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);

            CallContextHeader callContext = new CallContextHeader();
            EpicorRestStatusCode statusCode = new EpicorRestStatusCode();

            // Build mail body
            var sb = new StringBuilder();
            sb.AppendLine("Hello and thank you for your quotation!");
            sb.AppendLine();
            sb.AppendLine("Attached you will find your quotation copy.");
            sb.AppendLine();
            sb.AppendLine("As always we appreciate your business!");
            sb.AppendLine();
            sb.AppendLine(String.Format("Should you have any concerns, please feel free to reach out to " +
                "us at {0} and ask for customer service.", EnvironmentSettings.AppSettings("PhoneNumber")));

            using (EpicorSession sess = new EpicorSession())
            {
                var subject = "";

                // Get order details
                dynamic quoteObj = EAL.Quotes.GetQuotationSvc(id, salesRepCode);

                // Check to see if this order can be requested by this sales rep id.  If not, give unauthorized message.
                //var dtl = quoteObj.value[0];
                //if (dtl.QuoteNum == id && dtl.SalesRepList.ToString().Contains(salesRepCode))
                if (quoteObj.value.Count > 0)
                {
                    subject = String.Format("GRC Quote Reference: {0}", quoteObj.value[0].QSalesRP_QuoteNum);
                }
                else
                {
                    return HttpStatusCode.Unauthorized;
                }

                // Get Dynamic object to modify
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dynamic obj = EpicorRest.DynamicPost("Erp.RPT.QuoteFormSvc", "GetNewParameters", dic); ;

                // Populate parameter values
                foreach (var item in obj.ds.QuoteFormParam)
                {
                    item.QuoteNum = id;
                    item.SysRowID = Guid.Parse("00000000-0000-0000-0000-000000000000");
                    item.AutoAction = "SSRSPrint";
                    item.AgentSchedNum = 0;
                    item.AgentID = "SystemTaskAgent";
                    item.AgentTaskNum = 0;
                    item.RecurringTask = false;
                    item.RptPageSettings = "";
                    item.RptPrinterSettings = "";
                    item.RptVersion = "";
                    item.ReportStyleNum = 1002;
                    item.WorkstationID = "";
                    item.TaskNote = "";
                    item.ArchiveCode = 0;
                    item.DateFormat = "m/d/yyyy";
                    item.NumericFormat = ",.";
                    item.AgentCompareString = "";
                    item.ProcessID = "";
                    item.ProcessTaskNum = 0;
                    item.DecimalsGeneral = 0;
                    item.DecimalsCost = 0;
                    item.DecimalsPrice = 0;
                    item.GlbDecimalsGeneral = 0;
                    item.GlbDecimalsCost = 0;
                    item.GlbDecimalsPrice = 0;
                    item.FaxSubject = subject;
                    item.FaxTo = "";
                    item.FaxNumber = "";
                    item.EMailTo = email;
                    item.EMailBody = sb.ToString();

                    item.AttachmentType = "PDF";
                    item.ReportCurrencyCode = "USD";
                    item.ReportCultureCode = "en-US";
                    item.SSRSRenderFormat = "PDF";
                    item.SSRSEnableRouting = true;
                    item.RowMod = "A";
                }

                // Make request to send report
                EpicorRest.DynamicPost("Erp.RPT.QuoteFormSvc", "RunDirect", obj, true, statusCode, callContext);
            }

            return statusCode.RestCallStatusCode;
        }

        /// <summary>
        /// Send quotation copy by quote num and sales rep code.
        /// Keep in mind this currently only works for open quotes
        /// </summary>
        /// <param name="id">Quote Num</param>
        /// <param name="email">Email address</param>
        /// <param name="salesRepCode">Sales Rep Code</param>
        /// <returns>HttpStatusCode</returns>
        public static HttpStatusCode SendQuotePrintBySalesRepGroup(int id, string email, string salesRepGroup)
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);

            CallContextHeader callContext = new CallContextHeader();
            EpicorRestStatusCode statusCode = new EpicorRestStatusCode();

            // Build mail body
            var sb = new StringBuilder();
            sb.AppendLine("Hello and thank you for your quotation!");
            sb.AppendLine();
            sb.AppendLine("Attached you will find your quotation copy.");
            sb.AppendLine();
            sb.AppendLine("As always we appreciate your business!");
            sb.AppendLine();
            sb.AppendLine(String.Format("Should you have any concerns, please feel free to reach out to " +
                "us at {0} and ask for customer service.", EnvironmentSettings.AppSettings("PhoneNumber")));

            using (EpicorSession sess = new EpicorSession())
            {
                var subject = "";

                // Get Quote details
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("$filter", $"GRC_OpenQuotesActiveReps_CodeID eq '" + salesRepGroup + "'");
                dynamic quoteObj = EpicorRest.DynamicGet("BaqSvc", "GRC-OpenQuotesTicket9520", dic);
                dic.Clear();

                if (quoteObj.value.Count > 0)
                {
                    foreach (var item in quoteObj.value)
                    {
                        if (item.GRC_OpenQuotesActiveReps_QuoteNum == id)
                        {
                            subject = String.Format("GRC Quote Reference: {0}", item.GRC_OpenQuotesActiveReps_QuoteNum);
                        }
                    }
                }
                else
                {
                    // if nothing was found then we need to return nothing so junk is not sent to the task agent
                    return HttpStatusCode.NotFound;
                }

                // Get Dynamic object to modify
                dynamic obj = EpicorRest.DynamicPost("Erp.RPT.QuotFormSvc", "GetNewParameters", dic);

                // Populate parameter values
                foreach (var item in obj.ds.QuoteFormParam)
                {
                    item.QuoteNum = id;
                    item.SysRowID = Guid.Parse("00000000-0000-0000-0000-000000000000");
                    item.AutoAction = "SSRSPrint";
                    item.AgentSchedNum = 0;
                    item.AgentID = "SystemTaskAgent";
                    item.AgentTaskNum = 0;
                    item.RecurringTask = false;
                    item.RptPageSettings = "";
                    item.RptPrinterSettings = "";
                    item.RptVersion = "";
                    item.ReportStyleNum = 2; // 1002;
                    item.WorkstationID = "";
                    item.TaskNote = "";
                    item.ArchiveCode = 0;
                    item.DateFormat = "m/d/yyyy";
                    item.NumericFormat = ",.";
                    item.AgentCompareString = "";
                    item.ProcessID = "";
                    item.ProcessTaskNum = 0;
                    item.DecimalsGeneral = 0;
                    item.DecimalsCost = 0;
                    item.DecimalsPrice = 0;
                    item.GlbDecimalsGeneral = 0;
                    item.GlbDecimalsCost = 0;
                    item.GlbDecimalsPrice = 0;
                    item.FaxSubject = subject;
                    item.FaxTo = "";
                    item.FaxNumber = "";
                    item.EMailTo = email;
                    item.EMailBody = sb.ToString();

                    item.AttachmentType = "PDF";
                    item.ReportCurrencyCode = "USD";
                    item.ReportCultureCode = "en-US";
                    item.SSRSRenderFormat = "PDF";
                    item.SSRSEnableRouting = true;
                    item.RowMod = "A";
                }

                // Make request to send report
                EpicorRest.DynamicPost("Erp.RPT.QuotFormSvc", "RunDirect", obj, true, statusCode, callContext);
            }

            return statusCode.RestCallStatusCode;
        }

        /// Jasmine 21-11-2021 end

    }
}