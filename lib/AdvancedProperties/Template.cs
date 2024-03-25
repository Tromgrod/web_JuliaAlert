// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Template.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Template type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The template.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Template : PropertyItem
    {
        /// <summary>
        /// The translatable name.
        /// </summary>
        public const string TranslatableName = "TranslatableName";

        /// <summary>
        /// The translatable description.
        /// </summary>
        public const string TranslatableDescription = "TranslatableDescription";

        /// <summary>
        /// The description.
        /// </summary>
        public const string Description = "Description";

        /// <summary>
        /// The name.
        /// </summary>
        public const string Name = "Name";

        /// <summary>
        /// The ColorPicker.
        /// </summary>
        public const string ColorPicker = "ColorPicker";

        /// <summary>
        /// The check box.
        /// </summary>
        public const string CheckBox = "CheckBox";

        /// <summary>
        /// The number.
        /// </summary>
        public const string Number = "Number";

        /// <summary>
        /// The number.
        /// </summary>
        public const string Decimal = "Decimal";

        /// <summary>
        /// The link.
        /// </summary>
        public const string Link = "Link";

        /// <summary>
        /// The link item.
        /// </summary>
        public const string LinkItem = "LinkItem";

        /// <summary>
        /// The link item.
        /// </summary>
        public const string LinkItems = "LinkItems";

        /// <summary>
        /// The global link.
        /// </summary>
        public const string GlobalLink = "GlobalLink";

        /// <summary>
        /// The global link item.
        /// </summary>
        public const string GlobalLinkItem = "GlobalLinkItem";

        /// <summary>
        /// The drop down.
        /// </summary>
        public const string DropDown = "DropDown";

        /// <summary>
        /// The parent drop down.
        /// </summary>
        public const string ParentDropDown = "ParentDropDown";

        /// <summary>
        /// The search drop down.
        /// </summary>
        public const string SearchDropDown = "SearchDropDown";

        /// <summary>
        /// The drop down.
        /// </summary>
        public const string SelectList = "SelectList";

        /// <summary>
        /// The parent drop down.
        /// </summary>
        public const string ParentSelectList = "ParentSelectList";

        /// <summary>
        /// The search drop down.
        /// </summary>
        public const string SearchSelectList = "SearchSelectList";

        /// <summary>
        /// The parent.
        /// </summary>
        public const string Parent = "Parent";

        /// <summary>
        /// The date time.
        /// </summary>
        public const string DateTime = "DateTime";

        /// <summary>
        /// The date.
        /// </summary>
        public const string Date = "Date";

        /// <summary>
        /// The date.
        /// </summary>
        public const string DateTimeRange = "DateTimeRange";

        /// <summary>
        /// The date.
        /// </summary>
        public const string DateRange = "DateRange";

        /// <summary>
        /// The string.
        /// </summary>
        public const string String = "String";

        /// <summary>
        /// The string.
        /// </summary>
        public const string VisibleString = "VisibleString";

        /// <summary>
        /// The label string.
        /// </summary>
        public const string LabelString = "LabelString";

        /// <summary>
        /// The email.
        /// </summary>
        public const string Email = "Email";

        /// <summary>
        /// The multi check.
        /// </summary>
        public const string MultiCheck = "MultiCheck";

        /// <summary>
        /// The multi select.
        /// </summary>
        public const string MultiSelect = "MultiSelect";

        /// <summary>
        /// The permissions selector.
        /// </summary>
        public const string PermissionsSelector = "PermissionsSelector";

        /// <summary>
        /// The image.
        /// </summary>
        public const string Image = "Image";

        /// <summary>
        /// The html.
        /// </summary>
        public const string Html = "Html";

        /// <summary>
        /// The html.
        /// </summary>
        public const string HtmlPopup = "HtmlPopup";

        /// <summary>
        /// The html.
        /// </summary>
        public const string Document = "Document";

        /// <summary>
        /// Gets or sets the common.
        /// </summary>
        public Image ImageProperties { get; set; }

        /// <summary>
        /// Gets or sets the common.
        /// </summary>
        public Common Common { get; set; }

        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        public Access Access { get; set; }

        /// <summary>
        /// Gets or sets the validation.
        /// </summary>
        public Validation Validation { get; set; }

        /// <summary>
        /// Gets or sets the translate.
        /// </summary>
        public Translate Translate { get; set; }

        /// <summary>
        /// Gets or sets the translate.
        /// </summary>
        public Db Db { get; set; }

        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        public Service Service { get; set; }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        public string Mode
        {
            get
            {
                return string.Empty;
            }
            set
            {
                switch (value)
                {
                    case TranslatableName:
                        Common = new Common
                        {
                            EditTemplate = EditTemplates.SimpleInput,
                            ControlClass = CssClass.Wide,
                            ViewCssClass = "DefaultInput"
                        };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        Validation = new Validation { ValidationType = ValidationTypes.Required };
                        Translate = new Translate { Translatable = true };
                        break;

                    case Name:
                        Common = new Common
                        {
                            EditTemplate = EditTemplates.SimpleInput,
                            ControlClass = CssClass.Wide,
                            ViewCssClass = "DefaultInput",
                            Sortable = true
                        };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        Validation = new Validation { ValidationType = ValidationTypes.Required };
                        break;

                    case String:
                        Common = new Common
                        {
                            EditTemplate = EditTemplates.SimpleInput,
                            ControlClass = CssClass.Wide,
                            ViewCssClass = "DefaultInput",
                            Sortable = true
                        };
                        Access = new Access { DisplayMode = DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case VisibleString:
                        Common = new Common
                        {
                            EditTemplate = EditTemplates.SimpleInput,
                            ControlClass = CssClass.Wide,
                            ViewCssClass = "DefaultInput",
                            Sortable = true
                        };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case LabelString:
                        Common = new Common { EditTemplate = EditTemplates.Label };
                        Db = new Db { Editable = false, Readable = false, Populate = false };
                        Access = new Access { DisplayMode = DisplayMode.Advanced };
                        break;

                    case Link:
                        Common = new Common { EditTemplate = EditTemplates.Link };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case LinkItem:
                        Common = new Common { EditTemplate = EditTemplates.LinkItem };
                        Access = new Access { DisplayMode = DisplayMode.Advanced };
                        break;

                    case LinkItems:
                        Common = new Common { EditTemplate = EditTemplates.LinkItems };
                        Access = new Access { DisplayMode = DisplayMode.Advanced };
                        Db = new Db { Editable = false, Readable = false, Populate = false };
                        break;

                    case GlobalLink:
                        Common = new Common { EditTemplate = EditTemplates.GlobalLink };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case GlobalLinkItem:
                        Common = new Common { EditTemplate = EditTemplates.GlobalLinkItem };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case DropDown:
                        Common = new Common { EditTemplate = EditTemplates.DropDown };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case ParentDropDown:
                        Common = new Common { EditTemplate = EditTemplates.DropDownParent, Searchable = true };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Search | DisplayMode.Print };
                        break;

                    case SearchDropDown:
                        Common = new Common { EditTemplate = EditTemplates.DropDown, Searchable = true };
                        Access = new Access { DisplayMode = DisplayMode.Search | DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case SelectList:
                        Common = new Common { EditTemplate = EditTemplates.SelectList };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case ParentSelectList:
                        Common = new Common { EditTemplate = EditTemplates.SelectListParent, Searchable = true };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Search | DisplayMode.Print };
                        break;

                    case SearchSelectList:
                        Common = new Common { EditTemplate = EditTemplates.SelectList, Searchable = true };
                        Access = new Access { DisplayMode = DisplayMode.Search | DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case Parent:
                        Common = new Common { EditTemplate = EditTemplates.Parent, Searchable = true };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print | DisplayMode.Search };
                        break;

                    case MultiCheck:
                        Common = new Common { EditTemplate = EditTemplates.MultiCheck };
                        Access = new Access { DisplayMode = DisplayMode.Advanced };
                        break;

                    case MultiSelect:
                        Common = new Common { EditTemplate = EditTemplates.MultiSelect };
                        Access = new Access { DisplayMode = DisplayMode.Advanced };
                        break;

                    case PermissionsSelector:
                        Common = new Common { EditTemplate = EditTemplates.PermissionsSelector };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case Number:
                        Common = new Common
                        {
                            EditTemplate = EditTemplates.SimpleInput,
                            ControlClass = CssClass.Mini,
                            Sortable = true
                        };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        Validation = new Validation
                        {
                            ValidationType = ValidationTypes.RegularExpression,
                            AlertMessage = "ar trebui să fie o cifră",
                            RegularExpression = @"^[0-9-]+$"
                        };
                        break;

                    case Decimal:
                        Common = new Common
                        {
                            EditTemplate = EditTemplates.SimpleInput,
                            ControlClass = CssClass.Mini,
                            Sortable = true
                        };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        Validation = new Validation
                        {
                            ValidationType = ValidationTypes.RegularExpressionRequired,
                            AlertMessage = "ar trebui să fie o cifră",
                            RegularExpression = @"^[0-9.,-]+$"
                        };
                        break;

                    case Email:
                        Common = new Common { EditTemplate = EditTemplates.SimpleInput };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        Validation = new Validation
                        {
                            ValidationType = ValidationTypes.RegularExpression,
                            RegularExpression = @"^[a-z0-9_\.-]+\@[a-z_\.]+\.[\.a-z]+$"
                        };
                        break;

                    case TranslatableDescription:
                        Common = new Common
                        {
                            EditTemplate = EditTemplates.MultiLine,
                            ControlClass = "WideTextArea",
                            ViewCssClass = "WideLabel200px"
                        };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        Validation = new Validation { ValidationType = ValidationTypes.Required };
                        Translate = new Translate { Translatable = true };
                        break;

                    case Description:
                        Common = new Common
                        {
                            EditTemplate = EditTemplates.MultiLine,
                            ControlClass = "WideTextArea",
                            ViewCssClass = "WideLabel200px"
                        };
                        Access = new Access { DisplayMode = DisplayMode.Advanced };
                        Db = new Db() { ParamSize = 2000 };
                        break;

                    case ColorPicker:
                        Common = new Common { EditTemplate = EditTemplates.ColorPicker };
                        Access = new Access { DisplayMode = DisplayMode.Advanced };
                        break;

                    case Html:
                        Common = new Common { EditTemplate = EditTemplates.HtmlInput };
                        Access = new Access { DisplayMode = DisplayMode.Advanced };
                        Db = new Db() { ParamSize = -1 };
                        break;

                    case HtmlPopup:
                        Common = new Common { EditTemplate = EditTemplates.HtmlPopUpInput };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        Db = new Db() { ParamSize = -1 };
                        break;

                    case CheckBox:
                        Common = new Common { EditTemplate = EditTemplates.CheckBox, Sortable = true };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case Image:
                        Common = new Common { EditTemplate = EditTemplates.ImageUpload };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        ImageProperties = ImageProperties ?? new Image();
                        break;

                    case Document:
                        Common = new Common { EditTemplate = EditTemplates.DocumentUpload };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case DateTime:
                        Common = new Common { EditTemplate = EditTemplates.DateTimeInput };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case Date:
                        Common = new Common { EditTemplate = EditTemplates.DateInput };
                        Access = new Access { DisplayMode = DisplayMode.Simple | DisplayMode.Advanced | DisplayMode.Print };
                        break;

                    case DateRange:
                        Common = new Common { EditTemplate = EditTemplates.DateRange, Searchable = true };
                        Access = new Access { DisplayMode = DisplayMode.Search };
                        break;

                    case DateTimeRange:
                        Common = new Common { EditTemplate = EditTemplates.DateTimeRange, Searchable = true };
                        Access = new Access { DisplayMode = DisplayMode.Search };
                        break;
                }
            }
        }
    }
}