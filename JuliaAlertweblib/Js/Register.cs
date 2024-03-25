// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Register.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the Register type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertweblib.Js
{
    using System.Collections.Generic;

    using Weblib.Helpers;

    /// <summary>
    /// The register.
    /// </summary>
    public class Register
    {
        /// <summary>
        /// The register bundles.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public static List<ResourceRegister> RegisterBundles()
        {
            var javaScripts = Weblib.Js.Register.RegisterBundles();

            javaScripts.AddRange(new List<ResourceRegister>
                                 {
                                     { new ResourceRegister() { File = "~/Scripts/JQuery/jquery.printArea.js", Key = "front_advanced" } },
                                     { new ResourceRegister() { File = "~/Scripts/FrontEnd/dashboard.js", Key = "front_advanced" } },
                                     { new ResourceRegister() { File = "~/Scripts/FrontEnd/document.js", Key = "front_advanced" } },
                                     { new ResourceRegister() { File = "~/Scripts/FrontEnd/report.js", Key = "front_advanced" } },
                                     { new ResourceRegister() { File = "~/Scripts/Other_Plugins/d3.min.js", Key = "d3" } },
                                     { new ResourceRegister() { File = "~/Scripts/Other_Plugins/Donut3D.js", Key = "d3" } },
                                     { new ResourceRegister() { File = "~/Scripts/JQuery/jquery.mousewheel-3.0.6.pack.js", Key = "jquery-ui" } },
                                     { new ResourceRegister() { File = "~/Scripts/JQuery/jquery.fancybox.pack.js", Key = "jquery-ui" } },
                                     { new ResourceRegister() { File = "~/Scripts/JQuery/jquery.fancybox-buttons.js", Key = "jquery-ui" } }
                                 });

            return javaScripts;
        }
    }
}
