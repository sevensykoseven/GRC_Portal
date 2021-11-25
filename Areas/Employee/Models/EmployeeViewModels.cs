using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using protean.Models;
using protean.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace protean.Areas.Employee.Models
{
    public class ImportSalesRepViewModel
    {
        public dynamic ResponseObject { get; set; }
    }

    public class EditRolesViewModel
    {
        public ApplicationUser User { get; set; }

        public List<SelectRoleEditor> Roles { get; set; }

        public EditRolesViewModel()
        {
            Roles = new List<SelectRoleEditor>();
        }

        public string UserId { get; set; }

    }

    public class RegionSalesViewModel
    {
        public decimal RegionBookingsTotalYTD { get; set; }

        public decimal RegionBookingsQuotaYTD { get; set; }

        public decimal RegionQuotaYTDPercent { get; set; }

        public decimal RegionBookingsTotalMTD { get; set; }

        public decimal RegionBookingsQuotaM { get; set; }

        public decimal RegionQuotaMPercent { get; set; }

        public dynamic ResponseObject { get; set; }

        public string Region { get; set; }
    }

    public class RepGroupDetails
    {
        public string RepGroupName { get; set; }

        public decimal BookingsTotalYTD { get; set; }

        public decimal BookingsQuotaYTD { get; set; }

        public decimal QuotaYTDPercent { get; set; }

        public decimal BookingsTotalMonth { get; set; }

        public decimal BookingsQuotaMonth { get; set; }

        public decimal QuotaMonthPercent { get; set; }

        public string QuoteContributionLabel { get; set; }

        public string QuoteContributionData { get; set; }

        public dynamic QuotaResponseObject { get; set; }

        public string BookingsLabel { get; set; }

        public string BookingsData { get; set; }

        public string PrevBookingsData { get; set; }

        public decimal PriorMonthBookings { get; set; }

        public decimal PriorYearQuota { get; set; }

        public decimal PriorYearBookings { get; set; }

        public decimal PriorYTDBookings { get; set; }

        public decimal PriorYearPercent { get; set; }

        public decimal PriorYTDPercent { get; set; }

        public decimal PriorYTDQuota { get; set; }

        public string BookingsLabel1 { get; set; }

        public string BookingsLabel2 { get; set; }
    }

    public class RegionSalesExecViewModel
    {
        public decimal RegionBookingsTotalYTD { get; set; }

        public decimal RegionBookingsQuotaYTD { get; set; }

        public decimal RegionQuotaYTDPercent { get; set; }

        public decimal RegionBookingsTotalMTD { get; set; }

        public decimal RegionBookingsQuotaM { get; set; }

        public decimal RegionQuotaMPercent { get; set; }

        public dynamic ResponseObject { get; set; }

        public string Region { get; set; }

        public Dictionary<string, decimal> regionYTDBookings { get; set; }

        public Dictionary<string, decimal> regionYTDQuota { get; set; }

        public Dictionary<string, decimal> RegionPercents { get; set; }

        public Dictionary<string, decimal> Contributions { get; set; }

    }

    public class RegionQuoteOverviewViewModel
    {
        public Dictionary<string, int> CountData1 { get; set; }

        public string CountLabel1 { get; set; }

        public Dictionary<string, decimal> SalesData1 { get; set; }

        public string SalesLabel1 { get; set; }

        public string Region { get; set; }

        public dynamic ResponseObject { get; set; }
    }

    ///Jasmine 22-11-2021 start
    
    public class DocumentViewModel
    {
        public dynamic ResponseObject { get; set; }

        [DisplayName("File ID")]
        public string FileID { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }

        [DisplayName("File Image")]
        public string FileImg { get; set; }

        public HttpPostedFileBase UploadFile { get; set; }
    }


    public class Document
    {
        [DisplayName("File ID")]
        public string FileID { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }

        [DisplayName("File Image")]
        public string FileImg { get; set; }

        public HttpPostedFileBase UploadFile { get; set; }
    }


}