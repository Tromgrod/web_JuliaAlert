// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Common.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Common type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;
    using System.Globalization;

    /// <summary>
    /// The common.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Image : PropertyItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Common"/> class.
        /// </summary>
        public Image()
        {
            this.AdminThumbnailWidth = 62;
            this.AdminThumbnailHeight = 41;
            this.ThumbnailWidth = 62;
            this.ThumbnailHeight = 41;
        }

        /// <summary>
        /// Gets or sets the Admin Thumbnail Width.
        /// </summary>
        public int AdminThumbnailWidth { get; set; }

        /// <summary>
        /// Gets or sets the Admin Thumbnail Height.
        /// </summary>
        public int AdminThumbnailHeight { get; set; }

        /// <summary>
        /// Gets or sets the Thumbnail Width.
        /// </summary>
        public int ThumbnailWidth { get; set; }

        /// <summary>
        /// Gets or sets the Thumbnail Height.
        /// </summary>
        public int ThumbnailHeight { get; set; }


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
                this.AdminThumbnailWidth = ((Image)item).AdminThumbnailWidth;
                this.AdminThumbnailHeight = ((Image)item).AdminThumbnailHeight;
                this.ThumbnailWidth = ((Image)item).ThumbnailWidth;
                this.ThumbnailHeight = ((Image)item).ThumbnailHeight;
            }
        }

    }
}
