// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagement.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the SystemManagement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Weblib.Controllers
{
    using System.Web.Mvc;

    using LIB.BusinessObjects;

    using Weblib.Helpers;
    using Weblib.Models;
    using Weblib.Models.Common;
    using Weblib.Models.Common.Enums;
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
    /// The SystemManagement controller.
    /// </summary>
    public class SystemManagement : BaseController
    {
        public SystemManagement()
        {
            if (Authentication.CheckUser(this.HttpContext, Modulesenum.SMI))
            {
                Type[] JuliaAlertLib = GetTypesInNamespace(Assembly.Load("JuliaAlertLib"), "JuliaAlertLib.BusinessObjects");
                Type[] lib = GetTypesInNamespace(Assembly.Load("LIB"), "LIB.BusinessObjects");
                Type[] Galex = AdditionalTypes();
                List<Type> List = new List<Type>(JuliaAlertLib.Concat<Type>(lib).Concat<Type>(Galex));
                System.Web.HttpContext.Current.Session[SessionItems.Module] = "SystemManagement";
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
                            (boproperties.ModulesAccess != 0 && (boproperties.ModulesAccess & (long)Modulesenum.SMI) == (long)Modulesenum.SMI)
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

                ViewBag.Title = "System Management";
            }
        }

        public ActionResult DashBoard()
        {
            if (!Authentication.CheckUser(this.HttpContext, Modulesenum.SMI))
            {
                return new RedirectResult(Config.GetConfigValue("SMILoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }
            if (!LIB.Tools.Security.Authentication.GetCurrentUser().HasPermissions((long)BasePermissionenum.SMIAccess))
            {
                return new RedirectResult(Config.GetConfigValue("SMILoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));
            }

            return this.View();
        }

        public virtual Type[] AdditionalTypes()
        {
            return null;
        }

        public Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal) && (t.BaseType == typeof(ItemBase) || (t.BaseType.BaseType != null && t.BaseType.BaseType == typeof(ItemBase)) || (t.BaseType.BaseType != null && t.BaseType.BaseType.BaseType != null && t.BaseType.BaseType.BaseType == typeof(ItemBase)))).ToArray();
        }
    }
}
