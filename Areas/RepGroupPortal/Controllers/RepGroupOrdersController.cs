using protean.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using protean.Areas.RepGroupPortal.Models;
using Newtonsoft.Json;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.IO;
using MyUtils;

namespace protean.Areas.RepGroupPortal.Controllers
{
    [Authorize(Roles = "repgroupadmin")]
    public class RepGroupOrdersController : Controller
    {
        /// <summary>
        /// Get list of open orders for rep group
        /// </summary>
        /// <returns></returns>
        [SelectedSidebar("repgroupreports")]
        [HttpGet]
        public ActionResult OpenOrders()
        {
            dynamic newObj;
            int openOrderCount = 0;
            decimal openOrderTotal = 0.0M;

            newObj = EAL.Orders.GetRepGroupOpenOrders(Current.User.SalesRep.RepGroupId);
            openOrderCount = Convert.ToInt32(newObj.value.Count);
            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    openOrderTotal = openOrderTotal + Convert.ToDecimal(item.GRC_RepGroupOpenOrders_OrderAmt);
                }
            }

            return View(new OpenOrderViewModel { OpenOrderCount = openOrderCount, ResponseObject = newObj, OpenOrderTotal = openOrderTotal });
        }

        /// <summary>
        /// Get list of open orders and return an excel file
        /// </summary>
        /// <returns>Filestream of an excel file</returns>
        [HttpGet]
        public FileStreamResult GetOpenOrders_Excel()
        {
            // Get latest open orders
            dynamic newObj = EAL.Orders.GetRepGroupOpenOrders(Current.User.SalesRep.RepGroupId);

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
                        CustomerName = x.Field<string>("GRC_RepGroupOpenOrders_Name"),
                        CreditHold = x.Field<bool>("GRC_RepGroupOpenOrders_CreditHold"),
                        OrderHeld = x.Field<bool>("GRC_RepGroupOpenOrders_OrderHeld"),
                        PO = x.Field<string>("GRC_RepGroupOpenOrders_PONum"),
                        OrderNumber = x.Field<string>("GRC_RepGroupOpenOrders_Ordernum"),
                        OrderAmount = x.Field<string>("GRC_RepGroupOpenOrders_OrderAmt"),
                        OrderDate = x.Field<string>("GRC_RepGroupOpenOrders_OrderDate"),
                        SalesRep = x.Field<string>("GRC_RepGroupOpenOrders_SalesRepName"),
                        ShipDate = x.Field<string>("GRC_RepGroupOpenOrders_NeedByDate")
                    });

                /// Create worksheet
                var ws = pck.Workbook.Worksheets.Add("Open Orders");

                //Load the datatable and set the number formats...
                ws.Cells["A1"].LoadFromCollection(customizedData, true, TableStyles.Medium9);
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                /// Create stream and send file
                var memoryStream = new MemoryStream();

                pck.SaveAs(memoryStream);
                memoryStream.Position = 0;

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = String.Format("{0} Open Orders {1}.xlsx", Current.User.SalesRep.RepGroupDescription, DateTime.Today.ToString("yyyy-MM-dd")),
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        /// <summary>
        /// Get Open Order details for order by Id
        /// </summary>
        /// <param name="id">Id of order</param>
        /// <returns></returns>
        [HttpGet]
        [SelectedSidebar("repgroupreports")]
        public ActionResult OpenOrderDetails(int id)
        {
            dynamic newObj;

            var vm = new OpenOrderDetailViewModel();

            newObj = EAL.Orders.GetRepGroupOpenOrders(Current.User.SalesRep.RepGroupId);
            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    if (item.GRC_RepGroupOpenOrders_Ordernum == id)
                    {
                        vm.OrderDetails = item;
                    }
                }
            }

            vm.LineDetails = EAL.Orders.GetOrderDetails(Convert.ToInt32(id));

            return View(vm);
        }

        /// <summary>
        /// Return open order details excel stream
        /// </summary>
        /// <param name="id">Order number to get details</param>
        /// <returns>Filestream of an excel file</returns>
        public FileStreamResult GetOpenOrderDetails_Excel(string id)
        {
            id = Cryptography.DecodeAndDecrypt(id, GlobalVariables.InternalKey);

            // Get latest open orders
            dynamic newObj = EAL.Orders.GetOrderDetails(Convert.ToInt32(id));

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
                        OrderNum = x.Field<Int64>("OrderDtl_OrderNum"),
                        OrderLine = x.Field<Int64>("OrderDtl_OrderLine"),
                        PartNum = x.Field<string>("OrderDtl_PartNum"),
                        LineDescription = x.Field<string>("OrderDtl_LineDesc"),
                        Quantity = Convert.ToDouble(x.Field<string>("OrderDtl_SellingQuantity")),
                        UnitofMeasure = x.Field<string>("OrderDtl_SalesUM"),
                        UnitPrice = Convert.ToDecimal(x.Field<string>("OrderDtl_DocUnitPrice")),
                        Discount = x.Field<string>("OrderDtl_Discount"),
                        LineTotal = x.Field<string>("Calculated_LineTotal"),
                        Comment = x.Field<string>("OrderDtl_OrderComment")
                    });

                /// Create worksheet
                var ws = pck.Workbook.Worksheets.Add("Order Details");

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SelectedSidebar("repgroupreports")]
        [Authorize(Roles = "repgroupprincipal")]
        [HttpGet]
        public ActionResult OpenOrderCommissions()
        {
            dynamic newObj;
            int orderCount = 0;
            decimal commissionTotal = 0.0M;             

            newObj = EAL.Orders.GetRepGroupSalesOrderCommissions(Current.User.SalesRep.RepGroupId);
            orderCount = Convert.ToInt32(newObj.value.Count);
            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    commissionTotal = commissionTotal + Convert.ToDecimal(item.GRC_SalesOrderCommRpt_RepCom1);
                }
            }

            return View(new OpenOrderCommissionViewModel { OrderCount = orderCount, ResponseObject = newObj, CommissionTotal = commissionTotal });
        }

        /// <summary>
        /// Get commission details on order
        /// </summary>
        /// <param name="id">Id of order</param>
        /// <returns>OpenOrderDetailViewModel</returns>
        [HttpGet]
        [Authorize(Roles = "repgroupprincipal")]
        public ActionResult OpenOrderCommissionDetails(int id)
        {
            dynamic newObj;

            var vm = new OpenOrderDetailViewModel();

            newObj = EAL.Orders.GetRepGroupSalesOrderCommissions(Current.User.SalesRep.RepGroupId);

            Utils.WriteDebugLog("'" + newObj.ToString() + "'");

            if (newObj.value.Count > 0)
            {
                foreach (var item in newObj.value)
                {
                    if (item.GRC_SalesOrderCommRpt_OrderNum1 == id)
                    {
                        vm.OrderDetails = item;
                    }
                }
            }

            vm.LineDetails = EAL.Orders.GetOrderDetails(Convert.ToInt32(id));

            return View(vm);
        }

        /// <summary>
        /// Get open order commission report
        /// </summary>
        /// <returns>Filestream of an excel file</returns>
        [HttpGet]
        [Authorize(Roles = "repgroupprincipal")]
        public FileStreamResult GetOpenOrderCommissions_Excel()
        {
            // Get latest open order commissions
            dynamic newObj = EAL.Orders.GetRepGroupSalesOrderCommissions(Current.User.SalesRep.RepGroupId);

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
                        CustomerName = x.Field<string>("GRC_SalesOrderCommRpt_CustName1"),
                        OrderNum = x.Field<Int64>("GRC_SalesOrderCommRpt_OrderNum1"),
                        PO = x.Field<string>("GRC_SalesOrderCommRpt_PONum1"),
                        OrderDate = x.Field<DateTime>("GRC_SalesOrderCommRpt_OrderDate1").ToShortDateString(),
                        OrderTotal = x.Field<string>("GRC_SalesOrderCommRpt_OrderTot1"),
                        Tax = x.Field<string>("GRC_SalesOrderCommRpt_TaxAmt1"),
                        MiscCharges = x.Field<string>("GRC_SalesOrderCommRpt_MiscAmt1"),
                        OrderTotalProduct = x.Field<string>("GRC_SalesOrderCommRpt_OrderDtlTotalProduct1"),
                        Split = x.Field<Int64>("GRC_SalesOrderCommRpt_RepSplit1"),
                        CommissionableAmount = x.Field<string>("GRC_SalesOrderCommRpt_CommableAmt1"),
                        Rate = x.Field<string>("GRC_SalesOrderCommRpt_RepRate1"),
                        Commission = x.Field<string>("GRC_SalesOrderCommRpt_RepCom1"),
                        EstimatedShipDate = Utils.HandleDatesInJson(x.Field<dynamic>("GRC_SalesOrderCommRpt_ShipByDate1"))
                    });



                /// Create worksheet
                var ws = pck.Workbook.Worksheets.Add("Open Order Commissions");

                //Load the datatable and set the number formats...
                ws.Cells["A1"].LoadFromCollection(customizedData, true, TableStyles.Medium9);
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                /// Create stream and send file
                var memoryStream = new MemoryStream();

                pck.SaveAs(memoryStream);
                memoryStream.Position = 0;

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = String.Format("OpenOrderCommissions {0}.xlsx", DateTime.Today.ToString("yyyy-MM-dd")),
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        /// <summary>
        /// Send request to Epicor to queue request for order acknowledgement
        /// </summary>
        /// <param name="id">encrypted Id as a string</param>
        /// <param name="salesRepCode">sales rep code</param>
        /// <returns>HttpStatusCode</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendOrderAck(string id, string salesRepCode)
        {
            // Decrypt id
            id = Cryptography.DecodeAndDecrypt(id, GlobalVariables.InternalKey);

            if (string.IsNullOrWhiteSpace(salesRepCode))
            {
                salesRepCode = Current.User.SalesRepCode;
            }

            var result = EAL.RPT.SendSalesOrderAckBySalesRep(Convert.ToInt32(id), Current.User.Email, salesRepCode);

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
        public ActionResult SendOrderAckByOrderNum(string id, string salesRepCode)
        {
            // Decrypt id
            //id = Cryptography.DecodeAndDecrypt(id, GlobalVariables.InternalKey);

            if (string.IsNullOrWhiteSpace(salesRepCode))
            {
                salesRepCode = Current.User.SalesRepCode;
            }

            var result = EAL.RPT.SendSalesOrderAckByOrderNum(Convert.ToInt32(id), Current.User.Email, salesRepCode);

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

        /// <summary>
        /// Get the order history for rep group
        /// </summary>
        /// <returns>OrderHistoryViewModel</returns>
        [HttpGet]
        [SelectedSidebar("repgrouporderhistory")]
        public ActionResult OrderHistory()
        {
            dynamic response = EAL.Orders.GetOrderHistoryByRepGroup(Current.User.SalesRep.RepGroupId, DateTime.Now.Year);

            var vm = new OrderHistoryViewModel
            {
                SelectedYear = DateTime.Now.Year,
                ResponseObject = response
            };

            return View(vm);
        }

        /// <summary>
        /// Get the order history for rep on filter change
        /// </summary>
        /// <param name="model">OrderHistoryViewModel</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SelectedSidebar("repgrouporderhistory")]
        public ActionResult OrderHistory(OrderHistoryViewModel model)
        {
            dynamic response = EAL.Orders.GetOrderHistoryByRepGroup(Current.User.SalesRep.RepGroupId, model.SelectedYear);

            model.ResponseObject = response;

            return View(model);
        }

        /// <summary>
        /// Get tracking numbers for order
        /// </summary>
        /// <param name="Id">Order number as a string</param>
        /// <returns>PartialView of TrackingNumbersViewModel</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrackingNumbers(int Id)
        {
            dynamic obj = EAL.Orders.GetTrackingNumbers(Id);

            return PartialView("TrackingNumbers", new TrackingNumbersViewModel { ResponseObject = obj });
        }

        ///Jasmine 01-11-2021
        /// <summary>
        /// Get tship details for order
        /// </summary>
        /// <param name="Id">Order number as a string</param>
        /// <returns>PartialView of ShipDetailsViewModel</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShipDetails(int Id)
        {
            dynamic obj = EAL.Orders.GetShipDetails(Id);

            return PartialView("ShipDetails", new ShipDetailsViewModel { ResponseObject = obj, OrderNum = Id });
        }

        /// <summary>
        /// Create an excel download of order history by year
        /// </summary>
        /// <param name="year">Year as Integer</param>
        /// <returns>Excel stream</returns>
        [HttpGet]
        public FileStreamResult OrderHistory_Excel(int year)
        {
            // Get latest open order commissions
            dynamic newObj = EAL.Orders.GetOrderHistoryByRepGroup(Current.User.SalesRep.RepGroupId, year);

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
                        CustomerName = x.Field<string>("Customer_Name"),
                        OrderNum = x.Field<Int64>("OrderHed_OrderNum"),
                        PO = x.Field<string>("OrderHed_PONum"),
                        Rep = x.Field<string>("SalesRep_Name"),
                        OrderDate = x.Field<DateTime>("OrderHed_OrderDate").ToShortDateString(),
                        OrderTotal = x.Field<string>("OrderHed_OrderAmt")
                    }); ;



                /// Create worksheet
                var ws = pck.Workbook.Worksheets.Add(year.ToString());

                //Load the datatable and set the number formats...
                ws.Cells["A1"].LoadFromCollection(customizedData, true, TableStyles.Medium9);
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                /// Create stream and send file
                var memoryStream = new MemoryStream();

                pck.SaveAs(memoryStream);
                memoryStream.Position = 0;

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = String.Format("{0} OrderHistory {1}.xlsx", year.ToString(), DateTime.Today.ToString("yyyy-MM-dd")),
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}