// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Strings.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Strings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Utils
{
    /// <summary>
    /// The strings.
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// The get limited by length html text.
        /// </summary>
        /// <param name="str">
        /// The string.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetCuttedHtmlText(string str, int length)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(str);
            return null != doc.DocumentNode ? doc.DocumentNode.InnerText.Substring(0, doc.DocumentNode.InnerText.Length > length ? length : doc.DocumentNode.InnerText.Length) : str;
        }

        /// <summary>
        /// The like.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="search">
        /// The search.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Like(string value, string search)
        {
            if (value.ToLower().IndexOf(search.ToLower(), System.StringComparison.Ordinal) > -1)
            {
                return true;
            }

            return false;
        }
    }
}