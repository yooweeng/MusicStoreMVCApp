using System.Web;
using System.Web.Optimization;

namespace MusicStoreMVCApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/Site.css",
                      "~/Content/fontawesome-free-6.4.0-web/css/all.css"));

            bundles.Add(new ScriptBundle("~/bundles/chosenjs").Include(
                      "~/Library/chosen_v1.8.7/chosen.jquery.min.js"));

            bundles.Add(new StyleBundle("~/bundles/chosencss").Include(
                      "~/Library/chosen_v1.8.7/chosen.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/dropzonejs").Include(
                      "~/Library/dropzone@5/dropzone.min.js"));

            bundles.Add(new StyleBundle("~/bundles/dropzonecss").Include(
                      "~/Library/dropzone@5/dropzone.min.css"));
        }
    }
}
