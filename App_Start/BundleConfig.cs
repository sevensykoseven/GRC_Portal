using System.Web.Optimization;
using protean.Infrastructure;

namespace protean
{
    public class BundleConfig
    {
        /// <summary>
        /// For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        /// </summary>
        /// <param name="bundles">BundleCollection</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Style bundle for external (before login)
            var external2Style = new StyleBundle("~/external/styles")
                .Include("~/Content/css/sb-admin-2.css")
                .Include("~/Content/css/external.css");
            external2Style.Orderer = new AsIsBundleOrderer();
            bundles.Add(external2Style);

            // Style bundle for internal (after login)
            var internal2Style = new StyleBundle("~/internal/styles")
                .Include("~/Content/css/sb-admin-2.css")
                .Include("~/Content/css/internal.css");
            internal2Style.Orderer = new AsIsBundleOrderer();
            bundles.Add(internal2Style);

            // Style bundle for dataTables
            var dataTablesStyle = new StyleBundle("~/datatables")
                .Include("~/Content/vendor/datatables/datatables.css");
            dataTablesStyle.Orderer = new AsIsBundleOrderer();
            bundles.Add(dataTablesStyle);

            // Style bundle for print ready
            var printReadyStyle = new ScriptBundle("~/printready/styles")
                .Include("~/Content/css/printready.css");
            printReadyStyle.Orderer = new AsIsBundleOrderer();
            bundles.Add(printReadyStyle);

            // Script bundle for external (before login)
            var externalScript = new ScriptBundle("~/external/scripts")
                .Include("~/Content/vendor/jquery/jquery-3.5.1.js")
                .Include("~/Content/vendor/bootstrap/bootstrap.bundle.js")
                .Include("~/Content/vendor/jquery-easing/jquery.easing.js")
                .Include("~/Content/vendor/jquery/jquery.validate.js")
                .Include("~/Content/js/jquery.validate.unobtrusive.js");
            externalScript.Orderer = new AsIsBundleOrderer();
            bundles.Add(externalScript);

            // Script bundle for internal (after login)
            var internalScript = new ScriptBundle("~/internal/scripts")
                .Include("~/Content/vendor/jquery/jquery-3.5.1.js")
                .Include("~/Content/vendor/bootstrap/js/bootstrap.bundle.js")
                .Include("~/Content/vendor/jquery-easing/jquery.easing.js")
                .Include("~/Content/js/sb-admin-2.js")
                .Include("~/Content/vendor/jquery/jquery.validate.js")
                .Include("~/Content/js/jquery.validate.unobtrusive.js")
                .Include("~/Content/js/common.js");                
            internalScript.Orderer = new AsIsBundleOrderer();
            bundles.Add(internalScript);

            // Script bundle for charts
            var chartjsScript = new ScriptBundle("~/chartjs")
                .Include("~/Content/vendor/chart.js/Chart.js");
            chartjsScript.Orderer = new AsIsBundleOrderer();
            bundles.Add(chartjsScript);

            // Script bundle for dataTables
            var dataTablesScript = new ScriptBundle("~/datatables")
                .Include("~/Content/vendor/datatables/datatables.js");
            //.Include("~/Content/vendor/datatables/DataTables-1.10.21/js/dataTables.bootstrap4.js");
            dataTablesScript.Orderer = new AsIsBundleOrderer();
            bundles.Add(dataTablesScript);

            //BundleTable.EnableOptimizations = true;
        }
    }

}
