// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyItem.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the PropertyItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The property item.
    /// </summary>
    public class PropertyItem : Attribute
    {
        /// <summary>
        /// The copy user fields.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public virtual void CopyUserFields(PropertyItem item)
        {
        }
    }
}