// ------------------------------------public --------------------------------------------------------------------------------
// <copyright file="TextboxModel.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the TextboxModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Weblib.Models.Common
{
    using LIB.AdvancedProperties;
    using LIB.Models.Common;
    using System;
    using Weblib.Models.Common.Enums;

    public class TextboxModel : iBaseControlModel
    {
        public TextboxModel()
        {
            RequiredMessage = "Заполните поле";
            PopUpClick = "return false";
            UseFancyBox = true;
            AutocompleteClear = true;
            MaxLength = 0;
            MinLength = 0;
            MaxYear = DateTime.Now.AddYears(1).Year;
            AutocompleteMinLen = 2;
        }

        public string Id { get; set; }

        public TextboxType Type { get; set; }

        public string Caption { get; set; }

        public string PlaceHolder { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string HtmlValue { get; set; }

        public DateTime DateValue { get; set; }

        public int MaxYear { get; set; }

        public string Class { get; set; }

        public string EditCssClass { get; set; }

        public ValidationTypes ValidationType { get; set; }

        public bool ReadOnly { get; set; }

        public bool Disabled { get; set; }

        public string RequiredMessage { get; set; }

        public string RegularExpression { get; set; }

        public int MaxLength { get; set; }

        public int MinLength { get; set; }

        public int Width { get; set; }

        public string OnType { get; set; }

        public string Min { get; set; }

        public string Max { get; set; }

        public string OnKeyUp { get; set; }

        public string OnKeyPress { get; set; }

        public string OnChange { get; set; }

        public string ValidationFuction { get; set; }

        public string PopUpParam { get; set; }

        public string PopUpClick { get; set; }

        public string OnDblClick { get; set; }

        public bool UseFancyBox { get; set; }

        public string AutocompleteFilter { get; set; }

        public Type AutocompleteType { get; set; }

        public bool AutocompleteServer { get; set; }

        public int AutocompleteMinLen { get; set; }

        public string AutocompleteName { get; set; }

        public bool AutocompleteAllowNew { get; set; }

        public bool AutocompleteClear { get; set; }

        public string AutocompleteClearFunction { get; set; }
    }
}