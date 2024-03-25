// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Calendar.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Calendar type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The calendar.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Calendar : PropertyItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether disableable.
        /// </summary>
        public bool Disableable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show time.
        /// </summary>
        public bool ShowTime { get; set; }
    }
}