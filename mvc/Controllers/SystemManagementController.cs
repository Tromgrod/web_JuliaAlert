// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagementController.cs" company="Natur Bravo Pilot">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the SystemManagementController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlert.Controllers
{
    using JuliaAlertweblib;
    using JuliaAlertweblib.Controllers;
    using System;
    using Weblib.Controllers;

    /// <summary>
    /// The SystemManagement controller.
    /// </summary>
    public class SystemManagementController : JuliaAlertweblib.Controllers.SystemManagement
    {

        public override Type[] AdditionalTypes()
        {
            Type[] JuliaAlert = GetTypesInNamespace(this.GetType().Assembly, "JuliaAlert.Models.Objects");
            return JuliaAlert;
        }
    }
}
