namespace Weblib.Controllers
{
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Tools.BO;
    using Weblib.Helpers;
    using System.Collections.Generic;
    using LIB.Tools.Security;
    using LIB.Tools.Controls;
    using System.IO;
    using LIB.Tools.Utils;
    using System.Web;
    using LIB.Helpers;

    public class DataProcessor : BaseController
    {
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Load(string BO, string Namespace, string BOLink = "ItemBase", string NamespaceLink = "Lib.BusinessObjects", string Id = "")
        {
            if (Authentication.CheckUser(this.HttpContext))
            {
                var item = Activator.CreateInstance(Type.GetType(Namespace + "." + BO + ", " + Namespace.Split('.')[0], true));
                BoAttribute boproperties = null;
                if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                {
                    boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
                }

                var Module = "ControlPanel";
                if (Session[SessionItems.Module] != null)
                    Module = Session[SessionItems.Module].ToString();

                if (Module == "ControlPanel")
                {
                    System.Web.HttpContext.Current.Items["ControlPanel"] = true;
                }
                else
                {
                    System.Web.HttpContext.Current.Items["SystemManagement"] = true;
                }

                if (Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ReadAccess))
                {
                    var pss = new PropertySorter();
                    var pdc = TypeDescriptor.GetProperties(item.GetType());
                    var properties = pss.GetProperties(pdc, Authentication.GetCurrentUser());

                    var is_search = false;
                    if (!string.IsNullOrEmpty(Id))
                    {

                        var LinkItem = Activator.CreateInstance(Type.GetType(NamespaceLink + "." + BOLink + ", " + NamespaceLink.Split('.')[0], true));
                        ((ItemBase)LinkItem).Id = Convert.ToInt64(Id);
                        foreach (AdvancedProperty property in properties)
                        {
                            if (
                                (property.Common.EditTemplate == EditTemplates.Parent
                                || property.Common.EditTemplate == EditTemplates.DropDownParent
                                || property.Common.EditTemplate == EditTemplates.SelectListParent)
                                && property.Type == LinkItem.GetType()
                                )
                            {
                                is_search = true;
                                property.PropertyDescriptor.SetValue(item, LinkItem);
                                break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Form["isFileter"]))
                    {
                        is_search = true;

                        var search_properties = pss.GetSearchProperties(pdc);

                        foreach (AdvancedProperty property in search_properties)
                        {
                            property.PropertyDescriptor.SetValue(item, property.GetDataProcessor().GetValue(property, "", DisplayMode.Search));
                        }
                    }

                    var iDisplayStart = Convert.ToInt32(Request.Form["iDisplayStart"]);
                    var iDisplayLength = Convert.ToInt32(Request.Form["iDisplayLength"]);
                    var sSearch = Request.Form["sSearch"];
                    var sEcho = Convert.ToInt32(Request.Form["sEcho"]);
                    var SortParameters = new List<SortParameter>();

                    if (!string.IsNullOrEmpty(Request.Form["iSortCol_0"]))
                    {
                        var iSortingCols = Convert.ToInt32(Request.Form["iSortingCols"]);
                        for (var i = 0; i < iSortingCols; i++)
                        {
                            var iSortingCol = Convert.ToInt32(Request.Form["iSortCol_" + i.ToString()]);
                            if (Request.Form["bSortable_" + iSortingCol.ToString()] == "true")
                            {
                                if (iSortingCol == 1)
                                {
                                    SortParameters.Add(new SortParameter() { Direction = Request.Form["sSortDir_" + i.ToString()], Field = item.GetType().Name + "Id" });
                                }
                                else if (iSortingCol == properties.Count + 2)
                                {
                                    SortParameters.Add(new SortParameter() { Direction = Request.Form["sSortDir_" + i.ToString()], Field = "DateCreated" });
                                }
                                else if (iSortingCol == properties.Count + 3)
                                {
                                    SortParameters.Add(new SortParameter() { Direction = Request.Form["sSortDir_" + i.ToString()], Field = "CreatedBy" });
                                }
                                else
                                {
                                    var PropertyName = properties[iSortingCol - 2].PropertyName;
                                    if (properties[iSortingCol - 2].Type.BaseType == typeof(ItemBase)
                                        || (properties[iSortingCol - 2].Type.BaseType.BaseType != null && properties[iSortingCol - 2].Type.BaseType.BaseType == typeof(ItemBase))
                                        || (properties[iSortingCol - 2].Type.BaseType.BaseType != null && properties[iSortingCol - 2].Type.BaseType.BaseType != null && properties[iSortingCol - 2].Type.BaseType.BaseType.BaseType != null && properties[iSortingCol - 2].Type.BaseType.BaseType.BaseType == typeof(ItemBase))
                                        )
                                    {
                                        PropertyName += "Id";
                                    }
                                    SortParameters.Add(new SortParameter() { Direction = Request.Form["sSortDir_" + i.ToString()], Field = PropertyName });
                                }
                            }
                        }
                    }

                    long totalDisplayItems = 0;
                    var DataItems = new Dictionary<long, ItemBase>();

                    if (boproperties.LoadFromDb)
                    {
                        DataItems = ((ItemBase)(item)).PopulateGridTools(null, (is_search ? (ItemBase)(item) : null), iDisplayStart, iDisplayLength, sSearch, SortParameters, false, null, out totalDisplayItems);
                    }
                    else
                    {
                        DataItems = ((ItemBase)(item)).Populate(SortParameters);

                        totalDisplayItems = DataItems.Count;
                    }

                    var Output = new DataTableOutput();

                    Output.sEcho = sEcho;
                    Output.iTotalRecords = totalDisplayItems;
                    Output.iTotalDisplayRecords = totalDisplayItems;
                    Output.aaData = new List<List<string>>();

                    HtmlHelper helper = new HtmlHelper(new ViewContext(ControllerContext, new WebFormView(ControllerContext, "Index"), new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());

                    foreach (var litem in DataItems.Values)
                    {
                        Output.aaData.Add(new List<string>());
                        Output.aaData[Output.aaData.Count - 1].Add(litem.Id.ToString());
                        Output.aaData[Output.aaData.Count - 1].Add(litem.Id.ToString());
                        foreach (AdvancedProperty property in properties)
                        {
                            var renderString = helper.Action(property.ControlView, property.Control, new { model = property.GetDataProcessor().SetValue(property.PropertyDescriptor.GetValue(litem), property, litem) });

                            Output.aaData[Output.aaData.Count - 1].Add(renderString.ToHtmlString());
                        }
                        Output.aaData[Output.aaData.Count - 1].Add(litem.DateCreated != DateTime.MinValue ? litem.DateCreated.ToString("dd/MM/yyyy HH:mm") : DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                        Output.aaData[Output.aaData.Count - 1].Add(litem.CreatedBy != null ? "<a href='" + URLHelper.GetUrl(Module + "/Profile/" + litem.CreatedBy.Id.ToString()) + "'>" + litem.CreatedBy.Login + "</a>" : "");

                        if (item.GetType() == typeof(User) && !Authentication.GetCurrentUser().HasAtLeastOnePermission(((User)litem).Role.RoleAccessPermission))
                        {
                            Output.aaData[Output.aaData.Count - 1].Add("");
                            Output.aaData[Output.aaData.Count - 1].Add("");
                            Output.aaData[Output.aaData.Count - 1].Add("");
                        }
                        else
                        {
                            Output.aaData[Output.aaData.Count - 1].Add(litem.Id.ToString());
                            Output.aaData[Output.aaData.Count - 1].Add(litem.GetName());
                            Output.aaData[Output.aaData.Count - 1].Add(litem.GetName());
                        }
                    }
                    var jsonResult = Json(Output, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
            }

            return this.Json(new RequestResult() { Message = "SeeionTimeOut", Result = RequestResultType.Reload });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Insert()
        {
            if (Authentication.CheckUser(this.HttpContext))
            {
                var item = (ItemBase)Activator.CreateInstance(Type.GetType(Request.Form["bo_type"], true));

                var pss = new PropertySorter();
                var pdc = TypeDescriptor.GetProperties(item);
                var properties = pss.GetProperties(pdc);

                foreach (AdvancedProperty property in properties)
                {
                    property.PropertyDescriptor.SetValue(item, property.GetDataProcessor().GetValue(property));
                }

                return this.Json(item.ActionInsert(item));
            }

            return this.Json(new RequestResult() { Message = "SeeionTimeOut", Result = RequestResultType.Reload });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update()
        {

            if (Authentication.CheckUser(this.HttpContext))
            {
                if (!string.IsNullOrEmpty(Request.Form["id"]))
                {
                    var item = (ItemBase)Activator.CreateInstance(Type.GetType(Request.Form["bo_type"], true));

                    item.Id = Convert.ToInt64(Request.Form["id"]);
                    var pss = new PropertySorter();
                    var pdc = TypeDescriptor.GetProperties(item);

                    AdvancedProperties properties = null;
                    if (!string.IsNullOrEmpty(Request.Form["simple"]))
                    {
                        properties = pss.GetProperties(pdc, Authentication.GetCurrentUser(), DisplayMode.Simple);
                    }
                    else
                    {
                        properties = pss.GetProperties(pdc, Authentication.GetCurrentUser(), DisplayMode.Advanced);
                    }

                    foreach (AdvancedProperty property in properties)
                    {
                        property.PropertyDescriptor.SetValue(item, property.GetDataProcessor().GetValue(property));
                    }

                    return this.Json(item.ActionUpdate(item, !string.IsNullOrEmpty(Request.Form["simple"]) ? DisplayMode.Simple : DisplayMode.Advanced));
                }
                return this.Json(new RequestResult() { Message = "WrongParameters", Result = RequestResultType.Fail });
            }

            return this.Json(new RequestResult() { Message = "SeeionTimeOut", Result = RequestResultType.Reload });

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Clone()
        {
            if (Authentication.CheckUser(this.HttpContext))
            {
                var Module = "ControlPanel";
                if (Session[SessionItems.Module] != null)
                    Module = Session[SessionItems.Module].ToString();

                if (Module == "ControlPanel")
                {
                    System.Web.HttpContext.Current.Items["ControlPanel"] = true;
                }
                else
                {
                    System.Web.HttpContext.Current.Items["SystemManagement"] = true;
                }

                if (!string.IsNullOrEmpty(Request.Form["id"]) && !string.IsNullOrEmpty(Request.Form["name"]))
                {
                    var item = (ItemBase)Activator.CreateInstance(Type.GetType(Request.Form["bo_type"], true));

                    item.Id = Convert.ToInt64(Request.Form["id"]);
                    item = item.PopulateOne(item);
                    if (item != null)
                    {
                        item.Id = 0;
                        var comment = "Copied from \"" + item.GetName() + "\"";
                        item.SetName(Request.Form["name"]);

                        item.Insert(item, comment);

                        return this.Json(new RequestResult() { Message = URLHelper.GetUrl(Module + "/EditItem/" + item.GetType().Name + "/" + HttpUtility.UrlEncode(item.GetType().Namespace) + "/" + item.Id.ToString()), Result = RequestResultType.Success });
                    }
                }
                return this.Json(new RequestResult() { Message = "WrongParameters", Result = RequestResultType.Fail });
            }

            return this.Json(new RequestResult() { Message = "SeeionTimeOut", Result = RequestResultType.Reload });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Delete()
        {
            if (Authentication.CheckUser(this.HttpContext))
            {
                if (!string.IsNullOrEmpty(Request.Form["id"]))
                {
                    var item = (ItemBase)Activator.CreateInstance(Type.GetType(Request.Form["bo_type"], true));
                    var sid = Request.Form["id"];
                    var itemsToDelete = new Dictionary<long, ItemBase>();
                    if (sid != "all")
                    {
                        item.Id = Convert.ToInt64(sid);
                        var pss = new PropertySorter();
                        var pdc = TypeDescriptor.GetProperties(item);
                        var properties = pss.GetProperties(pdc);

                        foreach (AdvancedProperty property in properties)
                        {
                            property.PropertyDescriptor.SetValue(item, property.GetDataProcessor().GetValue(property));
                        }

                        itemsToDelete.Add(item.Id, item);
                    }
                    else
                    {
                        itemsToDelete = item.Populate();
                    }

                    var result = item.Delete(itemsToDelete);
                    if (result)
                    {
                        if (sid != "all")
                        {
                            return this.Json(new RequestResult() { Message = "OK", Result = RequestResultType.Success });
                        }
                        else
                        {
                            return this.Json(new RequestResult() { Message = "OK", Result = RequestResultType.Reload });
                        }
                    }
                    else
                    {
                        return this.Json(new RequestResult() { Message = "", Result = RequestResultType.Alert });
                    }
                }
                return this.Json(new RequestResult() { Message = "WrongParameters", Result = RequestResultType.Fail });
            }

            return this.Json(new RequestResult() { Message = "SeeionTimeOut", Result = RequestResultType.Reload });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Print()
        {
            if (Authentication.CheckUser(this.HttpContext))
            {
                if (!string.IsNullOrEmpty(Request.Form["id"]))
                {
                    var item = (ItemBase)Activator.CreateInstance(Type.GetType(Request.Form["bo_type"], true));

                    item.Id = Convert.ToInt64(Request.Form["id"]);
                    ViewData["BOType"] = item.GetType();
                    var pss = new PropertySorter();
                    var pdc = TypeDescriptor.GetProperties(item.GetType());
                    var properties = pss.GetAdvancedProperties(pdc, Authentication.GetCurrentUser());
                    ViewData["Properties"] = properties;

                    BoAttribute boproperties = null;
                    if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                    {
                        boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
                    }
                    ViewData["BOProperties"] = boproperties;

                    item = ((ItemBase)(item)).PopulateOne((ItemBase)(item));

                    return this.View("~/Views/ControlPanel/PrintItem.cshtml", item);
                }
                else
                {
                    var item = (ItemBase)Activator.CreateInstance(Type.GetType(Request.Form["bo_type"], true));

                    ViewData["BOType"] = item.GetType();
                    var pss = new PropertySorter();
                    var pdc = TypeDescriptor.GetProperties(item.GetType());
                    var properties = pss.GetProperties(pdc, Authentication.GetCurrentUser());
                    ViewData["Properties"] = properties;

                    BoAttribute boproperties = null;
                    if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                    {
                        boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
                    }
                    ViewData["BOProperties"] = boproperties;

                    var is_search = false;

                    if (!string.IsNullOrEmpty(Request.Form["isFileter"]))
                    {
                        is_search = true;

                        var search_properties = pss.GetSearchProperties(pdc);

                        foreach (AdvancedProperty property in search_properties)
                        {
                            property.PropertyDescriptor.SetValue(item, property.GetDataProcessor().GetValue(property));
                        }
                    }

                    var DataItems = item.Populate(is_search ? item : null);

                    return this.View("~/Views/ControlPanel/Print.cshtml", DataItems);
                }
            }

            return this.Json(new RequestResult() { Message = "SeeionTimeOut", Result = RequestResultType.Reload });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExportExcell()
        {
            if (Authentication.CheckUser(this.HttpContext))
            {
                if (!string.IsNullOrEmpty(Request.Form["id"]))
                {
                    var item = (ItemBase)Activator.CreateInstance(Type.GetType(Request.Form["bo_type"], true));

                    item.Id = Convert.ToInt64(Request.Form["id"]);
                    ViewData["BOType"] = item.GetType();
                    var pss = new PropertySorter();
                    var pdc = TypeDescriptor.GetProperties(item.GetType());
                    var properties = pss.GetAdvancedProperties(pdc, Authentication.GetCurrentUser());
                    ViewData["Properties"] = properties;

                    BoAttribute boproperties = null;
                    if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                    {
                        boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
                    }
                    ViewData["BOProperties"] = boproperties;

                    item = item.PopulateOne(item);

                    var DataItems = new Dictionary<long, ItemBase>
                    {
                        { item.Id, item }
                    };

                    return File(ExcelExportHelper.ExportExcel(DataItems, properties, item.GetType().Name, true), ExcelExportHelper.ExcelContentType, item.GetType().Name + ".xlsx");
                }
                else
                {
                    var item = (ItemBase)Activator.CreateInstance(Type.GetType(Request.Form["bo_type"], true));

                    ViewData["BOType"] = item.GetType();
                    var pss = new PropertySorter();
                    var pdc = TypeDescriptor.GetProperties(item.GetType());
                    var properties = pss.GetProperties(pdc, Authentication.GetCurrentUser());
                    ViewData["Properties"] = properties;

                    BoAttribute boproperties = null;
                    if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                    {
                        boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
                    }
                    ViewData["BOProperties"] = boproperties;

                    var is_search = false;

                    if (!string.IsNullOrEmpty(Request.Form["isFileter"]))
                    {
                        is_search = true;

                        var search_properties = pss.GetSearchProperties(pdc);

                        foreach (AdvancedProperty property in search_properties)
                        {
                            property.PropertyDescriptor.SetValue(item, property.GetDataProcessor().GetValue(property));
                        }
                    }

                    var DataItems = item.Populate(is_search ? item : null);

                    return File(ExcelExportHelper.ExportExcel(DataItems, properties, item.GetType().Name, true), ExcelExportHelper.ExcelContentType, item.GetType().Name + ".xlsx");
                }
            }

            return File("", "application/ms-excel");
        }
    }
}
