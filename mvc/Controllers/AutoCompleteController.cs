using LIB.Tools.BO;
using LIB.Tools.Security;
using LIB.Tools.Utils;
using System;
using System.Web;
using System.Web.Mvc;
using JuliaAlertweblib.Controllers;
using LIB.Helpers;

namespace JuliaAlert.Controllers
{
    public class AutoCompleteController : FrontEndController
    {
        //
        // GET: /AutoComplete/

        public ActionResult List(string Namespace)
        {
            var item = (ItemBase)Activator.CreateInstance(Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true));

            if (item.CheckAutocompleteSecurity())
            {
                if (!Authentication.CheckUser(this.HttpContext))
                {
                    return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });
                }
            }

            var Items = item.PopulateAutocomplete(Request.QueryString["cond"], Request.QueryString["term"]);
            return View("Search", Items);
        }
    }
}