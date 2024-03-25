using LIB.Tools.Security;
using LIB.Tools.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Weblib.Helpers
{
    public class AuthActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Authentication.CheckUser())
            {
                filterContext.Result = new RedirectResult(Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(System.Web.HttpContext.Current.Request.Url.AbsolutePath));
                return;
            }    

            base.OnActionExecuting(filterContext);           
        }
    }
}
