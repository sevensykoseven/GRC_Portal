using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace protean.Areas.RepPortal.Models
{
    public class RepPortalDBViewModel
    {
        public int OpenOrderCount { get; set; }

        public decimal OpenOrderTotal { get; set; }

        public decimal OpenOrderCommissionTotal { get; set; }

        public string CommissionLabel { get; set; }

        public string CommissionData { get; set; }

        //Jasmine 02-11-2021
        public decimal OrderTotal { get; set; }

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

    //Jasmine 26-10-2021
    public class ShipDetailsViewModel
    {
        public dynamic ResponseObject { get; set; }

        public int OrderNum { get; set; }
    }

    //Jasmine 15-11-2021
    public class OpenQuoteDetailViewModel
    {
        public dynamic QuoteDetails { get; set; }

        public dynamic QuoteLineDetails { get; set; }
    }


}