// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopUpSelector.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the PopUpSelector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The pop up selector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PopUpSelector : LookUp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PopUpSelector"/> class.
        /// </summary>
        public PopUpSelector()
        {
            this.IconField = "Icon";
        }

        /// <summary>
        /// Gets or sets the search label.
        /// </summary>
        public string SearchLabel { get; set; }

        /// <summary>
        /// Gets or sets the control id.
        /// </summary>
        public string ControlId { get; set; }

        /// <summary>
        /// Gets or sets the icon field.
        /// </summary>
        public string IconField { get; set; }
    }
}