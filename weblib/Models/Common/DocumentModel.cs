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
    using LIB.BusinessObjects;
    using LIB.Models.Common;
    using System;
    using Weblib.Models.Common.Enums;

    /// <summary>
    /// The textbox model.
    /// </summary>
    public class DocumentModel : iBaseControlModel
    {
        public DocumentModel()
        {
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public Document Value { get; set; }
        
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets if textbox is required.
        /// </summary>
        public ValidationTypes ValidationType { get; set; }

        /// <summary>
        /// Gets or sets if textbox is ReadOnly.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets if textbox is Disabled.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets textbox required message.
        /// </summary>
        public string RequiredMessage { get; set; }

    }
}
