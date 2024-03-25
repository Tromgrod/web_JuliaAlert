// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Register.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the Register type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Weblib.Js
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
            var javaScripts = new List<ResourceRegister>
                                 {
                                     { new ResourceRegister() { File = "~/Scripts/Common/common.js", Key = "commonlib"} },
                                     { new ResourceRegister() { File = "~/Scripts/Common/utils.js", Key = "commonlib" } },
                                     { new ResourceRegister() { File = "~/Scripts/BackEnd/back_common.js", Key = "back_common" } },
                                     { new ResourceRegister() { File = "~/Scripts/BackEnd/back_utils.js", Key = "back_common" } },

                                     { new ResourceRegister() { File = "~/Scripts/BackEnd/bootstrap.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/BackEnd/bootstrap-checkbox.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/morris/morris.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/sparkline/jquery.sparkline.min.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/jvectormap/jquery-jvectormap-1.2.2.min.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/jvectormap/jquery-jvectormap-world-mill-en.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/knob/jquery.knob.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/datatables/jquery.dataTables.min.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/datatables/dataTables.bootstrap.min.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/daterangepicker/daterangepicker.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/datepicker/bootstrap-datepicker.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/slimScroll/jquery.slimscroll.min.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/fastclick/fastclick.min.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/select2/select2.full.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/BackEnd/app.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/BackEnd/pages/dashboard.js", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/BackEnd/config.js", Key = "bootstrap" } },

                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/datepicker/bootstrap-datepicker.js", Key = "front_common" } },
                                     { new ResourceRegister() { File = "~/Scripts/FrontEnd/front_common.js", Key = "front_common" } },
                                     { new ResourceRegister() { File = "~/Scripts/FrontEnd/front_utils.js", Key = "front_common" } },
                                     { new ResourceRegister() { File = "~/Scripts/FrontEnd/front_advanced.js", Key = "front_advanced" } },
                                     { new ResourceRegister() { File = "~/Scripts/FrontEnd/barcode.js", Key = "front_advanced" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/select2/select2.full.js", Key = "front_advanced" } },
                                     { new ResourceRegister() { File = "~/Scripts/JQuery/jquery-{version}.js", Key = "jquerylib" } },
                                     { new ResourceRegister() { File = "~/Scripts/JQuery/jquery.printArea.js", Key = "jquerylib" } },
                                     { new ResourceRegister() { File = "~/Scripts/JQuery/jquery-ui-{version}.js", Key = "jquery-ui" } },
                                     { new ResourceRegister() { File = "~/Scripts/JQuery/jquery.tooltipster.js", Key = "jquery-ui" } },
                                     { new ResourceRegister() { File = "~/Scripts/JQuery/ajaxupload-v1.2.js", Key = "jquery-upload" } },
                                     { new ResourceRegister() { File = "~/Scripts/JQuery/jquery.fileDownload.js", Key = "jquery-download" } },
                                     { new ResourceRegister() { File = "~/Scripts/Common/login.js", Key = "login" } }
                                 };

            return javaScripts;
        }
    }
}
