// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Encryption.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Encryption type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The encryption.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Encryption : PropertyItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether encrypted.
        /// </summary>
        public bool Encrypted { get; set; }
    }
}