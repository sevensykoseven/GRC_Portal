using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using protean.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using protean.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using protean.Areas.Epicor.Models.Json;
using EpicorRestAPI;
using Newtonsoft.Json.Linq;
using MyUtils;

namespace protean.Areas.Epicor.Controllers
{
    [SelectedSidebar("resttest")]
    public class RESTController : Controller
    {
        // GET: Epicor/REST
        public ActionResult Index()
        {
            // Get everyone clocked in
            //var client = new RestClient(EnvironmentSettings.AppSettings("EpicorRestURL"));
            //client.Authenticator = new HttpBasicAuthenticator(EnvironmentSettings.AppSettings("EpicorUsername"), EnvironmentSettings.AppSettings("EpicorPassword"));

            //var request = new RestRequest("/BaqSvc/WEB-WhoIsClockedIn(GRC)/", DataFormat.Json);
            //var response = client.Get(request);

            //var employees = JsonConvert.DeserializeObject<EpicorEmployees>(response.Content);

            // Get everyone clocked in
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("$filter", $"ActiveTrans eq true");
            dynamic newObj = EpicorRest.DynamicGet("Erp.BO.LaborSvc", "Labors", dic);

            return View(newObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClockOutEmployee(string id)
        {
            EpicorRest.AppPoolHost = EnvironmentSettings.AppSettings("EpicorAppPoolHost");
            EpicorRest.AppPoolInstance = EnvironmentSettings.AppSettings("EpicorAppPoolInstance");
            EpicorRest.UserName = EnvironmentSettings.AppSettings("EpicorUsername");
            EpicorRest.Password = EnvironmentSettings.AppSettings("EpicorPassword");
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.Default;
            EpicorRest.CallSettings = new CallSettings("GRC", string.Empty, string.Empty, string.Empty);

            using (EpicorSession sess = new EpicorSession())
            {
                // Instanciate parameters
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("$filter", $"EmployeeNum eq '" + id + "' and ActiveTrans eq true");

                dynamic newObj = EpicorRest.DynamicGet("Erp.BO.LaborSvc", "Labors", dic);

                if (newObj.value.Count > 0)
                {
                    Dictionary<string, string> laborDic = new Dictionary<string, string>();
                    laborDic.Add("whereClauseLaborHed", "EmployeeNum = '" + id + "' and ActiveTrans = yes BY PayrollDate");
                    laborDic.Add("whereClauseLaborDtl", "EmployeeNum = '" + id + "' and ActiveTrans = yes");
                    laborDic.Add("whereClauseLaborDtlAttch", "");
                    laborDic.Add("whereClauseLaborDtlComment", "");
                    laborDic.Add("whereClauseLaborEquip", "");
                    laborDic.Add("whereClauseLaborPart", "");
                    laborDic.Add("whereClauseLbrScrapSerialNumbers", "");
                    laborDic.Add("whereClauseLaborDtlGroup", "");
                    laborDic.Add("whereClauseSelectedSerialNumbers", "");
                    laborDic.Add("whereClauseSNFormat", "");
                    laborDic.Add("whereClauseTimeWeeklyView", "");
                    laborDic.Add("whereClauseTimeWorkHours", "");
                    laborDic.Add("pageSize", "0");
                    laborDic.Add("absolutePage", "0");
                    dynamic laborDetails = EpicorRest.DynamicPost("Erp.BO.LaborSvc", "GetRows", laborDic);
                    foreach (var item in laborDetails.ds.LaborDtl)
                    {
                        item.EndActivity = true;
                        item.RowMod = "U";
                    }
                    EpicorRest.DynamicPost("Erp.BO.Laborsvc", "EndActivity", laborDetails);
                    EpicorRest.DynamicPost("Erp.BO.Laborsvc", "Update", laborDetails);
                }

                Dictionary<string, string> empClockout = new Dictionary<string, string>();
                empClockout.Add("employeeID", id);
                EpicorRest.DynamicPost("Erp.BO.EmpBasicSvc", "ClockOut", empClockout);
            }

            //////////////////////////////////////////////////////////////////////////////////////
            /// Clock out
            //////////////////////////////////////////////////////////////////////////////////////
            //var client = new RestClient("https://epicor.grandrapidschair.com/EpicorTest10/api/v1");
            //client.Authenticator = new HttpBasicAuthenticator("User009", "Password1");
            //var request = new RestRequest("/Erp.BO.EmpBasicSvc/ClockOut", Method.POST);

            //request.AddHeader("Accept", "application/json");
            //request.Parameters.Clear();

            ////var emp = new Employee { LaborHed_EmployeeNum = id };
            ////var str = JsonConvert.SerializeObject(emp);
            //String str = "{'employeeID':'4125'}";

            //request.AddParameter("application/json", str.Replace("LaborHed_EmployeeNum", "employeeID"), ParameterType.RequestBody);            

            //IRestResponse response = client.Post(request);
            //var content = response.Content;

            //Utils.WriteDebugLog(content);
            ////////////////////////////////////////////////////////////////////////////////////////


            return Json(new { success = true }, JsonRequestBehavior.AllowGet);

        }

    }
}



