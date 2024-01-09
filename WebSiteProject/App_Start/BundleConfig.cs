using System.Web;
using System.Web.Optimization;

namespace WebSiteProject
{
    public class BundleConfig
    {
        // 如需「搭配」的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            
            bundles.Add(new StyleBundle("~/Content/admincss/logincss").Include(
                     "~/Content/admincss/bootstrap.min.css",
                     "~/Content/admincss/components.min.css",
                     "~/Content/admincss/font-awesome.min.css",
                     "~/Content/admincss/style.css"
                 ));


            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                "~/Scripts/jquery-3.5.1.min.js",
                "~/Scripts/jquery-migrate-3.0.0.min.js",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/string.format.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                  "~/Content/admincss/components.min.css",
                  "~/Content/admincss/bootstrap.min.css"
                   ));

             bundles.Add(new StyleBundle("~/Content/login").Include(
                     "~/Content/bootstrap/bootstrap.min.css",
                     "~/Content/bootstrap/font-awesome.min.css",
                     "~/Content/animate.min.css",
                     "~/Content/admincss/urlfont.css",
                     "~/Content/admincss/style.css",
                     "~/Content/loginform.css"));

        }
    }
}
