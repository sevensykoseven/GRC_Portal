using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using protean.Areas.RepPortal.Models;
using protean.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyUtils;

namespace protean.Areas.RepPortal.Controllers
{
    [Authorize(Roles = "salesrep")]
    [SelectedSidebar("repportal")]
    public class QuotesController : Controller
    {
        /// <summary>
        /// Get Open quotes for this rep
        /// </summary>
        /// <returns>OpenQuoteViewModel</returns>
        [SelectedSidebar("openquotereport")]
        [HttpGet]
        public ActionResult OpenQuotes()
        {

            dynamic newObj;
            int openQuoteCount = 0;
            decimal openQuoteTotal = 0.0M;

            newObj = EAL.Quotes.GetRepOpenQuotes(Current.User.SalesRepCode);
            openQuoteCount = Convert.ToInt32(newObj.value.Count);
            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    openQuoteTotal = openQuoteTotal + Convert.ToDecimal(item.GRC_OpenQuotesActiveReps_QuoteTotal);
                }
            }

            return View(new OpenQuoteViewModel { OpenQuoteCount = openQuoteCount, ResponseObject = newObj, OpenQuoteTotal = openQuoteTotal });
        }

        /// <summary>
        /// Get open quotes for rep in excel
        /// </summary>
        /// <returns>Filestream of an excel fil</returns>
        [HttpGet]
        public FileStreamResult GetOpenQuotes_Excel()
        {
            // Get latest open orders
            dynamic newObj = EAL.Quotes.GetRepOpenQuotes(Current.User.SalesRepCode);

            // set license
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // create an excel doc
            using (var pck = new ExcelPackage())
            {
                // convert data first into a dataset and then into a datatable
                // there is probably a better way to do with but I just couldn't get it to work
                // any other way                
                string json = "{'value':" + newObj.value.ToString() + "}";
                var ds = JsonConvert.DeserializeObject<DataSet>(json);
                var dt = new DataTable();
                dt = ds.Tables[0];

                // Now customize what the excel sheet looks like (column names, and columns)
                var customizedData = dt
                    .AsEnumerable()
                    .Select(x => new
                    {
                        EntryDate = Convert.ToDateTime(Utils.HandleDatesInJson(x.Field<dynamic>("GRC_OpenQuotesActiveReps_EntryDate"))).ToShortDateString(),
                        QuoteNumber = x.Field<Int64>("GRC_OpenQuotesActiveReps_QuoteNum"),
                        CustomerName = x.Field<string>("GRC_OpenQuotesActiveReps_CustomerName"),
                        Project = x.Field<string>("GRC_OpenQuotesActiveReps_Project"),
                        PO = x.Field<string>("GRC_OpenQuotesActiveReps_PONum"),
                        TotalPotential = x.Field<string>("GRC_OpenQuotesActiveReps_DocTotalPotential"),
                        TotalMiscAmt = x.Field<string>("GRC_OpenQuotesActiveReps_DocTotalMiscAmt"),
                        Tax = x.Field<string>("GRC_OpenQuotesActiveReps_DocTax"),
                        QuoteTotal = x.Field<string>("GRC_OpenQuotesActiveReps_QuoteTotal"),
                        QuoteDiscount = x.Field<string>("GRC_OpenQuotesActiveReps_DiscountPercent"),
                        PrimarySalesRep = x.Field<string>("GRC_OpenQuotesActiveReps_PrimRepName"),
                        RepGroup = x.Field<string>("GRC_OpenQuotesActiveReps_CodeDesc")
                    }); ;

                /// Create worksheet
                var ws = pck.Workbook.Worksheets.Add("Open Quotes");

                //Load the datatable and set the number formats...
                ws.Cells["A1"].LoadFromCollection(customizedData, true, TableStyles.Medium9);
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                /// Create stream and send file
                var memoryStream = new MemoryStream();

                pck.SaveAs(memoryStream);
                memoryStream.Position = 0;

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = String.Format("OpenQuotes {0}.xlsx", DateTime.Today.ToString("yyyy-MM-dd")),
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        //Jasmine 17-11-2021 start
        /// <summary>
        /// Get Open Quote details for order by Id
        /// </summary>
        /// <param name="id">Id of order</param>
        /// <returns></returns>
        [HttpGet]
        [SelectedSidebar("openquotereport")]
        public ActionResult OpenQuoteDetails(int id)
        {
            dynamic newObj;

            var vm = new OpenQuoteDetailViewModel();

            newObj = EAL.Quotes.GetRepOpenQuotes(Current.User.SalesRepCode);
            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    if (item.GRC_OpenQuotesActiveReps_QuoteNum == id)
                    {
                        vm.QuoteDetails = item;
                    }
                }
            }

            if (vm.QuoteDetails != null)
            {
                vm.QuoteLineDetails = EAL.Quotes.GetQuoteDetails(Convert.ToInt32(id));
            }

            return View(vm);
        }


        /// <summary>
        /// Return open quote details excel stream
        /// </summary>
        /// <param name="id">Quote number to get details</param>
        /// <returns>Filestream of an excel file</returns>
        [HttpGet]
        public FileStreamResult GetOpenQuoteDetails_Excel(string id)
        {
            id = Cryptography.DecodeAndDecrypt(id, GlobalVariables.InternalKey);

            // Get latest open quotes
            dynamic newObj = EAL.Quotes.GetQuoteDetails(Convert.ToInt32(id));

            // set license
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // create an excel doc
            using (var pck = new ExcelPackage())
            {
                // convert data first into a dataset and then into a datatable
                // there is probably a better way to do with but I just couldn't get it to work
                // any other way                
                string json = "{'value':" + newObj.value.ToString() + "}";
                var ds = JsonConvert.DeserializeObject<DataSet>(json);
                var dt = new DataTable();
                dt = ds.Tables[0];

                // Now customize what the excel sheet looks like (column names, and columns)
                var customizedData = dt
                    .AsEnumerable()
                    .Select(x => new
                    {
                        QuoteNum = x.Field<Int64>("QuoteDtl_QuoteNum"),
                        QuoteLine = x.Field<Int64>("QuoteDtl_QuoteLine"),
                        PartNum = x.Field<string>("QuoteDtl_PartNum"),
                        LineDescription = x.Field<string>("QuoteDtl_LineDesc"),
                        Quantity = Convert.ToDouble(x.Field<string>("QuoteDtl_OrderQty")),
                        UnitofMeasure = x.Field<string>("QuoteDtl_SellingExpectedUM"),
                        UnitPrice = Convert.ToDecimal(x.Field<string>("QuoteDtl_DocExpUnitPrice")),
                        Discount = x.Field<string>("QuoteDtl_Discount"),
                        LineTotal = x.Field<string>("Calculated_LineTotal"),
                        Comment = x.Field<string>("QuoteDtl_QuoteComment")
                    });

                /// Create worksheet
                var ws = pck.Workbook.Worksheets.Add("Quote Details");

                //Load the datatable and set the number formats...
                ws.Cells["A1"].LoadFromCollection(customizedData, true, TableStyles.Medium9);
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                /// Create stream and send file
                var memoryStream = new MemoryStream();

                pck.SaveAs(memoryStream);
                memoryStream.Position = 0;

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = String.Format("{0} Details {1}.xlsx", id.ToString(), DateTime.Today.ToString("yyyy-MM-dd")),
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

       //Jasmine 21-11-2021
        /// <summary>
        /// Send request to Epicor to queue request for Quote  print
        /// </summary>
        /// <param name="id">encrypted Id as a string</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendQuotePrint(string id)
        {
            // Decrypt id
            id = Cryptography.DecodeAndDecrypt(id, GlobalVariables.InternalKey);

            var result = EAL.RPT.SendQuotePrintBySalesRep(Convert.ToInt32(id), Current.User.Email, Current.User.SalesRepCode);

            if (result == System.Net.HttpStatusCode.OK)
            {
                return Json(result.ToString());
            }

            return Json(System.Net.HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Send request to Epicor to queue request for order acknowledgement
        /// </summary>
        /// <param name="id">OrderNum as a string</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendQuotePrintByOrderNum(string id)
        {
            // Decrypt id
            //id = Cryptography.DecodeAndDecrypt(id, GlobalVariables.InternalKey);

            var result = EAL.RPT.SendQuotePrintByOrderNum(Convert.ToInt32(id), Current.User.Email, Current.User.SalesRepCode);

            if (result == System.Net.HttpStatusCode.OK)
            {
                return Json(result.ToString());
            }

            if (result == System.Net.HttpStatusCode.Unauthorized)
            {
                Utils.WriteErrorLog("ATTEMPT TO ACCESS ORDER NOT BELONGING TO REP GROUP WAS MADE::" + Current.User.Email.ToString());
                return Json(result.ToString());
            }

            return Json(System.Net.HttpStatusCode.InternalServerError);
        }

        //Jasmine 21-11-2021 end
    }
}
