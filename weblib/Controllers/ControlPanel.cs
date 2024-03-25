// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlPanel.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the ControlPanel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Weblib.Controllers
{
    using System.Web.Mvc;

    using LIB.BusinessObjects;

    using System.Collections.Generic;
    using System;
    using System.Linq;
    using System.Reflection;
    using LIB.Tools.BO;
    using LIB.Tools.Security;
    using LIB.Tools.Utils;
    using System.Web;
    using LIB.Tools.AdminArea;

    /// <summary>
    /// The ControlPanel controller.
    /// </summary>
    public class ControlPanel : BaseController
    {
        public ControlPanel()
        {
            if (Authentication.CheckUser(this.HttpContext, Modulesenum.ControlPanel))
            {
                Type[] JuliaAlertLib = GetTypesInNamespace(Assembly.Load("JuliaAlertLib"), "JuliaAlertLib.BusinessObjects");
                Type[] lib = GetTypesInNamespace(Assembly.Load("LIB"), "LIB.BusinessObjects");
                Type[] Galex = AdditionalTypes();
                List<Type> List = new List<Type>(JuliaAlertLib.Concat<Type>(lib).Concat<Type>(Galex));
                System.Web.HttpContext.Current.Session[SessionItems.Module] = "ControlPanel";
                System.Web.HttpContext.Current.Items["SystemManagement"] = true;

                Dictionary<AdminAreaGroupenum, List<BusinessObject>> finalList = new Dictionary<AdminAreaGroupenum, List<BusinessObject>>();
                foreach (var type in List)
                {
                    LIB.AdvancedProperties.BoAttribute boproperties = null;
                    if (type.GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true).Length > 0)
                    {
                        boproperties = (LIB.AdvancedProperties.BoAttribute)type.GetCustomAttributes(typeof(LIB.AdvancedProperties.BoAttribute), true)[0];
                    }
                    if (boproperties != null)
                    {
                        if (
                            (boproperties.ModulesAccess != 0 && (boproperties.ModulesAccess & (long)Modulesenum.ControlPanel) == (long)Modulesenum.ControlPanel)
                            &&
                            (boproperties.ReadAccess == 0
                            ||
                            LIB.Tools.Security.Authentication.GetCurrentUser().HasAtLeastOnePermission(boproperties.ReadAccess))
                            )
                        {
                            if (!finalList.ContainsKey(boproperties.Group))
                            {
                                finalList.Add(boproperties.Group, new List<BusinessObject>());
                            }
                            finalList[boproperties.Group].Add(new BusinessObject() { Type = type, Properties = boproperties });
                        }
                    }
                }

                ViewData["TypeList"] = finalList;


                ViewBag.Title = "Control Panel";
            }
        }

        public ActionResult DashBoard()
        {
            if (!Authentication.CheckUser(this.HttpContext, Modulesenum.ControlPanel))
            {
                return new RedirectResult(Config.GetConfigValue("CPLoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (!LIB.Tools.Security.Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.CPAccess))
            {
                return new RedirectResult(Config.GetConfigValue("CPLoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }

            return this.View();
        }

        public virtual Type[] AdditionalTypes()
        {
            return null;
        }

        public Type[] GetTypesInNamespace(Assembly assembly, params string[] nameSpaces)
        {
            IEnumerable<Type> types = null;
            foreach (var nameSpace in nameSpaces)
            {
                var tempTypes = assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal) && (t.BaseType == typeof(ItemBase) || (t.BaseType.BaseType != null && t.BaseType.BaseType == typeof(ItemBase)) || (t.BaseType.BaseType != null && t.BaseType.BaseType.BaseType != null && t.BaseType.BaseType.BaseType == typeof(ItemBase))));

                if (types == null)
                    types = tempTypes;
                else
                    types.Union(tempTypes);
            }
            return types.ToArray();
        }
    }
}
