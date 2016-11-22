using System;
using System.Web.Optimization;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/sfajs").Include(
                              "~/dist/javascripts/jquery-1.11.0.min.js",
                              "~/dist/javascripts/select2.min.js",
                              "~/dist/javascripts/govuk-template.js",
                              "~/dist/javascripts/selection-buttons.js",
                              "~/dist/javascripts/app.js"
                              ));

            bundles.Add(new ScriptBundle("~/bundles/apprentice").Include(
                      "~/dist/javascripts/apprentice/select2.min.js",
                      "~/dist/javascripts/apprentice/dropdown.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalcustom").Include(
                      "~/Scripts/jquery.validate.js", "~/Scripts/jquery.validate.unobtrusive.custom.js"));
        }
    }
}
