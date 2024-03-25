using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using LIB.Tools.BO;
using LIB.Tools.Security;
using LIB.Tools.Utils;
using LIB.Helpers;
using JuliaAlertweblib.Controllers;

namespace JuliaAlert.Controllers
{
    public class DynamicControlController : FrontEndController
    {
        [HttpPost]
        public ActionResult Load()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            var Namespace = Request.Form["Namespace"];

            var item = (ItemBase)Activator.CreateInstance(Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true));

            var items = item.PopulateFrontEndItems();

            var viewData = item.LoadFrontEndViewdata();
            if (viewData != null && viewData.Count > 0)
            {
                foreach (var key in viewData.Keys)
                {
                    ViewData[key] = viewData[key];
                }
            }
            return View(item.GetType().Name, items);
        }

        [HttpPost]
        public ActionResult LoadNewItem(string ParentId)
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            var Namespace = Request.Form["Namespace"];
            ViewData["ParentId"] = Convert.ToInt64(ParentId);

            var item = (ItemBase)Activator.CreateInstance(Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true));

            return View(item.GetType().Name + "AddRow");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            try
            {
                var Namespace = Request.Form["Namespace"];
                var ItemId = Request.Form["Id"];

                var item = (ItemBase)Activator.CreateInstance(Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true));

                if (string.IsNullOrEmpty(ItemId) is false)
                    item.Id = long.Parse(ItemId);

                item.CollectFromForm();
                item.SaveForm();

                var items = item.PopulateFrontEndItems();

                var viewData = item.LoadFrontEndViewdata();
                if (viewData != null && viewData.Count > 0)
                {
                    foreach (var key in viewData.Keys)
                    {
                        ViewData[key] = viewData[key];
                    }
                }

                ViewData["NewItemId"] = item.Id;

                return View(item.GetType().Name, items);

            }
            catch (Exception ex)
            {
                return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = ex.ToString() });
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AllSave()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            try
            {
                var Namespace = Request.Form["Namespace"];

                var ItemIds = Request.Form["Id"]?.Split(',');

                var type = Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true);

                var items = new Dictionary<long, ItemBase>();

                for (var index = 0; index < ItemIds.Length; index++)
                {
                    var itemId = ItemIds[index];
                    var item = (ItemBase)Activator.CreateInstance(type);

                    if (long.TryParse(itemId, out var Id))
                        item.Id = Id;

                    item.ManyCollectFromForm(index);
                    item.SaveForm();

                    items.Add(item.Id, item);
                }

                return View(type.Name, items);
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
                item.Id = Convert.ToInt64(Request.Form["Id"]);

                item.Delete();

                return this.Json(new RequestResult() { Result = RequestResultType.Success });
            }
            catch (Exception ex)
            {
                return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = ex.ToString() });
            }
        }
    }
}