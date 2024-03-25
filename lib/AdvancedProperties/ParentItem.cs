// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parent.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Parent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The parent.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Parent : PropertyItem
    {
        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        public string Prefix { get; set; }
    }
}