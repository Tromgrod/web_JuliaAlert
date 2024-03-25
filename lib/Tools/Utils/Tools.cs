// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tools.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Tools type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Utils
{
    using System.Collections.Generic;

    using LIB.Tools.AdminArea;

    /// <summary>
    /// The tools.
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// The get link.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="displayValue">
        /// The display value.
        /// </param>
        /// <param name="final">
        /// The final.
        /// </param>
        /// <returns>
        /// The <see cref="ToolLinkItem"/>.
        /// </returns>
        public static ToolLinkItem GetLink(
            string host, string displayName, string key = "", string value = "", string displayValue = "", bool final = false)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var parameters = new Dictionary<string, string> { { key, value } };
                return GetLink(host, displayName, displayValue, final, parameters);
            }

            return GetLink(host, displayName, displayValue, final);
        }

        /// <summary>
        /// The get link.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        /// <param name="displayValue">
        /// The display value.
        /// </param>
        /// <param name="final">
        /// The final.
        /// </param>
        /// <param name="parameters">
        /// Query Parameters
        /// </param>
        /// <returns>
        /// The <see cref="ToolLinkItem"/>.
        /// </returns>
        public static ToolLinkItem GetLink(
            string host,
            string displayName,
            string displayValue,
            bool final,
            Dictionary<string, string> parameters = null)
        {
            return new ToolLinkItem
                         {
                             Host = host,
                             PropertyName = displayName,
                             PropertyValue = displayValue,
                             Final = final,
                             Parameters = parameters
                         };
        }
    }
}