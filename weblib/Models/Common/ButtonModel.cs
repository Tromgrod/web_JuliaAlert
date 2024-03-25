// ------------------------------------public --------------------------------------------------------------------------------
// <copyright file="TextboxModel.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the TextboxModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Weblib.Models.Common
{
    using LIB.Models.Common;
    using Weblib.Models.Common.Enums;

    /// <summary>
    /// The textbox model.
    /// </summary>
    public class ButtonModel : iBaseControlModel
    {
        public ButtonModel()
        {
            Visible = true;
        }
        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public Icon PredefinedIcon { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Href { get; set; }

        public bool InNewTab { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the Visible.
        /// </summary>
        public bool Visible { get; set; }
    }
}
