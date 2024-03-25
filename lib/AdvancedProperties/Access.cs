// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Access.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Access type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;
    using LIB.BusinessObjects;

    /// <summary>
    /// The access.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Access : PropertyItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Access"/> class.
        /// </summary>
        public Access()
        {
            this.DisplayMode = DisplayMode.Simple;
            this.VisibleFor = Permission.None.Value;
            this.EditableFor = Permission.None.Value;
            this.SearchableFor = Permission.None.Value;
        }


        /// <summary>
        /// Gets or sets the searchable for.
        /// </summary>
        public long SearchableFor { get; set; }

        /// <summary>
        /// Gets or sets the editable for.
        /// </summary>
        public long EditableFor { get; set; }

        /// <summary>
        /// Gets or sets the visible for.
        /// </summary>
        public long VisibleFor { get; set; }

        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        public DisplayMode DisplayMode { get; set; }
        
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
                if (this.SearchableFor == Permission.None.Value)
                {
                    this.SearchableFor = ((Access)item).SearchableFor;
                }

                if (this.DisplayMode == DisplayMode.Simple)
                {
                    this.DisplayMode = ((Access)item).DisplayMode;
                }

                if (this.EditableFor == Permission.None.Value)
                {
                    this.EditableFor = ((Access)item).EditableFor;
                }

                if (this.VisibleFor == Permission.None.Value)
                {
                    this.VisibleFor = ((Access)item).VisibleFor;
                }
            }
        }
    }
}