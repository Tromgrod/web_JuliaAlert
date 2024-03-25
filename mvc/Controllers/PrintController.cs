using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LIB.Tools.BO;
using LIB.Tools.Security;
using LIB.Tools.Utils;
using JuliaAlertLib.BusinessObjects;
using LIB.Helpers;
using Weblib.Converters;
using JuliaAlertweblib.Controllers;

namespace JuliaAlert.Controllers
{
    public class PrintController : FrontEndController
    {
        //[HttpPost]
        public ActionResult Print()
        {
            if (!Authentication.CheckUser(this.HttpContext))
            {
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });
            }
            try
            {
                ViewData["SandboxPrint"] = false;
                ViewData["Styles"] = System.IO.File.ReadAllText(Server.MapPath(@"~/Content/Print/common.css"));
                var Namespace = Request.Form["Namespace"];

                var item = (PrintBase)Activator.CreateInstance(Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true));

                var Filters = new Dictionary<string, string>();
                foreach (var postItem in Request.Form.AllKeys)
                {
                    Filters.Add(postItem, Request.Form[postItem]);
                }
                item.Filters = Filters;

                var View = item.LoadReport(ControllerContext, ViewData, TempData);

                if (!string.IsNullOrEmpty(item.StylesFile) && System.IO.File.Exists(Server.MapPath(@"~/Content/Print/" + item.StylesFile + ".css")))
                {
                    ViewData["Styles"] += System.IO.File.ReadAllText(Server.MapPath(@"~/Content/Print/" + item.StylesFile + ".css"));
                }

                return this.View(View, item);
            }
            catch (Exception ex)
            {
                return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = ex.ToString() });
            }
        }

        public ActionResult CustomPrint()
        {
            var item = (ItemBase)Activator.CreateInstance(Type.GetType(Request.Form["Namespace"], true));

            item.Id = long.Parse(Request.Form["ModelId"]);

            return this.View("Templates/" + Request.Form["ViewPrint"], item.PopulatePrintData());
        }

        [ValidateInput(false)]
        public void PrintBarcode()
        {

        }

        [ValidateInput(false)]
        public FileResult ExportWord()
        {
            var filedownload = new HttpCookie("fileDownload")
            {
                Expires = DateTime.Now.AddDays(1),
                Value = "true"
            };
            Response.Cookies.Add(filedownload);
            var path = new HttpCookie("path")
            {
                Expires = DateTime.Now.AddDays(1),
                Value = "/"
            };
            Response.Cookies.Add(path);
            if (!Authentication.CheckUser(this.HttpContext))
            {
                return File(WordHelper.HtmlToWord(""), "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            }
            try
            {
                ViewData["SandboxPrint"] = false;
                ViewData["Styles"] = "";
                var Namespace = Request.Form["Namespace"];

                var item = (PrintBase)Activator.CreateInstance(Type.GetType(Namespace + ", " + Namespace.Split('.')[0], true));

                var Filters = new Dictionary<string, string>();
                foreach (var postItem in Request.Form.AllKeys)
                {
                    Filters.Add(postItem, Request.Form[postItem]);
                }
                item.Filters = Filters;

                var View = item.LoadReport(ControllerContext, ViewData, TempData, ExportType.Word);

                var viewName = "~/Views/Export/Word/" + View + ".cshtml";
                using (StringWriter sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
                    ViewData.Model = item;
                    var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);

                    return File(WordHelper.HtmlToWord(sw.GetStringBuilder().ToString()), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", item.FileDownloadName);
                }
            }
            catch (Exception ex)
            {
                return File(WordHelper.HtmlToWord(ex.ToString()), "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            }
        }
    }
}