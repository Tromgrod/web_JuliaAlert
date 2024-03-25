// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Translation.cs" company="Galex">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The static translations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertLib.BusinessObjects
{
    using LIB.AdvancedProperties;

    public class Translation : LIB.BusinessObjects.Translation
    {
        #region properties
        [Common(EditTemplate = EditTemplates.SimpleInput, DisplayName = "Admin_RUTrans", ControlClass = CssClass.Wide),
        Access(DisplayMode = LIB.AdvancedProperties.DisplayMode.Simple | LIB.AdvancedProperties.DisplayMode.Advanced),
        Service(LanguageAbbr = "ru")]
        public string Russian{ get; set; }

        [Common(EditTemplate = EditTemplates.SimpleInput, DisplayName = "Admin_MDTrans", ControlClass = CssClass.Wide),
        Access(DisplayMode = LIB.AdvancedProperties.DisplayMode.Simple | LIB.AdvancedProperties.DisplayMode.Advanced),
        Service(LanguageAbbr = "ro")]
        public string Moldavian { get; set; }
        #endregion
    }
}