using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using protean.Models;
using protean.Infrastructure;

namespace protean.Areas.CustomerPortal.Models
{
    public class CustomerPortalDBViewModel
    {
        public int OpenOrderCount { get; set; }

        public decimal OpenOrderTotal { get; set; }
    }

    public class CustomerPortalOOViewModel
    {
        public int OpenOrderCount { get; set; }

        public decimal OpenOrderTotal { get; set; }

        public dynamic ResponseObject { get; set; }
    }
}