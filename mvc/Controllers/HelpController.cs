using LIB.Tools.Security;
using LIB.Tools.Utils;
using System.Web;
using System.Web.Mvc;

namespace JuliaAlert.Controllers
{
    public class HelpController : Controller
    {
        public ActionResult Index()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return new RedirectResult(Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));

            return View();
        }
    }
}