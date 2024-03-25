// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToolLinkItem.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Summary description for ToolLinkItem
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.AdminArea
{
    using System.Collections.Generic;

    /// <summary>
    /// The tool link item.
    /// </summary>
    public class ToolLinkItem
    {
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string PropertyName{ get; set; }

        /// <summary>
        /// Gets or sets the property description.
        /// </summary>
        public string PropertyDescription{ get; set; }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string PropertyValue{ get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether final.
        /// </summary>
        public bool Final{ get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }
   }
}