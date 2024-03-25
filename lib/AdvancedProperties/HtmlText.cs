// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlText.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the HtmlText type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The html text.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HtmlText : PropertyItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlText"/> class.
        /// </summary>
        public HtmlText()
        {
            this.Height = 300;
            this.Width = 800;
            this.Skin = string.Empty; // office2003
            this.ToolBar = "gtoolbarBasic";
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the skin.
        /// </summary>
        public string Skin { get; set; }

        /// <summary>
        /// Gets or sets the tool bar.
        /// </summary>
        public string ToolBar { get; set; }
    }
}