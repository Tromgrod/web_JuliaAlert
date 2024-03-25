// ------------------------------public --------------------------------------------------------------------------------------
// <copyright file="Reports.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the Converter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Weblib.Helpers
{
    using LIB.AdvancedProperties;
    using LIB.Tools.Controls;

    using Weblib.Models.Common.Enums;

    /// <summary>
    /// The converter.
    /// </summary>
    public class Reports
    {
        /// <summary>
        /// The get textbox type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetReportBox(string value,int count)
        {
            var html = "";
            for (var i = 0; i < count; i++)
            {
                html += "<div class='print-value'>" + (value!=null && value.Length > i && value[i] != null ? value[i].ToString() : "") + "</div>";
            }

            return html;
        }

    } 
}
