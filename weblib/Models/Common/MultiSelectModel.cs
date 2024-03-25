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
    using LIB.Tools.BO;
    using System;
    using System.Collections.Generic;
    using Weblib.Models.Common.Enums;

    /// <summary>
    /// The textbox model.
    /// </summary>
    public class MultiSelectModel : TextboxModel
    {
        public MultiSelectModel(): base()
        {

        }

        /// <summary>
        /// Gets or sets the Values.
        /// </summary>
        public Dictionary<long, ItemBase> Values { get; set; }

        /// <summary>
        /// Gets or sets the Widget Name.
        /// </summary>
        public string WidgetName { get; set; }
    }
}
