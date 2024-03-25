using System;
using System.Web;
using System.Web.Mvc;
using LIB.Tools.BO;
using LIB.Tools.Security;
using LIB.Tools.Utils;
using LIB.Helpers;

namespace JuliaAlert.Controllers
{
    public class MultySelectController : JuliaAlertweblib.Controllers.FrontEndController
    {
        public ActionResult Options(string Namespace, string Param)
        {
            if (!Authentication.CheckUser(this.HttpContext))
            {
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });
            }

            var item = (ItemBase)Activator.CreateInstance(Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true));
            var Items = item.PopulateAutocomplete(Param,Request.QueryString["term"]);
            ViewData["Param"] = Param;
            ViewData["Values"] = !string.IsNullOrEmpty(Request.Form["values"])?Request.Form["values"].Split(','):null;
            ViewData["AllowDefault"] = false;
            return View("Options", Items);
        }
    }
}