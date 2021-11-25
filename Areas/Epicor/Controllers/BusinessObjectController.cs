using System;
using System.Web.Mvc;
using protean.Infrastructure;
using Erp.BO;
using Erp.Proxy.BO;
using Ice.Core;
using Ice.Lib.Framework;
using System.Web.UI.WebControls;
using Erp.Tablesets;
using System.Data;
using MyUtils;

namespace protean.Areas.Epicor.Controllers
{
    [SelectedSidebar("businessobjecttest")]
    public class BusinessObjectController : Controller
    {
        // GET: Epicor/BusinessObject
        public ActionResult Index()
        {
            LaborHedListDataSet ret = new LaborHedListDataSet();
            // Create our session
            using (Session session = new Session(EnvironmentSettings.AppSettings("EpicorUsername"), EnvironmentSettings.AppSettings("EpicorPassword"), @"net.tcp://GRC-E10APP/EpicorTest10", Ice.Core.Session.LicenseType.Default, @"C:\Epicor\ERP10.2Client\Client\config\EpicorTest10.sysconfig", false))
            {
                session.CompanyID = "GRC";
                LaborImpl labor = WCFServiceSupport.CreateImpl<LaborImpl>(session, "Erp/BO/Labor");

                string where = String.Format(@"Company='{0}' AND ActiveTrans=True", session.CompanyID);
                bool more = false;
                ret = labor.GetList(where, 0, 0, out more);
            }

            //foreach (Erp.BO.LaborHedListDataSet.LaborHedListRow row in ret.Tables["LaborHedList"].Rows)
            //{

            //    Utils.WriteDebugLog(row.EmployeeNumFirstName.ToString());
            //}

            return View(ret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClockOutEmployee(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Utils.WriteDebugLog("Clock out employee " + id.ToString());

            string ret = "No action taken";
            // Create our session
            using (Session session = new Session(EnvironmentSettings.AppSettings("EpicorUsername"), EnvironmentSettings.AppSettings("EpicorPassword"), @"net.tcp://GRC-E10APP/EpicorTest10", Ice.Core.Session.LicenseType.Default, @"C:\Epicor\ERP10.2Client\Client\config\EpicorTest10.sysconfig", false))
            {
                // Set the company ID
                session.CompanyID = "GRC";
                // Create our implementation
                EmpBasicImpl empBasic = WCFServiceSupport.CreateImpl<EmpBasicImpl>(session, "Erp/BO/EmpBasic");
                LaborImpl labor = WCFServiceSupport.CreateImpl<LaborImpl>(session, "Erp/BO/Labor");
                // Get the employee record to get the shift
                EmpBasicDataSet emp = empBasic.GetByID(id);
                // Get the shift record
                int shift = (int)emp.Tables["EmpBasic"].Rows[0]["Shift"];

                // Now we need to see if they are already clocked in or not
                string where = String.Format(@"EmployeeNum='{0}' AND Company='{1}' AND ActiveTrans=True", id, session.CompanyID);

                //string where = "Company='GRC' AND ActiveTrans=True";
                bool more = false;
                LaborHedListDataSet ds = labor.GetList(where, 0, 0, out more);

                LaborDataSet details = labor.GetRows(where, where, "", "", "", "", "", "", "", "", "", "", 0, 0, out more);

                // See if we have an active trans
                if (ds.Tables["LaborHedList"].Rows.Count > 0)
                {
                    // We have an active laborhed, we need to clock out

                    // Check to see if we have labordtls
                    if (details.Tables["LaborDtl"].Rows.Count > 0)
                    {
                        foreach (DataRow row in details.Tables["LaborDtl"].Rows)
                        {
                            row["EndActivity"] = true;
                            row["RowMod"] = "U";
                        }
                        labor.EndActivity(details);
                        labor.Update(details);
                    }
                    ret = String.Format(@"Employee {0} clocked out", id);
                    empBasic.ClockOut(ref id);
                }
                else
                {
                    ret = String.Format(@"Employee {0} not clocked in", id);
                }
            }

            Utils.WriteDebugLog(ret);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}
