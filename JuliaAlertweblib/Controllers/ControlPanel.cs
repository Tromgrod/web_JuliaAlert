// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Account.cs" company="Natur Bravo">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the Account type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertweblib.Controllers
{
    using System.Web.Mvc;
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
    /// The account.
    /// </summary>
    public class ControlPanel : Weblib.Controllers.ControlPanel
    {
        public ActionResult Edit(string BO, string Namespace, string BOLink = "ItemBase", string NamespaceLink = "Lib.BusinessObjects", string Id = "")
        {
            if (!Authentication.CheckUser(this.HttpContext, Modulesenum.ControlPanel))
            {
                return new RedirectResult(Config.GetConfigValue("CPLoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (!Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.CPAccess))
            {
                return new RedirectResult(Config.GetConfigValue("CPLoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (string.IsNullOrEmpty(Namespace))
            {
                return new RedirectResult(URLHelper.GetUrl("ControlPanel/DashBoard"));
            }            	
            var item = Activator.CreateInstance(Type.GetType(Namespace + "." + BO + ", " + Namespace.Split('.')[0], true));
            
            ViewData["BOType"] = item.GetType();

            BoAttribute boproperties = null;
            if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
            {
                boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
            }

            if (!Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ReadAccess))
            {
                return View("NoAccess");
            }

            if (boproperties.ModulesAccess != 0 && (boproperties.ModulesAccess & (long)Modulesenum.ControlPanel) != (long)Modulesenum.ControlPanel)
            {
                return View("NoAccess");
            }

            ViewData["BOLink"] = BOLink;
            ViewData["NamespaceLink"] = HttpUtility.UrlEncode(NamespaceLink);
            ViewData["Id"] = Id;
            ViewBag.Title = Config.GetConfigValue("SiteNameAbbr")+": "+boproperties.DisplayName;

            if (!string.IsNullOrEmpty(Id))
            {
                var LinkItem = Activator.CreateInstance(Type.GetType(NamespaceLink + "." + BOLink + ", " + NamespaceLink.Split('.')[0], true));
                ((ItemBase)LinkItem).Id = Convert.ToInt64(Id);
                LinkItem = ((ItemBase)(LinkItem)).PopulateOne((ItemBase)LinkItem, true);
                ViewData["LinkItem"] = LinkItem;

                BoAttribute BOPropertiesLinked = null;
                if (LinkItem.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                {
                    BOPropertiesLinked = (BoAttribute)LinkItem.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
                }
                ViewData["BOPropertiesLinked"] = BOPropertiesLinked;
                ViewData["BOLinkType"] = LinkItem.GetType();
            }
            else
            {
                ViewData["LinkItem"] = null;
            }

            if (boproperties.AllowCreate && Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.CreateAccess))
            {
                var add_button = new ButtonModel();
                add_button.Name = "add_new";
                if (boproperties != null)
                {
                    add_button.Text = "Добавить " + boproperties.SingleName.ToLower();
                }
                else
                {
                    add_button.Text = "Добавить " + item.GetType().Name.ToLower();
                }
                add_button.Class = "btn btn-success btn-add fancybox.ajax";
                var url = "ControlPanel/CreateItem/" + item.GetType().Name + "/" + HttpUtility.UrlEncode(item.GetType().Namespace);
                if (!string.IsNullOrEmpty(Id))
                {
                    url += "/" + BOLink + "/" + HttpUtility.UrlEncode(NamespaceLink) + "/" + Id;
                }
                add_button.Href = URLHelper.GetUrl(url);
                add_button.Icon = "plus";
                ViewData["Add_Button"] = add_button;
            }
            else
            {
                ViewData["Add_Button"] = null;
            }

            if (Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.PrintAccess))
            {
                var print_button = new ButtonModel
                {
                    Name = "print",
                    Text = "Печать",
                    Class = "btn btn-default btn-print",
                    Icon = "print",
                    Action = "do_print_class()"
                };
                ViewData["Print_Button"] = print_button;
            }
            else
            {
                ViewData["Print_Button"] = null;
            }

            if (Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ExportAccess))
            {
                var export_button = new ButtonModel
                {
                    Name = "export",
                    Text = "Экспорт",
                    Class = "btn btn-primary btn-export",
                    Icon = "download"
                };
                ViewData["Export_Button"] = export_button;
            }
            else
            {
                ViewData["Export_Button"] = null;
            }

            if (boproperties.AllowImport && Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ImportAccess))
            {
                var import_button = new ButtonModel
                {
                    Name = "import",
                    Text = "Импорт",
                    Class = "btn btn-warning btn-import",
                    Icon = "file-excel-o"
                };
                ViewData["Import_Button"] = import_button;
            }
            else
            {
                ViewData["Import_Button"] = null;
            }

            if (boproperties.AllowDeleteAll && Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.DeleteAllAccess))
            {
                var deleteall_button = new ButtonModel
                {
                    Name = "deleteall",
                    Text = "Удолить все",
                    Class = "btn btn-danger btn-deleteall",
                    Icon = "trash-o",
                    Action = "delete_all_from_grid('" + boproperties.DisplayName + "')"
                };
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
            if (!Authentication.CheckUser(this.HttpContext, Modulesenum.ControlPanel))
            {
                return new RedirectResult(Config.GetConfigValue("CPLoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (!Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.CPAccess))
            {
                return new RedirectResult(Config.GetConfigValue("CPLoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }

            var item = Activator.CreateInstance(Type.GetType(Namespace + "." + BO + ", " + Namespace.Split('.')[0], true));
            ViewData["BOType"] = item.GetType();
            ViewData["Back_Link"] = URLHelper.GetUrl("ControlPanel/Edit/" + item.GetType().Name + "/" + HttpUtility.UrlEncode(item.GetType().Namespace));
            
            BoAttribute boproperties = null;
            if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
            {
                boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
            }

            ViewData["BOProperties"] = boproperties;

            if (!Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ReadAccess))
            {
                return View("NoAccess");
            }

            if (boproperties.ModulesAccess != 0 && (boproperties.ModulesAccess & (long)Modulesenum.ControlPanel) != (long)Modulesenum.ControlPanel)
            {
                return View("NoAccess");
            }
            
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
                if (LinkItem.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                {
                    BOPropertiesLinked = (BoAttribute)LinkItem.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
                }
                ViewData["BOPropertiesLinked"] = BOPropertiesLinked;
                
                ViewData["BOLink"] = BOLink;
                ViewData["NamespaceLink"] = HttpUtility.UrlEncode(NamespaceLink);
                ViewData["Id"] = IdLink.ToString();
                ViewData["Back_Link"] = URLHelper.GetUrl("ControlPanel/Edit/" + item.GetType().Name + "/" + HttpUtility.UrlEncode(item.GetType().Namespace) + "/" + BOLink + "/" + HttpUtility.UrlEncode(NamespaceLink) + "/" + IdLink.ToString());
            }
            else
            {
                ViewData["LinkItem"] = null;
            }

            ((ItemBase)item).Id = Id;
            item = ((ItemBase)(item)).PopulateOne((ItemBase)item,true);

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
                    if (!Authentication.GetCurrentUser().HasAtLeastOnePermission(((JuliaAlertLib.BusinessObjects.User)item).Role.RoleAccessPermission))
                    {
                        ViewData["AllowCRUD"] = false;
                    }
                }

                ViewData["Properties"] = PropertiesGroup;
                ViewData["Grid_Type"] = item.GetType().AssemblyQualifiedName;

                if (Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.RevisionsAccess) && boproperties.LogRevisions)
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
            if (!Authentication.CheckUser(this.HttpContext, Modulesenum.ControlPanel))
            {
                return new RedirectResult(Config.GetConfigValue("CPLoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (!Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.CPAccess))
            {
                return new RedirectResult(Config.GetConfigValue("CPLoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }

            var item = Activator.CreateInstance(Type.GetType(Namespace + "." + BO + ", " + Namespace.Split('.')[0], true));
            ViewData["BOType"] = item.GetType();
            BoAttribute boproperties = null;
            if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
            {
                boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
            }

            if (!Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ReadAccess) || !Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.CreateAccess))
            {
                return View("NoAccess");
            }

            if (boproperties.ModulesAccess != 0 && (boproperties.ModulesAccess & (long)Modulesenum.ControlPanel) != (long)Modulesenum.ControlPanel)
            {
                return View("NoAccess");
            }

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
                        || property.Common.EditTemplate == EditTemplates.SelectListParent
                        || property.Common.EditTemplate == EditTemplates.DropDownParent)
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
            if (!Authentication.CheckUser(this.HttpContext, Modulesenum.ControlPanel))
            {
                return new RedirectResult(Config.GetConfigValue("CPLoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (!Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.CPAccess))
            {
                return new RedirectResult(Config.GetConfigValue("CPLoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            var UserId = Convert.ToInt64(Id);
            if (!Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.CPAccess) && UserId != Authentication.GetCurrentUser().Id)
            {
                return View("NoAccess");
            }
            General.TraceWarn("UserId:" + UserId.ToString());
            var user = new JuliaAlertLib.BusinessObjects.User(UserId);
            user = (JuliaAlertLib.BusinessObjects.User)user.PopulateOne(user);
            user.Role = (Role) user.Role.PopulateOne(user.Role);
            user.Person = (Person)user.Person.PopulateOne(user.Person);
            
            ViewData["Revisions"] = Revision.LoadRevisions(user);
            ViewData["BORevisions"] = Revision.LoadRevisions("User", UserId);

            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(user.GetType());
            var properties = pss.GetAdvancedPropertiesForInsert(pdc, Authentication.GetCurrentUser());
            ViewData["Properties"] = properties;

            ViewBag.Title = "Profile: "+user.GetName();

            return View("Profile", user);
        }
    }    
}