// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditTemplatesWorker.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the EditTemplatesWorker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// The edit templates worker.
    /// </summary>
    public static class EditTemplatesWorker
    {
        /// <summary>
        /// The get enumerator description.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetEnumDescription(EditTemplates value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return value.ToString();
        }
    }
}