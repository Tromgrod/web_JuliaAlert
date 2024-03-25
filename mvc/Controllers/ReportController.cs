using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.ComponentModel;
using LIB.Tools.BO;
using LIB.Tools.Security;
using LIB.Tools.Utils;
using LIB.Helpers;
using LIB.AdvancedProperties;
using Weblib.Helpers;
using JuliaAlertLib.BusinessObjects;
using JuliaAlertweblib.Controllers;
using DisplayMode = JuliaAlertLib.BusinessObjects.DisplayMode;

namespace JuliaAlert.Controllers
{
    public class ReportController : ReportObjectController
    {
        public ActionResult View(string Model, string BOLinks = "ItemBase", string NamespaceLinks = "Lib.BusinessObjects", string Ids = "")
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            var sSearch = "";

            var is_search = false;
            var item = (ReportBase)Activator.CreateInstance(Type.GetType("JuliaAlert.Models.Reports." + Model, true));
            var usr = Authentication.GetCurrentUser();

            if (item.HaveAccess(Model) is false)
                return Redirect(URLHelper.GetUrl("Error/AccessDenied"));

            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(item.GetType());
            var search_properties = pss.GetFilterControlProperties(pdc, Authentication.GetCurrentUser());
            var lookup_properties = pss.GetSearchProperties(pdc);
            var DBProperties = Field.LoadByPage(item.GetType().FullName);
            AdvancedProperties properties = null; ;

            if (Session["Display_" + Model] != null)
            {
                properties = pss.GetAvailableProperties(pdc, Authentication.GetCurrentUser(), (List<string>)Session["Display_" + Model]);
            }
            else if (DBProperties != null && DBProperties.Count > 0)
            {
                properties = new AdvancedProperties();
                var tprops = pss.GetAvailableProperties(pdc, Authentication.GetCurrentUser());
                foreach (AdvancedProperty property in tprops)
                {
                    if (DBProperties.Values.Any(f => f.FieldName == property.PropertyName))
                    {
                        var DBField = DBProperties.Values.FirstOrDefault(f => f.FieldName == property.PropertyName);
                        property.Common.DisplayName = DBField.Name;
                        property.Common.PrintName = DBField.PrintName;

                        if (DBField.DisplayModes != null && DBField.DisplayModes.Values.Any(dm => dm == DisplayMode.Simple) && usr.HasAtLeastOnePermission(DBField.Permission) && Session["Display_" + Model] == null)
                        {
                            properties.Add(property);
                        }
                    }
                }
                properties.Sort();
            }
            else
            {
                properties = pss.GetProperties(pdc, Authentication.GetCurrentUser());
            }

            ViewData["Model"] = Model;

            BoAttribute boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];

            if (!string.IsNullOrEmpty(boproperties.DisplayName))
            {
                ViewBag.Title = boproperties.DisplayName;
            }

            ViewData["NewTab"] = boproperties.OpenInNewTab;

            if (!string.IsNullOrEmpty(Ids))
            {
                var IdsList = Ids.Split('-');
                var NamespaceLinksList = NamespaceLinks.Split('-');
                var BOLinksList = BOLinks.Split('-');

                if (IdsList.Length == NamespaceLinksList.Length && NamespaceLinksList.Length == BOLinksList.Length)
                {
                    for (var index = 0; index < IdsList.Length; index++)
                    {
                        var Id = IdsList[index];
                        var NamespaceLink = NamespaceLinksList[index];
                        var BOLink = BOLinksList[index];

                        if (!string.IsNullOrEmpty(Id))
                        {
                            if (!string.IsNullOrEmpty(NamespaceLink) && NamespaceLink != "null")
                            {
                                var LinkItem = (ItemBase)Activator.CreateInstance(Type.GetType(NamespaceLink + "." + BOLink + ", " + NamespaceLink.Split('.')[0], true));
                                LinkItem.Id = Convert.ToInt64(Id);
                                foreach (AdvancedProperty property in lookup_properties)
                                {
                                    if (
                                        (property.Common.EditTemplate == EditTemplates.Parent
                                        || property.Common.EditTemplate == EditTemplates.DropDownParent
                                        || property.Common.EditTemplate == EditTemplates.SelectListParent
                                        || property.Common.EditTemplate == EditTemplates.SelectList)
                                        && property.Type.Name == LinkItem.GetType().Name
                                        )
                                    {
                                        is_search = true;
                                        property.PropertyDescriptor.SetValue(item, LinkItem);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                foreach (AdvancedProperty property in lookup_properties)
                                {
                                    if (property.PropertyName == BOLink)
                                    {
                                        is_search = true;
                                        property.PropertyDescriptor.SetValue(item, Convert.ChangeType(Id, property.Type));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            is_search = item.DefaultReportFilter(is_search);

            if (!string.IsNullOrEmpty(Request.QueryString["s"]))
            {
                sSearch = Request.QueryString["s"];
                is_search = true;
            }

            ViewData["sSearch"] = sSearch;
            sSearch = item.SimpleSearch(sSearch);

            ViewData["Breadcrumbs"] = item.LoadBreadcrumbs();

            var Items = item.PopulateReport(null, is_search ? item : null, 0, boproperties.RecordsPerPage, sSearch, null, null, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum);

            if (Items.Count == 1 && item.ReportSingleItemRedirect(Items.Values.FirstOrDefault(i => i.Id > 0), out string redirectUrl))
                return Redirect(redirectUrl);

            ViewData["TotalColumSum"] = ColumsSum;
            ViewData["Count"] = idisplaytotal;
            ViewData["CountPerPage"] = boproperties.RecordsPerPage;
            ViewData["PageNum"] = 0;
            ViewData["BuildPaginng"] = BuildPaginng(idisplaytotal, boproperties.RecordsPerPage, 0);

            ViewData["DataItems"] = Items;

            var viewMode = ViewMode.Line;

            if (Session[item.GetType() + "ViewMode"] != null)
                viewMode = (ViewMode)Session[item.GetType() + "ViewMode"];

            ViewData["ViewMode"] = viewMode;
            ViewData["Grid_Type"] = item.GetType().AssemblyQualifiedName;
            ViewData["Search_Item"] = item.GetSearchItem(item);

            ViewData["Search_Properties"] = search_properties;
            ViewData["Properties"] = properties;

            ViewData["Report_Name"] = boproperties.DisplayName + item.AdditionalName();

            return View();
        }

        public ActionResult View_Min()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            string Model = Request.Form["ModelName"],
                   BOLink = Request.Form["BOLink"],
                   NamespaceLink = Request.Form["NamespaceLink"],
                   Id = Request.Form["Id"],
                   DateRangeFromStr = Request.Form["DateRangeFrom"],
                   DateRangeToStr = Request.Form["DateRangeTo"];

            BOLink = string.IsNullOrEmpty(BOLink) ? "ItemBase" : BOLink;
            NamespaceLink = string.IsNullOrEmpty(NamespaceLink) ? "Lib.BusinessObjects" : NamespaceLink;
            Id = string.IsNullOrEmpty(Id) ? "" : Id;
            DateTime.TryParse(DateRangeFromStr, out DateTime DateRangeFrom);
            DateTime.TryParse(DateRangeToStr, out DateTime DateRangeTo);

            var is_search = false;
            var item = (ReportBase)Activator.CreateInstance(Type.GetType("JuliaAlert.Models.Reports." + Model, true));
            var usr = Authentication.GetCurrentUser();

            if (item.HaveAccess(Model) is false)
                return Redirect(URLHelper.GetUrl("Error/AccessDenied"));

            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(item.GetType());
            var search_properties = pss.GetFilterControlProperties(pdc, Authentication.GetCurrentUser());
            var lookup_properties = pss.GetSearchProperties(pdc);
            var DBProperties = Field.LoadByPage(item.GetType().FullName);
            AdvancedProperties properties = null; ;

            foreach (AdvancedProperty property in search_properties)
            {
                property.PropertyDescriptor.SetValue(item, property.GetDataProcessor().GetValue(property, "", LIB.AdvancedProperties.DisplayMode.Search));
            }

            if (Session["Display_" + Model] != null)
            {
                properties = pss.GetAvailableProperties(pdc, Authentication.GetCurrentUser(), (List<string>)Session["Display_" + Model]);
            }
            else if (DBProperties != null && DBProperties.Count > 0)
            {
                properties = new AdvancedProperties();
                var tprops = pss.GetAvailableProperties(pdc, Authentication.GetCurrentUser());
                foreach (AdvancedProperty property in tprops)
                {
                    if (DBProperties.Values.Any(f => f.FieldName == property.PropertyName))
                    {
                        var DBField = DBProperties.Values.FirstOrDefault(f => f.FieldName == property.PropertyName);
                        property.Common.DisplayName = DBField.Name;
                        property.Common.PrintName = DBField.PrintName;

                        if (DBField.DisplayModes != null && DBField.DisplayModes.Values.Any(dm => dm == DisplayMode.Simple) && usr.HasAtLeastOnePermission(DBField.Permission) && Session["Display_" + Model] == null)
                        {
                            properties.Add(property);
                        }
                    }
                }
                properties.Sort();
            }
            else
            {
                properties = pss.GetProperties(pdc, Authentication.GetCurrentUser());
            }

            ViewData["Model"] = Model;

            BoAttribute boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];

            if (!string.IsNullOrEmpty(boproperties.DisplayName))
            {
                ViewBag.Title = boproperties.DisplayName;
                ViewData["Report_Name"] = boproperties.DisplayName + (!string.IsNullOrEmpty(DateRangeFromStr) && !string.IsNullOrEmpty(DateRangeToStr) ? $"    ({DateRangeFromStr} - {DateRangeToStr})" : "");
            }

            ViewData["NewTab"] = boproperties.OpenInNewTab;

            if (!string.IsNullOrEmpty(Id))
            {
                if (!string.IsNullOrEmpty(NamespaceLink) && NamespaceLink != "null")
                {
                    var LinkItem = Activator.CreateInstance(Type.GetType(NamespaceLink + "." + BOLink + ", " + NamespaceLink.Split('.')[0], true));
                    ((ItemBase)LinkItem).Id = Convert.ToInt64(Id);
                    foreach (AdvancedProperty property in lookup_properties)
                    {
                        if (
                            (property.Common.EditTemplate == EditTemplates.Parent
                            || property.Common.EditTemplate == EditTemplates.DropDownParent
                            || property.Common.EditTemplate == EditTemplates.SelectListParent
                            || property.Common.EditTemplate == EditTemplates.SelectList)
                            && property.Type.Name == LinkItem.GetType().Name
                            )
                        {
                            is_search = true;
                            property.PropertyDescriptor.SetValue(item, LinkItem);
                            break;
                        }
                    }
                }
                else
                {
                    foreach (AdvancedProperty property in lookup_properties)
                    {
                        if (property.PropertyName == BOLink)
                        {
                            is_search = true;
                            property.PropertyDescriptor.SetValue(item, Convert.ChangeType(Id, property.Type));
                            break;
                        }
                    }
                }
            }

            var Items = item.PopulateReport(null, is_search ? item : null, 0, int.MaxValue, null, null, null, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum);

            if (Items.Count == 1 && item.ReportSingleItemRedirect(Items.Values.FirstOrDefault(i => i.Id > 0), out string redirectUrl))
                return Redirect(redirectUrl);

            ViewData["TotalColumSum"] = ColumsSum;
            ViewData["BuildPaginng"] = BuildPaginng(idisplaytotal, boproperties.RecordsPerPage, 0);

            ViewData["DataItems"] = Items;

            ViewData["Grid_Type"] = item.GetType().AssemblyQualifiedName;
            ViewData["BtnHeaderLink"] = item.GetBtnHeaderLink();

            ViewData["Properties"] = properties;

            return base.View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult OptionsSave(string Model)
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            var item = (ItemBase)Activator.CreateInstance(Type.GetType("JuliaAlert.Models.Reports." + Model, true));
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(item.GetType());
            var properties = pss.GetAvailableProperties(pdc, Authentication.GetCurrentUser());
            var DisplayProperties = new List<string>();
            var ExcellProperties = new List<string>();
            foreach (AdvancedProperty property in properties)
            {
                if (Request.Form["Display_" + property.PropertyName] == "1")
                {
                    DisplayProperties.Add(property.PropertyName);
                }
                if (Request.Form["Excell_" + property.PropertyName] == "1")
                {
                    ExcellProperties.Add(property.PropertyName);
                }
            }
            Session["Excell_" + Model] = ExcellProperties;
            Session["Display_" + Model] = DisplayProperties;

            return this.Json(new RequestResult() { Result = RequestResultType.Success });
        }

        public ActionResult Options(string Model)
        {
            if (!Authentication.CheckUser(this.HttpContext))
            {
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });
            }
            var usr = Authentication.GetCurrentUser();
            var item = (ItemBase)Activator.CreateInstance(Type.GetType("JuliaAlert.Models.Reports." + Model, true));
            var DBProperties = Field.LoadByPage(item.GetType().FullName);
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(item.GetType());
            var properties = pss.GetAvailableProperties(pdc, usr);

            AdvancedProperties DisplayProperties = null;
            AdvancedProperties ExcellProperties = null;

            if (DBProperties != null && DBProperties.Count > 0)
            {
                DisplayProperties = new AdvancedProperties();
                ExcellProperties = new AdvancedProperties();
                foreach (AdvancedProperty property in properties)
                {
                    if (DBProperties.Values.Any(f => f.FieldName == property.PropertyName))
                    {
                        var DBField = DBProperties.Values.FirstOrDefault(f => f.FieldName == property.PropertyName);
                        property.Common.DisplayName = DBField.Name;
                        property.Common.PrintName = DBField.PrintName;

                        if (DBField.DisplayModes != null && DBField.DisplayModes.Values.Any(dm => dm == DisplayMode.Simple) && usr.HasAtLeastOnePermission(DBField.Permission) && Session["Display_" + Model] == null)
                        {
                            DisplayProperties.Add(property);
                        }

                        if (DBField.DisplayModes != null && DBField.DisplayModes.Values.Any(dm => dm == DisplayMode.Excell) && usr.HasAtLeastOnePermission(DBField.Permission) && Session["Excell_" + Model] == null)
                        {
                            ExcellProperties.Add(property);
                        }
                    }
                }
                if (Session["Display_" + Model] != null)
                {
                    DisplayProperties = pss.GetAvailableProperties(pdc, usr, (List<string>)Session["Display_" + Model]);
                }
                if (Session["Excell_" + Model] != null)
                {
                    ExcellProperties = pss.GetAvailableProperties(pdc, usr, (List<string>)Session["Excell_" + Model]);
                }
            }
            else
            {
                DisplayProperties = Session["Display_" + Model] != null ? pss.GetAvailableProperties(pdc, usr, (List<string>)Session["Display_" + Model]) : pss.GetProperties(pdc, usr);

                ExcellProperties = Session["Excell_" + Model] != null ? pss.GetAvailableProperties(pdc, usr, (List<string>)Session["Excell_" + Model]) : pss.GetExcellProperties(pdc, usr);
            }

            ViewData["Properties"] = properties;
            ViewData["DisplayProperties"] = DisplayProperties;
            ViewData["ExcellProperties"] = ExcellProperties;

            return View();
        }

        private string BuildPaginng(long Count, int CountPerPage, int PageNum)
        {
            var pagingstr = "";

            if (Count / CountPerPage <= 6)
            {
                for (int p = 1; p <= Convert.ToInt32(Math.Ceiling((decimal)Count / CountPerPage)); p++)
                {
                    pagingstr += "<a href='#' onclick='return show_report_page(" + (p - 1).ToString() + ")' class='pagination-page" + ((p == PageNum + 1) ? "-active" : "") + "'>" + p.ToString() + "</a>";
                }
            }
            else if (PageNum <= 3 || PageNum >= Convert.ToInt32(Math.Ceiling((decimal)Count / CountPerPage)) - 2)
            {
                for (int p = 1; p <= 3; p++)
                {
                    pagingstr += "<a href='#' onclick='return show_report_page(" + (p - 1).ToString() + ")' class='pagination-page" + ((p == PageNum + 1) ? "-active" : "") + "'>" + p.ToString() + "</a>";
                }

                pagingstr += "<a href='#' onclick='return show_report_page(3)' class='pagination-page'>...</a>";

                for (int p = Convert.ToInt32(Math.Ceiling((decimal)Count / CountPerPage)) - 2; p <= Convert.ToInt32(Math.Ceiling((decimal)Count / CountPerPage)); p++)
                {
                    pagingstr += "<a href='#' onclick='return show_report_page(" + (p - 1).ToString() + ")' class='pagination-page" + ((p == PageNum + 1) ? "-active" : "") + "'>" + p.ToString() + "</a>";
                }
            }
            else
            {
                for (int p = 1; p <= 3; p++)
                {
                    pagingstr += "<a href='#' onclick='return show_report_page(" + (p - 1).ToString() + ")' class='pagination-page" + ((p == PageNum + 1) ? "-active" : "") + "'>" + p.ToString() + "</a>";
                }

                var pstart = PageNum - 1;
                var pend = PageNum + 1;

                if (pstart <= 3)
                {
                    pstart++;
                    pend++;
                }
                else if (pend >= Convert.ToInt32(Math.Ceiling((decimal)Count / CountPerPage)) - 2)
                {
                    pstart--;
                    pend--;
                }

                if (pstart <= 3)
                {
                    pstart++;
                }

                if (pend >= Convert.ToInt32(Math.Ceiling((decimal)Count / CountPerPage)) - 2)
                {
                    pend--;
                }

                if (pstart != 4)
                {
                    pagingstr += "<a href='#' onclick='return show_report_page(3)' class='pagination-page'>...</a>";
                }

                for (int p = pstart; p <= pend; p++)
                {
                    pagingstr += "<a href='#' onclick='return show_report_page(" + (p - 1).ToString() + ")' class='pagination-page" + ((p == PageNum + 1) ? "-active" : "") + "'>" + p.ToString() + "</a>";
                }

                if (pend != Convert.ToInt32(Math.Ceiling((decimal)Count / CountPerPage)) - 3)
                {
                    pagingstr += "<a href='#' onclick='return show_report_page(" + (Convert.ToInt32(Math.Ceiling((decimal)Count / CountPerPage)) - 2).ToString() + ")' ' class='pagination-page'>...</a>";
                }

                for (int p = Convert.ToInt32(Math.Ceiling((decimal)Count / CountPerPage)) - 2; p <= Convert.ToInt32(Math.Ceiling((decimal)Count / CountPerPage)); p++)
                {
                    pagingstr += "<a href='#' onclick='return show_report_page(" + (p - 1).ToString() + ")' class='pagination-page" + ((p == PageNum + 1) ? "-active" : "") + "'>" + p.ToString() + "</a>";
                }
            }

            return pagingstr;
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Search()
        {
            if (!Authentication.CheckUser(this.HttpContext))
            {
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });
            }
            try
            {
                var item = (ItemBase)Activator.CreateInstance(Type.GetType(Request.Form["bo_type"], true));

                var sSearch = "";

                var usr = Authentication.GetCurrentUser();

                if (item.HaveAccess() is false)
                    return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

                var pss = new PropertySorter();
                var pdc = TypeDescriptor.GetProperties(item);
                var search_properties = pss.GetSearchProperties(pdc);

                foreach (AdvancedProperty property in search_properties)
                {
                    property.PropertyDescriptor.SetValue(item, property.GetDataProcessor().GetValue(property, "", LIB.AdvancedProperties.DisplayMode.Search));
                }

                item.DefaultReportFilter(false);

                if (!string.IsNullOrEmpty(Request.Form["sSearch"]))
                    sSearch = Request.Form["sSearch"];

                sSearch = item.SimpleSearch(sSearch);

                var DBProperties = Field.LoadByPage(item.GetType().FullName);
                AdvancedProperties properties = null;

                if (Session["Display_" + item.GetType().Name] != null)
                {
                    properties = pss.GetAvailableProperties(pdc, Authentication.GetCurrentUser(), (List<string>)Session["Display_" + item.GetType().Name]);
                }
                else if (DBProperties != null && DBProperties.Count > 0)
                {
                    properties = new AdvancedProperties();
                    var tprops = pss.GetAvailableProperties(pdc, Authentication.GetCurrentUser());
                    foreach (AdvancedProperty property in tprops)
                    {
                        if (DBProperties.Values.Any(f => f.FieldName == property.PropertyName))
                        {
                            var DBField = DBProperties.Values.FirstOrDefault(f => f.FieldName == property.PropertyName);
                            property.Common.DisplayName = DBField.Name;
                            property.Common.PrintName = DBField.PrintName;

                            if (DBField.DisplayModes != null && DBField.DisplayModes.Values.Any(dm => dm == JuliaAlertLib.BusinessObjects.DisplayMode.Simple) && usr.HasAtLeastOnePermission(DBField.Permission) && Session["Display_" + item.GetType().Name] == null)
                            {
                                properties.Add(property);
                            }
                        }
                    }
                    properties.Sort();
                }
                else
                {
                    properties = pss.GetProperties(pdc, Authentication.GetCurrentUser());
                }

                BoAttribute boproperties = null;
                if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                {
                    boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];

                    ViewData["NewTab"] = boproperties.OpenInNewTab;
                }
                else
                {
                    ViewData["NewTab"] = false;
                }

                var CountPerPage = !string.IsNullOrEmpty(Request.Form["CountPerPage"]) ? Convert.ToInt32(Request.Form["CountPerPage"]) : boproperties.RecordsPerPage;
                var PageNum = Convert.ToInt32(Request.Form["PageNum"]);
                var SortParameters = new List<SortParameter>();

                if (!string.IsNullOrEmpty(Request.Form["SortCol"]))
                {
                    foreach (AdvancedProperty property in properties)
                    {
                        if (property.PropertyName == Request.Form["SortCol"])
                        {
                            var PropertyName = property.PropertyName;
                            if (property.Type.BaseType == typeof(ItemBase)
                                || (property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType == typeof(ItemBase))
                                || (property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType.BaseType != null && property.Type.BaseType.BaseType.BaseType == typeof(ItemBase))
                                )
                            {
                                PropertyName += "Id";
                            }
                            SortParameters.Add(new SortParameter() { Direction = Request.Form["SortDir"], Field = PropertyName });
                            break;
                        }
                    }
                }

                ViewData["SortCol"] = Request.Form["SortCol"];
                ViewData["SortDir"] = Request.Form["SortDir"];

                Enum.TryParse(Request.Form["ViewMode"], out ViewMode viewMode);

                Session[item.GetType() + "ViewMode"] = viewMode;

                var Items = item.PopulateReport(null, item, PageNum * CountPerPage, CountPerPage, sSearch, SortParameters, null, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum);

                ViewData["TotalColumSum"] = ColumsSum;
                ViewData["Count"] = idisplaytotal;
                ViewData["CountPerPage"] = CountPerPage;
                ViewData["PageNum"] = PageNum;
                ViewData["BuildPaginng"] = BuildPaginng(idisplaytotal, CountPerPage, PageNum);

                ViewData["DataItems"] = Items;

                ViewData["ViewMode"] = viewMode;
                ViewData["Grid_Type"] = item.GetType().AssemblyQualifiedName;
                ViewData["Search_Item"] = item;

                ViewData["Search_Properties"] = search_properties;
                ViewData["Properties"] = properties;

                var viewName = "~/Views/Report/Search.cshtml";

                var Data = new Dictionary<string, object>();

                using (StringWriter sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindView(ControllerContext, viewName, null);

                    var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);

                    Data["Search_Result"] = sw.ToString();
                }

                return this.Json(new RequestResult() { Result = RequestResultType.Success, Data = Data });
            }
            catch (Exception ex)
            {
                return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = ex.ToString() });
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public FileResult ExportExcell()
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
                return File(Encoding.UTF8.GetBytes("Authentification Error"), ExcelExportHelper.ExcelContentType, "Error.xlsx");
            }

            var sSearch = "";
            var item = (ItemBase)Activator.CreateInstance(Type.GetType(Request.Form["bo_type"], true));
            var usr = Authentication.GetCurrentUser();

            if (item.HaveAccess() is false)
                return File("", "application/ms-excel");

            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(item);
            var search_properties = pss.GetSearchProperties(pdc, Authentication.GetCurrentUser());

            foreach (AdvancedProperty property in search_properties)
            {
                property.PropertyDescriptor.SetValue(item, property.GetDataProcessor().GetValue(property, "", LIB.AdvancedProperties.DisplayMode.PrintSearch));
            }

            item.DefaultReportFilter(false);

            if (!string.IsNullOrEmpty(Request.Form["sSearch"]))
            {
                sSearch = Request.Form["sSearch"];
            }

            sSearch = item.SimpleSearch(sSearch);

            var DBProperties = Field.LoadByPage(item.GetType().FullName);
            AdvancedProperties properties = null; ;

            if (Session["Display_" + item.GetType().Name] != null)
            {
                properties = pss.GetAvailableProperties(pdc, Authentication.GetCurrentUser(), (List<string>)Session["Excell_" + item.GetType().Name]);
            }
            else if (DBProperties != null && DBProperties.Count > 0)
            {
                properties = new AdvancedProperties();
                var tprops = pss.GetAvailableProperties(pdc, Authentication.GetCurrentUser());
                foreach (AdvancedProperty property in tprops)
                {
                    if (DBProperties.Values.Any(f => f.FieldName == property.PropertyName))
                    {
                        var DBField = DBProperties.Values.FirstOrDefault(f => f.FieldName == property.PropertyName);
                        property.Common.DisplayName = DBField.Name;
                        property.Common.PrintName = DBField.PrintName;

                        if (DBField.DisplayModes != null && DBField.DisplayModes.Values.Any(dm => dm == DisplayMode.Excell) && usr.HasAtLeastOnePermission(DBField.Permission) && Session["Excell_" + item.GetType().Name] == null)
                        {
                            properties.Add(property);
                        }
                    }
                }
                properties.Sort();
            }
            else
            {
                properties = pss.GetExcellProperties(pdc, Authentication.GetCurrentUser());
            }

            BoAttribute boproperties = null;
            if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
            {
                boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
                if (boproperties != null && !string.IsNullOrEmpty(boproperties.DisplayName))
                {
                    ViewData["Report_Name"] = boproperties.DisplayName;
                }
            }

            var CountPerPage = int.MaxValue;
            var PageNum = 0;
            var SortParameters = new List<SortParameter>();

            if (!string.IsNullOrEmpty(Request.Form["SortCol"]))
            {
                foreach (AdvancedProperty property in properties)
                {
                    if (property.PropertyName == Request.Form["SortCol"])
                    {
                        var PropertyName = property.PropertyName;
                        if (property.Type.BaseType == typeof(ItemBase)
                            || (property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType == typeof(ItemBase))
                            || (property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType.BaseType != null && property.Type.BaseType.BaseType.BaseType == typeof(ItemBase))
                            )
                        {
                            PropertyName += "Id";
                        }
                        SortParameters.Add(new SortParameter() { Direction = Request.Form["SortDir"], Field = PropertyName });
                        break;
                    }
                }
            }

            var Items = item.PopulateReport(null, item, PageNum * CountPerPage, CountPerPage, sSearch, SortParameters, null, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum);

            return File(ExcelExportHelper.ExportExcel(Items, ColumsSum, properties, ViewData["Report_Name"].ToString()), ExcelExportHelper.ExcelContentType, ViewData["Report_Name"] + ".xlsx");
        }

        public enum ViewMode
        {
            Line,
            Square
        }
    }
}