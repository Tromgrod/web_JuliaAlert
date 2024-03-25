// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditTemplates.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The edit templates.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System.ComponentModel;

    /// <summary>
    /// The edit templates.
    /// </summary>
    public enum EditTemplates : int
    {
        /// <summary>
        /// The simple input.
        /// </summary>
        [Description("Input")]
        SimpleInput = 0,

        /// <summary>
        /// The date time input.
        /// </summary>
        [Description("ToolCalendar")]
        DateTimeInput = 1,

        /// <summary>
        /// The drop down.
        /// </summary>
        [Description("Dropdwn")]
        DropDown = 2,

        /// <summary>
        /// The multi check.
        /// </summary>
        [Description("ToolMultiCheck")]
        MultiCheck = 3,

        /// <summary>
        /// The check box.
        /// </summary>
        [Description("ToolCheckBox")]
        CheckBox = 4,

        /// <summary>
        /// The multi line.
        /// </summary>
        [Description("Input")]
        MultiLine = 5,

        /// <summary>
        /// The label.
        /// </summary>
        [Description("Label")]
        Label = 6,

        /// <summary>
        /// The start end date.
        /// </summary>
        [Description("ToolStartEndDate")]
        StartEndDate = 7,

        /// <summary>
        /// The pop up input.
        /// </summary>
        [Description("PopUpInput")]
        PopUpInput = 8,

        /// <summary>
        /// The hidden.
        /// </summary>
        [Description("HiddenInput")]
        Hidden = 9,

        /// <summary>
        /// The image upload.
        /// </summary>
        [Description("ImageUpload")]
        ImageUpload = 10,

        /// <summary>
        /// The time input.
        /// </summary>
        [Description("TimeInput")]
        TimeInput = 11,

        /// <summary>
        /// The date input.
        /// </summary>
        [Description("DateTimeInput")]
        DateInput = 12,

        /// <summary>
        /// The html popup input.
        /// </summary>
        [Description("PopUpHTMLInput")]
        HtmlPopUpInput = 13,

        /// <summary>
        /// The color picker.
        /// </summary>
        [Description("ColorInput")]
        ColorPicker = 14,

        /// <summary>
        /// The image picker.
        /// </summary>
        [Description("ImagePicker")]
        ImagePicker = 15,

        /// <summary>
        /// The document upload.
        /// </summary>
        [Description("DocumentUpload")]
        DocumentUpload = 16,

        /// <summary>
        /// The password.
        /// </summary>
        [Description("Password")]
        Password = 17,

        /// <summary>
        /// The link.
        /// </summary>
        Link = 18,

        /// <summary>
        /// The gallery.
        /// </summary>
        Gallery = 19,

        /// <summary>
        /// The parent.
        /// </summary>
        Parent = 20,

        /// <summary>
        /// The in visible.
        /// </summary>
        InVisible = 21,

        /// <summary>
        /// The drop down parent.
        /// </summary>
        [Description("Dropdwn")]
        DropDownParent = 22,

        /// <summary>
        /// The custom.
        /// </summary>
        Custom = 23,

        /// <summary>
        /// The select list.
        /// </summary>
        [Description("SelectList")]
        SelectList = 24,

        /// <summary>
        /// The select list parent.
        /// </summary>
        [Description("SelectList")]
        SelectListParent = 25,

        /// <summary>
        /// The link item.
        /// </summary>
        [Description("LinkItem")]
        LinkItem = 26,

        /// <summary>
        /// The global link.
        /// </summary>
        GlobalLink = 27,

        /// <summary>
        /// The global link item.
        /// </summary>
        [Description("LinkItem")]
        GlobalLinkItem = 28,

        /// <summary>
        /// The html input.
        /// </summary>
        [Description("HtmlInput")]
        HtmlInput = 29,

        /// <summary>
        /// The not updatable input.
        /// </summary>
        [Description("Input")]
        NotUpdatableInput = 30,

        /// <summary>
        /// The not updatable input.
        /// </summary>
        [Description("Input")]
        PermissionsSelector = 31,

        /// <summary>
        /// The Diagnostic selector.
        /// </summary>
        [Description("Diagnostic")]
        DiagnosticInput = 32,
        
        /// <summary>
        /// The Numer Range.
        /// </summary>
        [Description("NumberRange")]
        NumberRange = 33,

        /// <summary>
        /// The Date Range.
        /// </summary>
        [Description("DateRange")]
        DateRange = 34,

        /// <summary>
        /// The Diagnostic selector.
        /// </summary>
        [Description("AutoComplete")]
        AutoComplete = 35,

        /// <summary>
        /// The link item.
        /// </summary>
        [Description("LinkItems")]
        LinkItems = 36,

        /// <summary>
        /// The Date Time Range.
        /// </summary>
        [Description("DateTimeRange")]
        DateTimeRange = 37,

        /// <summary>
        /// The Multi Select.
        /// </summary>
        [Description("MultiSelect")]
        MultiSelect = 38,

        /// <summary>
        /// The Decimal Numer Range.
        /// </summary>
        [Description("DecimalNumberRange")]
        DecimalNumberRange = 39
    }
}