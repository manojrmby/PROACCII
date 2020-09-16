using System.Web.Optimization;

namespace PROACCII.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //ScriptBundle
            //bundles.Add(new ScriptBundle("~/bundles/Scripts/Layout").Include(
            //            "~/Asset/Scripts/jquery.min.js",
            //            "~/Asset/scripts/main.js",
            //            "~/Asset/Scripts/bootstrap.min.js"
            //            ));
            //bundles.Add(new ScriptBundle("~/bundles/Scripts/jquery-ui").Include(
            //    "~/Asset/Jquery/jquery-ui.js"
            //     ));
            //bundles.Add(new ScriptBundle("~/bundles/Scripts/ChartJS").Include(
            //   "~/Asset/Scripts/Chart.js",
            //   "~/Asset/Scripts/Chart.RadialGauge.umd.js",
            //   "~/Asset/Scripts/chartjs-plugin-labels.min.js"
            //    ));
            //bundles.Add(new ScriptBundle("~/bundles/Scripts/jqGrid").Include(
            //  "~/Asset/Jquery/jqGrid/grid.locale-en.js",
            //  "~/Asset/Jquery/jqGrid/jquery.jqGrid.min.js"
            //   ));


            ////StyleBundle
            //bundles.Add(new StyleBundle("~/Content/css/Layout").Include(
            //          "~/Asset/css/main.css",
            //          "~/Asset/css/LayoutStyles.css",
            //          "~/Asset/Jquery/jquery-ui.css",
            //          "~/Asset/css/Menu.css"
            //          ));

            //bundles.Add(new StyleBundle("~/Content/css/jqGrid").Include(
            //         "~/Asset/Jquery/jqGrid/css/octicons.css",
            //         "~/Asset/Jquery/jqGrid/css/ui.jqgrid-bootstrap4.css"
            //         ));

            BundleTable.EnableOptimizations = true;
        }
    }
}