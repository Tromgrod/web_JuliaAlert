// ------------------------------------------------------------------------------------------------------public --------------
// <copyright file="Authorization.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The Authorization.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertLib.Security
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Web;

    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Tools.Utils;
    using System.Collections.Generic;
    using JuliaAlertLib.BusinessObjects;
    using LIB.Tools.BO;

    /// <summary>
    /// The Authorization.
    /// </summary>
    public class Authorization
    {
        public static bool hasPageAccess(Dictionary<long, MenuGroup> MenuItems, LIB.BusinessObjects.User usr, ItemBase item)
        {
            if (usr.HasAtLeastOnePermission((long)BasePermissionenum.SuperAdmin))
                return true;

            return MenuItems.Values.Any(g => g.MenuItems != null 
                                        && g.MenuItems.Values.Any(
                                                mi => ((MenuItem)mi).Page != null 
                                                && ((MenuItem)mi).Page.PageObject != null 
                                                && ((MenuItem)mi).Page.PageObject.Type == item.GetPermissionsType() 
                                                && usr.HasAtLeastOnePermission(((MenuItem)mi).Page.Permission)
                                                )
                                        );
        }
    }
}