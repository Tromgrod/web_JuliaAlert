// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlPanelController.cs" company="Natur Bravo Pilot">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the ControlPanelController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlert.Controllers
{
    using System;
    using JuliaAlertweblib.Controllers;

    public class ControlPanelController : ControlPanel
    {        
        public override Type[] AdditionalTypes() => GetTypesInNamespace(this.GetType().Assembly, "JuliaAlert.Models.Objects");
    }
}
