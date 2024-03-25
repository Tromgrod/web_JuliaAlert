using System;
using System.Web;
using System.Web.Mvc;
using LIB.Helpers;
using LIB.Tools.Security;
using LIB.Tools.Utils;
using LIB.Tools.BO;
using Weblib.Controllers;

namespace JuliaAlert.Controllers
{
    public class OrderController : BaseController
    {
        [HttpPost]
        public ActionResult UpdataImage()
        {
            if (!Authentication.CheckUser(this.HttpContext))
            {
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });
            }
            try
            {
                var Namespace = Request.Form["Namespace"];

                var item = (ItemBase)Activator.CreateInstance(Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true));

                item.Id = Convert.ToInt64(Request.Form["Id"]);

                var imageId = Convert.ToInt64(Request.Form["ImageId"]);

                item.UpdateProperties(item.GetImageName() + "Id", imageId);

                return this.Json(new RequestResult() { Result = RequestResultType.Reload, Message = "Anularea Effectuata", RedirectURL = URLHelper.GetUrl("") });
            }
            catch (Exception ex)
            {
                return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = ex.ToString() });
            }
        }
    }
}