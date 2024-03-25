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
    using System.Collections.Generic;
    using LIB.Tools.BO;
    using LIB.AdvancedProperties;
    using LIB.Models.Common;
    using System;

    public class DropDownModel : iBaseControlModel
    {
        public DropDownModel()
        {
            RequiredMessage = "Заполните поле";
            ShowOptions = true;
        }

        public string Name { get; set; }

        public string Caption { get; set; }

        public string Value { get; set; }

        public Dictionary<long,ItemBase> Values { get; set; }

        public bool Multiple { get; set; }

        public bool ShowOptions { get; set; }

        public Type ItemType { get; set; }

        public string StrValues { get; set; }

        public string Class { get; set; }

        public bool ReadOnly { get; set; }

        public bool AllowDefault { get; set; }

        public ValidationTypes ValidationType { get; set; }

        public string ValidationFuction { get; set; }

        public string ValueName { get; set; }

        public string OnChange { get; set; }

        public string RequiredMessage { get; set; }

        public Dictionary<long, ItemBase> Options { get; set; }

        public Dictionary<string, Dictionary<long, ItemBase>> Groups { get; set; }

        public List<ItemBase> ExcludeOptions { get; set; }

        public string NameField { get; set; }

        public bool Disabled { get; set; }
    }
}