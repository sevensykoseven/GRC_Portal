using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace protean.Areas.RepGroupPortal.Models
{
    public class RepGroupPortalDBViewModel
    {
        public int OpenOrderCount { get; set; }

        public decimal OpenOrderTotal { get; set; }

        public decimal OpenOrderCommissionTotal { get; set; }

        public decimal YearQuota { get; set; }

        public string BookingsLabel { get; set; }

        public string BookingsData { get; set; }

        public dynamic QuotaResponseObject { get; set; }

        public string QuoteContributionLabel { get; set; }

        public string QuoteContributionData { get; set; }

        public string ShippedLabel { get; set; }

        public string ShippedData { get; set; }

        public dynamic ProjectResponseObject { get; set; }
    }

    public class OpenOrderViewModel
    {
        public int OpenOrderCount { get; set; }

        public decimal OpenOrderTotal { get; set; }

        public dynamic ResponseObject { get; set; }
    }

    public class OpenOrderDetailViewModel
    {
        public dynamic OrderDetails { get; set; }

        public dynamic LineDetails { get; set; }

    }

    public class OpenOrderCommissionViewModel
    {
        public int OrderCount { get; set; }

        public decimal CommissionTotal { get; set; }

        public dynamic ResponseObject { get; set; }
    }

    public class OrderHistoryViewModel
    {
        public int SelectedYear { get; set; }

        public dynamic ResponseObject { get; set; }
    }

    public class TrackingNumbersViewModel
    {
        public dynamic ResponseObject { get; set; }
    }

    public class OpenQuoteViewModel
    {
        public int OpenQuoteCount { get; set; }

        public decimal OpenQuoteTotal { get; set; }

        public dynamic ResponseObject { get; set; }
    }

    //Jasmine 01-11-2021
    public class ShipDetailsViewModel
    {
        public dynamic ResponseObject { get; set; }

        public int OrderNum { get; set; }
    }

    //Jasmine 21-11-2021
    public class OpenQuoteDetailViewModel
    {
        public dynamic QuoteDetails { get; set; }

        public dynamic QuoteLineDetails { get; set; }
    }
}