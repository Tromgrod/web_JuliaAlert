// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Register.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the Register type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Weblib.Css
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
            var cssScripts = new List<ResourceRegister>
                                 {
                                     { new ResourceRegister() { File = "~/Content/Common/common.css", Key = "common"} },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/jquery.fancybox.css", Key = "common" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/helpers/jquery.fancybox-buttons.css", Key = "common" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/helpers/jquery.fancybox-thumbs.css", Key = "common" } },

                                     { new ResourceRegister() { File = "~/Content/Common/simplified.css", Key = "simplified" } },

                                     { new ResourceRegister() { File = "~/Content/Common/full.css", Key = "full" } },
                                     { new ResourceRegister() { File = "~/Content/themes/base/jquery-ui.css", Key = "full" } },
                                     { new ResourceRegister() { File = "~/Content/themes/base/jquery.ui.datepicker.css", Key = "full" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/datepicker/datepicker3.css", Key = "full" } },
                                     { new ResourceRegister() { File = "~/Content/jquery/tooltipster.css", Key = "full" } },
                                     { new ResourceRegister() { File = "~/Content/jquery/tooltipster-light.css", Key = "full" } },

                                     { new ResourceRegister() { File = "~/Content/SuperBackEnd/back_common.css", Key = "super_back_common" } },
                                     { new ResourceRegister() { File = "~/Content/SuperBackEnd/back_simplified.css", Key = "super_back_simplified" } },
                                     { new ResourceRegister() { File = "~/Content/SuperBackEnd/back_full.css", Key = "super_back_full" } },

                                     { new ResourceRegister() { File = "~/Content/BackEnd/back_common.css", Key = "back_common" } },
                                     { new ResourceRegister() { File = "~/Content/BackEnd/back_simplified.css", Key = "back_simplified" } },
                                     { new ResourceRegister() { File = "~/Content/BackEnd/back_full.css", Key = "back_full" } },
                                     { new ResourceRegister() { File = "~/Content/BackEnd/bootstrap.css", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Content/BackEnd/AdminLTE.css", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Content/BackEnd/skins/_all-skins.css", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/iCheck/flat/blue.css", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/morris/morris.css", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/jvectormap/jquery-jvectormap-1.2.2.css", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/datatables/dataTables.bootstrap.css", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/daterangepicker/daterangepicker-bs3.css", Key = "bootstrap" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/select2/select2.css", Key = "bootstrap" } },

                                     { new ResourceRegister() { File = "~/Content/FrontEnd/front_common.css", Key = "front_common" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/jquery.fancybox.css", Key = "front_common" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/helpers/jquery.fancybox-buttons.css", Key = "front_common" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/jquery.fancybox-thumbs.css", Key = "front_common" } },

                                     { new ResourceRegister() { File = "~/Content/FrontEnd/front_simplified.css", Key = "front_simplified" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/front_full.css", Key = "front_full" } },
                                     { new ResourceRegister() { File = "~/Scripts/Bootstrap_Plugins/select2/select2.css", Key = "front_full" } },

                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/win_safari.css", Key = "win_safari" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/win_chrome.css", Key = "win_chrome" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/win_firefox.css", Key = "win_firefox" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/mac_safari.css", Key = "mac_safari" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/mac_chrome.css", Key = "mac_chrome" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/ie11.css", Key = "ie11" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/ie10.css", Key = "ie10" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/ie9.css", Key = "ie9" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/ieold.css", Key = "ieold" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/android_firefox.css", Key = "android_firefox" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/android.css", Key = "android" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/ipad.css", Key = "ipad" } },
                                     { new ResourceRegister() { File = "~/Content/FrontEnd/BrowserSpecific/mac_firefox.css", Key = "mac_firefox" } }
                                 };

            return cssScripts;
        }
    }
}
