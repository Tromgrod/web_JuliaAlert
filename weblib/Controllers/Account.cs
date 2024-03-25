// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Account.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the Account type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Weblib.Controllers
{
    using System.Web.Mvc;
    using LIB.BusinessObjects;
    using System.Collections.Generic;
    using LIB.Helpers;

    public class AccountController : BaseController
    {
        protected ActionResult Login(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;
            this.ViewBag.Script = "login";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        protected ActionResult Login(User user, string returnUrl)
        {
            if (LIB.Tools.Security.Authentication.DoAuthorization(user))
            {
                if (string.IsNullOrEmpty(returnUrl))
                    returnUrl = LIB.Tools.Utils.URLHelper.GetUrl("");
                return this.Json(new RequestResult() { RedirectURL = returnUrl, Result = RequestResultType.Success });
            }
            var errorFields = new List<string>
            {
                "input[name=Login]",
                "input[name=Password]"
            };
            return this.Json(new RequestResult() { Message = "Acest utilizator nu exista", Result = RequestResultType.Fail, ErrorFields = errorFields });
        }

        protected ActionResult CPLogin(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;
            this.ViewBag.Script = "login";

            ViewData["LoginFail"] = null;
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        protected ActionResult CPLogin(User user, string returnUrl)
        {
            if (LIB.Tools.Security.Authentication.DoAuthorization(user, null, null, Modulesenum.ControlPanel))
            {
                if (string.IsNullOrEmpty(returnUrl))
                    returnUrl = LIB.Tools.Utils.URLHelper.GetUrl("ControlPanel");
                return this.Redirect(returnUrl);
            }
            ViewData["LoginFail"] = true;

            return this.View();
        }

        protected ActionResult SMILogin(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;
            this.ViewBag.Script = "login";

            ViewData["LoginFail"] = null;
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        protected ActionResult SMILogin(User user, string returnUrl)
        {
            if (LIB.Tools.Security.Authentication.DoAuthorization(user, null, null, Modulesenum.SMI))
            {
                if (string.IsNullOrEmpty(returnUrl))
                    returnUrl = LIB.Tools.Utils.URLHelper.GetUrl("SystemManagement");
                return this.Redirect(returnUrl);
            }
            ViewData["LoginFail"] = true;

            return this.View();
        }

        public ActionResult LogOff()
        {
            LIB.Tools.Security.Authentication.LogOff();
            return this.RedirectToAction("Login", "Account");
        }

        public ActionResult CPLogOff()
        {
            LIB.Tools.Security.Authentication.LogOff();
            return this.RedirectToAction("CPLogin", "Account");
        }

        public ActionResult SMILogOff()
        {
            LIB.Tools.Security.Authentication.LogOff();
            return this.RedirectToAction("SMILogin", "Account");
        }
    }
}