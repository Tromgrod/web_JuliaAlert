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

    using Weblib.Models.Common.Enums;
    using LIB.Models.Common;

    /// <summary>
    /// The textbox model.
    /// </summary>
    public class MultyCheckWidgetModel : iBaseControlModel
    {
        public MultyCheckWidgetModel()
        {
            RequiredMessage = "Заполните поле";
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public Dictionary<long, ItemBase> Values { get; set; }
        
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets the input class.
        /// </summary>
        public string InputClass { get; set; }

        /// <summary>
        /// Gets or sets if textbox is required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets if textbox is AllowDefault.
        /// </summary>
        public bool AllowDefault { get; set; }

        /// <summary>
        /// Gets or sets if textbox is required.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets Value Name.
        /// </summary>
        public string ValueName { get; set; }

        /// <summary>
        /// Gets or sets OnChange.
        /// </summary>
        public string OnChange { get; set; }

        /// <summary>
        /// Gets or sets textbox required message.
        /// </summary>
        public string RequiredMessage { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public Dictionary<long, ItemBase> Options { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public Dictionary<string, Dictionary<long, ItemBase>> Groups { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public List<ItemBase> ExcludeOptions { get; set; }
    }
}
