// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopUp.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the PopUp type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The pop up.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PopUp : PropertyItem
    {
        /// <summary>
        /// Gets or sets the pop up caption.
        /// </summary>
        public string PopUpCaption { get; set; }
    }
}