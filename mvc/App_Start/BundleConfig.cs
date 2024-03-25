using System.Web.Optimization;

namespace JuliaAlert
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            var cssScripts = JuliaAlertweblib.Css.Register.RegisterBundles();

            foreach (var resourceRegister in cssScripts)
            {
                if (bundles.GetBundleFor("~/styles/" + resourceRegister.Key) != null)
                {
                    bundles.GetBundleFor("~/styles/" + resourceRegister.Key).Include(resourceRegister.File);
                }
                else
                {
                    bundles.Add(new StyleBundle("~/styles/" + resourceRegister.Key).Include(resourceRegister.File));
                }
            }

            var javaScripts = JuliaAlertweblib.Js.Register.RegisterBundles();

            foreach (var resourceRegister in javaScripts)
            {
                if (bundles.GetBundleFor("~/scripts/" + resourceRegister.Key) != null)
                {
                    bundles.GetBundleFor("~/scripts/" + resourceRegister.Key).Include(resourceRegister.File);
                }
                else
                {
                    bundles.Add(new ScriptBundle("~/scripts/" + resourceRegister.Key).Include(resourceRegister.File));
                }
            }

            bundles.Add(new ScriptBundle("~/scripts/dynamics").IncludeDirectory("~/Scripts/Dynamic", "*.js", true));
        }
    }
}