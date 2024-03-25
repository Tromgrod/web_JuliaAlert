// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Translate.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The translate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The translate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Translate : PropertyItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether translatable.
        /// </summary>
        public bool Translatable { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        public string TableName { get; set; }


        /// <summary>
        /// The copy user fields.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void CopyUserFields(PropertyItem item)
        {
            if (item != null)
            {
                if (!this.Translatable)
                {
                    this.Translatable = ((Translate)item).Translatable;
                }
                if (string.IsNullOrEmpty(this.Alias))
                {
                    this.Alias = ((Translate)item).Alias;
                }
                if (string.IsNullOrEmpty(this.TableName))
                {
                    this.TableName = ((Translate)item).TableName;
                }
            }
        }
    }
}