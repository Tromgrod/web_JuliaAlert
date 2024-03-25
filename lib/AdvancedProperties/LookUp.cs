// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DropDown.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the DropDown type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The drop down.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LookUp : PropertyItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookUp"/> class.
        /// </summary>
        public LookUp()
        {
            JoinTable = true;
            ShowOptions = true;
        }

        /// <summary>
        /// Gets or sets the Advanced Filter value.
        /// </summary>
        public string AdvancedFilter { get; set; }

        /// <summary>
        /// Gets or sets the On Change value.
        /// </summary>
        public string OnChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use default.
        /// </summary>
        public bool DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use default.
        /// </summary>
        public bool SearchAsMulticheck { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use default.
        /// </summary>
        public string SearchQuery { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use default.
        /// </summary>
        public bool SearchAsMultiSelect { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public Type ItemType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use default.
        /// </summary>
        public bool JoinTable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use GpoupByLaboratory.
        /// </summary>
        public string GpoupByField { get; set; }

        /// <summary>
        /// Gets or sets if options should be shown or be populated async.
        /// </summary>
        public bool ShowOptions { get; set; }

        /// <summary>
        /// Gets or sets if options should be shown or be populated async.
        /// </summary>
        public int AutocompleteMinLen { get; set; }
    }
}