// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagement.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the SystemManagement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertweblib.Controllers
{
    using System.Web.Mvc;

    using JuliaAlertLib.BusinessObjects;
    using Weblib.Models.Common;
    using System;
    using LIB.Tools.BO;
    using LIB.AdvancedProperties;
    using System.ComponentModel;
    using LIB.Tools.Security;
    using LIB.Tools.Utils;
    using System.Web;
    using LIB.BusinessObjects;
    using System.Collections.Generic;
    using LIB.Tools.Revisions;

    /// <summary>
    /// The SystemManagement controller.
    /// </summary>
    public class SystemManagement : Weblib.Controllers.SystemManagement
    {
        public ActionResult Edit(string BO, string Namespace, string BOLink = "ItemBase", string NamespaceLink = "Lib.BusinessObjects", string Id = "")
        {
            if (!Authentication.CheckUser(this.HttpContext, Modulesenum.SMI))
            {
                return new RedirectResult(Config.GetConfigValue("SMILoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (!LIB.Tools.Security.Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.SMIAccess))
            {
                return new RedirectResult(Config.GetConfigValue("SMILoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (string.IsNullOrEmpty(Namespace))
            {
                return new RedirectResult(LIB.Tools.Utils.URLHelper.GetUrl("SystemManagement/DashBoard"));
            }
            var item = Activator.CreateInstance(Type.GetType(Namespace + "." + BO + ", " + Namespace.Split('.')[0], true));

            ViewData["BOType"] = item.GetType();

            BoAttribute boproperties = null;
            if (item.GetType().GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true).Length > 0)
            {
                boproperties = (LIB.AdvancedProperties.BoAttribute)item.GetType().GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true)[0];
            }

            if (!LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ReadAccess))
            {
                return View("NoAccess");
            }
            
            if (boproperties.ModulesAccess != 0 && (boproperties.ModulesAccess & (long)Modulesenum.SMI) != (long)Modulesenum.SMI)
            {
                return View("NoAccess");
            }

            ViewData["BOLink"] = BOLink;
            ViewData["NamespaceLink"] = HttpUtility.UrlEncode(NamespaceLink);
            ViewData["Id"] = Id;
            ViewBag.Title = Config.GetConfigValue("SiteNameAbbr") + ": " + boproperties.DisplayName;

            if (!string.IsNullOrEmpty(Id))
            {
                var LinkItem = Activator.CreateInstance(Type.GetType(NamespaceLink + "." + BOLink + ", " + NamespaceLink.Split('.')[0], true));
                ((ItemBase)LinkItem).Id = Convert.ToInt64(Id);
                LinkItem = ((ItemBase)(LinkItem)).PopulateOne((ItemBase)LinkItem, true);
                ViewData["LinkItem"] = LinkItem;

                BoAttribute BOPropertiesLinked = null;
                if (LinkItem.GetType().GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true).Length > 0)
                {
                    BOPropertiesLinked = (LIB.AdvancedProperties.BoAttribute)LinkItem.GetType().GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true)[0];
                }
                ViewData["BOPropertiesLinked"] = BOPropertiesLinked;
                ViewData["BOLinkType"] = LinkItem.GetType();
            }
            else
            {
                ViewData["LinkItem"] = null;
            }

            if (boproperties.AllowCreate && LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.CreateAccess))
            {
                var add_button = new ButtonModel();
                add_button.Name = "add_new";
                if (boproperties != null)
                {
                    add_button.Text = LIB.Tools.Utils.Translate.GetTranslatedValue("Add", "AdminArea") + " " + boproperties.SingleName;
                }
                else
                {
                    add_button.Text = LIB.Tools.Utils.Translate.GetTranslatedValue("Add", "AdminArea") + " " + item.GetType().Name;
                }
                add_button.Class = "btn btn-success btn-add fancybox.ajax";
                var url = "SystemManagement/CreateItem/" + item.GetType().Name + "/" + HttpUtility.UrlEncode(item.GetType().Namespace);
                if (!string.IsNullOrEmpty(Id))
                {
                    url += "/" + BOLink + "/" + HttpUtility.UrlEncode(NamespaceLink) + "/" + Id;
                }
                add_button.Href = LIB.Tools.Utils.URLHelper.GetUrl(url);
                add_button.Icon = "plus";
                ViewData["Add_Button"] = add_button;
            }
            else
            {
                ViewData["Add_Button"] = null;
            }

            if (LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.PrintAccess))
            {
                var print_button = new ButtonModel();
                print_button.Name = "print";
                print_button.Text = LIB.Tools.Utils.Translate.GetTranslatedValue("Print", "AdminArea");
                print_button.Class = "btn btn-default btn-print";
                print_button.Icon = "print";
                print_button.Action = "do_print_class()";
                ViewData["Print_Button"] = print_button;
            }
            else
            {
                ViewData["Print_Button"] = null;
            }

            if (LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ExportAccess))
            {
                var export_button = new ButtonModel();
                export_button.Name = "export";
                export_button.Text = LIB.Tools.Utils.Translate.GetTranslatedValue("Export", "AdminArea");
                export_button.Class = "btn btn-primary btn-export";
                export_button.Icon = "download";
                ViewData["Export_Button"] = export_button;
            }
            else
            {
                ViewData["Export_Button"] = null;
            }

            if (boproperties.AllowImport && LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ImportAccess))
            {
                var import_button = new ButtonModel();
                import_button.Name = "import";
                import_button.Text = "Import";
                import_button.Class = "btn btn-warning btn-import";
                import_button.Icon = "file-excel-o";
                ViewData["Import_Button"] = import_button;
            }
            else
            {
                ViewData["Import_Button"] = null;
            }

            if (boproperties.AllowDeleteAll && LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.DeleteAllAccess))
            {
                var deleteall_button = new ButtonModel();
                deleteall_button.Name = "deleteall";
                deleteall_button.Text = "Delete All";
                deleteall_button.Class = "btn btn-danger btn-deleteall";
                deleteall_button.Icon = "trash-o";
                deleteall_button.Action = "delete_all_from_grid('" + boproperties.DisplayName + "')";
                ViewData["DeleteAll_Button"] = deleteall_button;
            }
            else
            {
                ViewData["DeleteAll_Button"] = null;
            }

            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(item.GetType());
            var properties = pss.GetProperties(pdc, Authentication.GetCurrentUser());
            var advanced_properties = pss.GetAdvancedProperties(pdc, Authentication.GetCurrentUser());
            var search_properties = pss.GetFilterControlProperties(pdc, Authentication.GetCurrentUser());

            ViewData["Grid_Type"] = item.GetType().AssemblyQualifiedName;
            ViewData["BOProperties"] = boproperties;
            ((ItemBase)item).Id = -1;
            ViewData["New_Item"] = item;

            ViewData["Properties"] = properties;
            ViewData["AdvancedProperties"] = advanced_properties;
            ViewData["SearchProperties"] = search_properties;
            if (search_properties.Count > 0)
            {
                var search_item = Activator.CreateInstance(Type.GetType(Namespace + "." + BO + ", " + Namespace.Split('.')[0], true));
                ViewData["Search_Item"] = search_item;
            }

            return View();
        }

        public ActionResult EditItem(string BO, string Namespace, long Id)
        {
            if (!Authentication.CheckUser(this.HttpContext, Modulesenum.SMI))
            {
                return new RedirectResult(Config.GetConfigValue("SMILoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (!LIB.Tools.Security.Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.SMIAccess))
            {
                return new RedirectResult(Config.GetConfigValue("SMILoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }

            var item = Activator.CreateInstance(Type.GetType(Namespace + "." + BO + ", " + Namespace.Split('.')[0], true));
            ViewData["BOType"] = item.GetType();
            ViewData["Back_Link"] = LIB.Tools.Utils.URLHelper.GetUrl("SystemManagement/Edit/" + item.GetType().Name + "/" + HttpUtility.UrlEncode(item.GetType().Namespace));

            BoAttribute boproperties = null;
            if (item.GetType().GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true).Length > 0)
            {
                boproperties = (LIB.AdvancedProperties.BoAttribute)item.GetType().GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true)[0];
            }

            ViewData["BOProperties"] = boproperties;

            if (!LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ReadAccess))
            {
                return View("NoAccess");
            }
            /*
            if (boproperties.ModulesAccess != 0 && (boproperties.ModulesAccess & (long)Modulesenum.SMI) != (long)Modulesenum.SMI)
            {
                return View("NoAccess");
            }
             * */

            if (!string.IsNullOrEmpty(Request.QueryString["BOLink"]))
            {

                var BOLink = Request.QueryString["BOLink"];
                var NamespaceLink = Request.QueryString["NamespaceLink"];
                var IdLink = Convert.ToInt64(Request.QueryString["IdLink"]);

                var LinkItem = Activator.CreateInstance(Type.GetType(NamespaceLink + "." + BOLink + ", " + NamespaceLink.Split('.')[0], true));
                ((ItemBase)LinkItem).Id = IdLink;
                LinkItem = ((ItemBase)(LinkItem)).PopulateOne((ItemBase)LinkItem, true);
                ViewData["LinkItem"] = LinkItem;
                ViewData["BOLinkType"] = LinkItem.GetType();

                BoAttribute BOPropertiesLinked = null;
                if (LinkItem.GetType().GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true).Length > 0)
                {
                    BOPropertiesLinked = (LIB.AdvancedProperties.BoAttribute)LinkItem.GetType().GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true)[0];
                }
                ViewData["BOPropertiesLinked"] = BOPropertiesLinked;

                ViewData["BOLink"] = BOLink;
                ViewData["NamespaceLink"] = HttpUtility.UrlEncode(NamespaceLink);
                ViewData["Id"] = IdLink.ToString();
                ViewData["Back_Link"] = LIB.Tools.Utils.URLHelper.GetUrl("SystemManagement/Edit/" + item.GetType().Name + "/" + HttpUtility.UrlEncode(item.GetType().Namespace) + "/" + BOLink + "/" + HttpUtility.UrlEncode(NamespaceLink) + "/" + IdLink.ToString());
            }
            else
            {
                ViewData["LinkItem"] = null;
            }

            ((ItemBase)item).Id = Id;
            item = ((ItemBase)(item)).PopulateOne((ItemBase)item, true);

            ViewBag.Title = boproperties.SingleName + ": " + ((ItemBase)(item)).GetName();

            if (item != null)
            {
                ViewData["AllowCRUD"] = true;
                var pss = new PropertySorter();
                var pdc = TypeDescriptor.GetProperties(item.GetType());
                var properties = pss.GetAdvancedProperties(pdc, Authentication.GetCurrentUser());

                var PropertiesGroup = new Dictionary<string, List<AdvancedProperty>>();

                foreach (AdvancedProperty property in properties)
                {
                    if (!PropertiesGroup.ContainsKey(property.Common.DisplayGroup))
                        PropertiesGroup.Add(property.Common.DisplayGroup, new List<AdvancedProperty>());

                    PropertiesGroup[property.Common.DisplayGroup].Add(property);
                }

                if (item.GetType() == typeof(JuliaAlertLib.BusinessObjects.User))
                {
                    if (!((JuliaAlertLib.BusinessObjects.User)item).HasAtLeastOnePermission(((JuliaAlertLib.BusinessObjects.User)item).Role.Permission))
                    {
                        ViewData["AllowCRUD"] = false;
                    }
                }

                ViewData["Properties"] = PropertiesGroup;
                ViewData["Grid_Type"] = item.GetType().AssemblyQualifiedName;

                if (LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.RevisionsAccess) && boproperties.LogRevisions)
                {
                    ViewData["Revisions"] = Revision.LoadRevisions(BO, Id);
                }
                return View("EditItem", item);
            }
            else
            {
                return View("NoItem", item);
            }
        }
        public ActionResult CreateItem(string BO, string Namespace, string BOLink = "ItemBase", string NamespaceLink = "Lib.BusinessObjects", string Id = "")
        {
            if (!Authentication.CheckUser(this.HttpContext, Modulesenum.SMI))
            {
                return new RedirectResult(Config.GetConfigValue("SMILoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (!LIB.Tools.Security.Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.SMIAccess))
            {
                return new RedirectResult(Config.GetConfigValue("SMILoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }

            var item = Activator.CreateInstance(Type.GetType(Namespace + "." + BO + ", " + Namespace.Split('.')[0], true));
            ViewData["BOType"] = item.GetType();
            BoAttribute boproperties = null;
            if (item.GetType().GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true).Length > 0)
            {
                boproperties = (LIB.AdvancedProperties.BoAttribute)item.GetType().GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true)[0];
            }

            if (!LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ReadAccess) || !LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.CreateAccess))
            {
                return View("NoAccess");
            }
            /*
            if (boproperties.ModulesAccess != 0 && (boproperties.ModulesAccess & (long)Modulesenum.SMI) != (long)Modulesenum.SMI)
            {
                return View("NoAccess");
            }*/

            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(item.GetType());
            var properties = pss.GetAdvancedPropertiesForInsert(pdc, Authentication.GetCurrentUser());

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
                        property.PropertyDescriptor.SetValue(item, LinkItem);
                        break;
                    }
                }
            }
            var PropertiesGroup = new Dictionary<string, List<AdvancedProperty>>();

            foreach (AdvancedProperty property in properties)
            {
                if (!PropertiesGroup.ContainsKey(property.Common.DisplayGroup))
                    PropertiesGroup.Add(property.Common.DisplayGroup, new List<AdvancedProperty>());

                PropertiesGroup[property.Common.DisplayGroup].Add(property);
            }

            ViewData["Properties"] = PropertiesGroup;
            ViewData["Grid_Type"] = item.GetType().AssemblyQualifiedName;
            ViewData["BOProperties"] = boproperties;

            return View("CreateItem", item);
        }

        public ActionResult Profile(string Id)
        {
            General.TraceWarn("sId:" + Id);
            if (!Authentication.CheckUser(this.HttpContext, Modulesenum.SMI))
            {
                return new RedirectResult(Config.GetConfigValue("SMILoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (!LIB.Tools.Security.Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.SMIAccess))
            {
                return new RedirectResult(Config.GetConfigValue("SMILoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            var UserId = Convert.ToInt64(Id);
            if (!LIB.Tools.Security.Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.SMIAccess) && UserId != LIB.Tools.Security.Authentication.GetCurrentUser().Id)
            {
                return View("NoAccess");
            }
            General.TraceWarn("UserId:" + UserId.ToString());
            var user = new JuliaAlertLib.BusinessObjects.User(UserId);
            user = (JuliaAlertLib.BusinessObjects.User)user.PopulateOne((ItemBase)user);
            user.Role = (LIB.BusinessObjects.Role) user.Role.PopulateOne((ItemBase)user.Role);
            user.Person = (LIB.BusinessObjects.Person)user.Person.PopulateOne((ItemBase)user.Person);

            ViewData["Revisions"] = Revision.LoadRevisions(user);
            ViewData["BORevisions"] = Revision.LoadRevisions("User", UserId);

            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(user.GetType());
            var properties = pss.GetAdvancedPropertiesForInsert(pdc, Authentication.GetCurrentUser());
            ViewData["Properties"] = properties;

            ViewBag.Title = "Profile: " + user.GetName();

            return View("Profile", user);
        }
    }
}
