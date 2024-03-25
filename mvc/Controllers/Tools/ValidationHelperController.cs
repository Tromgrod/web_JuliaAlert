namespace Galex.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web;
    using LIB.BusinessObjects;
    using LIB.Tools.Security;
    using LIB.Tools.Utils;
    using LIB.Helpers;
    using Weblib.Controllers;

    public class ValidationHelperController : BaseController
    {
        [HttpPost]
        public ActionResult ValidateUserName()
        {
            if (!Authentication.CheckUser(this.HttpContext)) //TBD
                return new RedirectResult(Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            else if (!Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.CPAccess | (long)BasePermissionenum.SMIAccess))
                return new RedirectResult(Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));

            var Login = Request.Form["Login"];
            var Userid = Convert.ToInt64(Request.Form["Userid"]);

            var user = new User() { Login = Login, Timeout=0 };
            
            user = (User)user.PopulateOne(user);
            if (user == null || (user != null && (Userid != 0 || user.Id == Userid)))
                return this.Json(new RequestResult() { Result = RequestResultType.Fail });

            return this.Json(new RequestResult() { Result = RequestResultType.Success, Message = "Acest nume este utilizat deja" });
        }    
    }
}