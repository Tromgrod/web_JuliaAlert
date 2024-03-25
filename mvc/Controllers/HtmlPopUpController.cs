using System;
using System.Web;
using System.Web.Mvc;
using LIB.Tools.BO;
using LIB.Tools.Security;
using LIB.Tools.Utils;
using LIB.Helpers;
using JuliaAlertweblib.Controllers;

namespace JuliaAlert.Controllers
{
    public class HtmlPopUpController : FrontEndController
    {
        public ActionResult Open()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            try
            {
                var @namespace = Request.Form["Namespace"];
                var modelId = Request.Form["ModelId"];
                var view = Request.Form["View"];

                var item = (ItemBase)Activator.CreateInstance(Type.GetType(@namespace + ", " + @namespace.Split('.')[0], true));

                long.TryParse(modelId, out var itemId);

                var itemData = item.LoadPopupData(itemId);

                return this.View(string.IsNullOrEmpty(view) ? item.GetType().Name : view, itemData);
            }
            catch (Exception ex)
            {
                return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = ex.ToString() });
            }
        }

        public ActionResult OpenNoData()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            return this.View(Request.Form["View"]);
        }

        public JsonResult Save()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            var @namespace = Request.Form["Namespace"];

            var item = (ItemBase)Activator.CreateInstance(Type.GetType(@namespace + ", " + @namespace.Split('.')[0], true));

            item.Id = Convert.ToInt64(Request.Form["Id"]);

            item.CollectFromForm();

            return this.Json(item.SaveForm());
        }

        public JsonResult Delete()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            var Namespace = Request.Form["Namespace"];

            var item = (ItemBase)Activator.CreateInstance(Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true));

            item.Id = Convert.ToInt64(Request.Form["Id"]);

            if (item.Delete())
            {
                return this.Json(new RequestResult() { Result = RequestResultType.Success, Message = "Удалено" });
            }
            else
            {
                return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = "Не удалось удалить" });
            }
        }
    }
}