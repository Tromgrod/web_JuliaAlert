// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiCheck.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The multi check.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The multi check.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MultiCheck : LookUp
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public Type ItemType { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string TableName { get; set; }
    }
}