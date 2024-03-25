// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Common.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Common type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;
    using System.Globalization;

    /// <summary>
    /// The common.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Common : PropertyItem
    {
        public Common()
        {
            this.EditTemplate = EditTemplates.SimpleInput;
            this.DisplayGroup = "";
            this.ControlClass = CssClass.None.ToString(CultureInfo.InvariantCulture);
            this.EditCssClass = CssClass.None.ToString(CultureInfo.InvariantCulture);
            this.ViewCssClass = CssClass.None.ToString(CultureInfo.InvariantCulture);
            this.Editable = true;
            this.Visible = true;
            this.Order = 1;
        }

        public EditTemplates EditTemplate { get; set; }

        public string Template { get; set; }

        public string EditCssClass { get; set; }

        public string ViewCssClass { get; set; }

        public string PrintCssClass { get; set; }

        public int PrintWidth { get; set; }

        public string Postfix { get; set; }

        public string TotalSumPostfix { get; set; }

        public string ControlClass { get; set; }

        public string DisplayGroup { get; set; }

        private string displayName = string.Empty;

        public string ReportModifyFunc { get; set; }

        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.displayName))
                {
                    return global::LIB.Tools.Utils.Translate.GetTranslatedValue(this.displayName, "BO", this.displayName.Replace("Admin_", string.Empty).Replace("_", " "));
                }
                return "";
            }
            set
            {
                this.displayName = value;
            }
        }

        private string printName = string.Empty;

        public string PrintName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.printName))
                {
                    return global::LIB.Tools.Utils.Translate.GetTranslatedValue(this.printName, "BO", this.printName.Replace("Admin_", string.Empty).Replace("_", " "));
                }
                return DisplayName;
            }
            set
            {
                this.printName = value;
            }
        }

        public int Order { get; set; }

        private string _PropertyDescription = string.Empty;

        public bool TotalSum { get; set; }

        public bool SearchPopUpDate { get; set; }

        public int DecimalRound { get; set; }

        public string PropertyDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(this._PropertyDescription))
                {
                    return global::LIB.Tools.Utils.Translate.GetTranslatedValue(this._PropertyDescription, "AdminArea", this._PropertyDescription.Replace("Admin_", string.Empty).Replace("_", " "));
                }
                return "";
            }
            set
            {
                this._PropertyDescription = value;
            }
        }

        public bool _Editable
        {
            get
            {
                return Editable == true;
            }
            set
            {
                Editable = value;
            }
        }

        public bool? Editable { get; set; }

        public bool _Visible
        {
            get
            {
                return Visible == true;
            }
            set
            {
                Visible = value;
            }
        }

        public bool? Visible { get; set; }

        public bool _Sortable
        {
            get
            {
                return Sortable == true;
            }
            set
            {
                Sortable = value;
            }
        }

        public string OnChangeSearch { get; set; }

        public bool? Sortable { get; set; }

        public bool _Searchable
        {
            get
            {
                return Searchable == true;
            }
            set
            {
                Searchable = value;
            }
        }

        public bool? Searchable { get; set; }

        public override void CopyUserFields(PropertyItem item)
        {
            if (item != null)
            {
                this.EditTemplate = ((Common)item).EditTemplate;
                if (string.IsNullOrEmpty(this.Template))
                {
                    this.Template = ((Common)item).Template;
                }
                if (string.IsNullOrEmpty(this.EditCssClass))
                {
                    this.EditCssClass = ((Common)item).EditCssClass;
                }
                if (string.IsNullOrEmpty(this.DisplayGroup))
                {
                    this.DisplayGroup = ((Common)item).DisplayGroup;
                }
                if (string.IsNullOrEmpty(this.ViewCssClass))
                {
                    this.ViewCssClass = ((Common)item).ViewCssClass;
                }
                if (string.IsNullOrEmpty(this.ControlClass))
                {
                    this.ControlClass = ((Common)item).ControlClass;
                }
                if (string.IsNullOrEmpty(this.DisplayName))
                {
                    this.DisplayName = ((Common)item).DisplayName;
                }
                if (string.IsNullOrEmpty(this.PropertyDescription))
                {
                    this.PropertyDescription = ((Common)item).PropertyDescription;
                }
                if (this.Sortable == null)
                {
                    this.Sortable = ((Common)item).Sortable;
                }
                if (this.Editable == null)
                {
                    this.Editable = ((Common)item).Editable;
                }
                if (this.Visible == null)
                {
                    this.Visible = ((Common)item).Visible;
                }
                if (this.Searchable == null)
                {
                    this.Searchable = ((Common)item).Searchable;
                }
            }
        }
    }
}