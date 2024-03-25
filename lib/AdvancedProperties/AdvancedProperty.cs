// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdvancedProperty.cs" company="GalexStudio">
//   Copyright ©  2022
// </copyright>
// <summary>
//   Defines the AdvancedProperty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;
    using System.ComponentModel;
    using LIB.Tools.Controls;

    [Serializable]
    public class AdvancedProperty
    {
        public AdvancedProperty() => this.Common = new Common();

        public PropertyDescriptor PropertyDescriptor { get; set; }

        public Type Type { get; set; }

        public string PropertyName { get; set; }

        public Common Common { get; set; }

        public Image Image { get; set; }

        public Validation Validation { get; set; }

        public Translate Translate { get; set; }

        public Encryption Encryption { get; set; }

        public Mark Mark { get; set; }

        public Access Access { get; set; }

        public Db Db { get; set; }

        public PropertyItem Custom { get; set; }

        public Service Service { get; set; }

        public string Control
        {
            get
            {
                switch (Common.EditTemplate)
                {
                    case EditTemplates.DateInput:
                    case EditTemplates.SimpleInput:
                    case EditTemplates.MultiLine:
                    case EditTemplates.HtmlInput:
                    case EditTemplates.HtmlPopUpInput:
                    case EditTemplates.DateTimeInput:
                    case EditTemplates.Password:
                    case EditTemplates.DiagnosticInput:
                    case EditTemplates.AutoComplete:
                    case EditTemplates.NotUpdatableInput:
                        return "Input";
                    case EditTemplates.ImageUpload:
                        return "Image";
                    case EditTemplates.DocumentUpload:
                        return "File";
                    case EditTemplates.DropDown:
                    case EditTemplates.DropDownParent:
                    case EditTemplates.SelectList:
                    case EditTemplates.SelectListParent:
                    case EditTemplates.MultiSelect:
                        return "Select";
                    case EditTemplates.CheckBox:
                        return "CheckBox";
                    case EditTemplates.Link:
                    case EditTemplates.LinkItem:
                    case EditTemplates.GlobalLink:
                    case EditTemplates.GlobalLinkItem:
                    case EditTemplates.LinkItems:
                    case EditTemplates.Parent:
                        return "Link";
                    case EditTemplates.MultiCheck:
                    case EditTemplates.PermissionsSelector:
                        return "MultyCheck";
                    case EditTemplates.NumberRange:
                    case EditTemplates.DecimalNumberRange:
                        return "NumberRange";
                    case EditTemplates.DateRange:
                    case EditTemplates.DateTimeRange:
                        return "DateRange";
                    default: 
                        return "Input";
                }
            }
        }

        public string ControlView => "Default";

        public IDataProcessor GetDataProcessor()
        {
            return (IDataProcessor)Activator.CreateInstance(Type.GetType("Controls." + Control + ".DataProcessor, Controls." + Control + "", true));
        }
    }
}