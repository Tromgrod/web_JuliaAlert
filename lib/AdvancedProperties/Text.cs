// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Text.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Text type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The text.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Text : PropertyItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether money.
        /// </summary>
        public bool Money { get; set; }
    }
}