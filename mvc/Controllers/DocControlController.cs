using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using LIB.Tools.BO;
using LIB.Tools.Security;
using LIB.Tools.Utils;
using LIB.Helpers;
using JuliaAlertLib.Security;
using JuliaAlertLib.BusinessObjects;
using JuliaAlertweblib.Controllers;

namespace JuliaAlert.Controllers
{
    public class DocControlController : PageObjectController
    {
        public ActionResult Edit(string Model, string Id = "", string additional = "")
        {
            var ModelItem = Model.Contains("_") ? Model.Split('_')[1] : Model;

            var item = (ItemBase)Activator.CreateInstance(Type.GetType("JuliaAlert.Models.Objects." + ModelItem, true));

            if (!item.HaveAccess(Model, Id))
                return Redirect(URLHelper.GetUrl("Error/AccessDenied"));

            if (!string.IsNullOrEmpty(Id))
            {
                item.Id = Convert.ToInt64(Id);

                item = item.PopulateFrontEnd(additional, item);

                ViewData["ParentId"] = item.Id;
            }

            var PageTitle = item.LoadPageTitle();
            if (!string.IsNullOrEmpty(PageTitle))
                ViewBag.Title = PageTitle;

            ViewData["Breadcrumbs"] = item.LoadBreadcrumbs(Model);
            return View(Model, item);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Save()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            try
            {
                var @namespace = Request.Form["Namespace"];
                var modelItem = @namespace.Split('.')[0];

                var item = (ItemBase)Activator.CreateInstance(Type.GetType(@namespace + ", " + modelItem, true));

                if (!item.HaveSaveAccess(modelItem, Request.Form["Id"]))
                    return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

                var usr = Authentication.GetCurrentUser();
                item.Id = Convert.ToInt64(Request.Form["Id"]);
                item.CollectFromForm();

                var redirectURL = item.ActionRedirect();
                if (!string.IsNullOrEmpty(redirectURL))
                    return this.Json(new RequestResult() { RedirectURL = URLHelper.GetUrl(redirectURL), Result = RequestResultType.Reload });

                return this.Json(item.SaveForm());
            }
            catch (Exception ex)
            {
                return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = ex.Message.ToString(), Data = new Dictionary<string, object> { { "Error", ex.ToString() } } });
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Update()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            try
            {
                var Namespace = Request.Form["Namespace"];
                var item = (ItemBase)Activator.CreateInstance(Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true));

                var MenuItems = (Dictionary<long, MenuGroup>)ViewData["MainMenu"];
                if (MenuItems != null)
                {
                    var usr = Authentication.GetCurrentUser();
                    if (!Authorization.hasPageAccess(MenuItems, usr, item))
                    {
                        return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });
                    }
                    item.Id = Convert.ToInt64(Request.Form["Id"]);
                    item.CollectFromForm();
                    item.Update(item);

                    return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload, Message = "Обновлено" });
                }
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });
            }
            catch (Exception ex)
            {
                return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = ex.ToString() });
            }
        }

        [HttpPost]
        public ActionResult Delete()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            try
            {
                var Namespace = Request.Form["Namespace"];
                var item = (ItemBase)Activator.CreateInstance(Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true));

                var usr = Authentication.GetCurrentUser();

                item.Id = Convert.ToInt64(Request.Form["Id"]);

                item.Delete();

                return this.Json(new RequestResult() { Result = RequestResultType.Reload, Message = "Удалено", RedirectURL = URLHelper.GetUrl("") });
            }
            catch (Exception ex)
            {
                return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = ex.ToString() });
            }
        }
    }
}