// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationTypes.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The validation types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    /// <summary>
    /// The validation types.
    /// </summary>
    public enum ValidationTypes : int
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0,

        /// <summary>
        /// The required.
        /// </summary>
        Required = 1,

        /// <summary>
        /// The regular expression.
        /// </summary>
        RegularExpression = 2,

        /// <summary>
        /// The function.
        /// </summary>
        Function = 3,

        /// <summary>
        /// The confirmation.
        /// </summary>
        Confirmation = 4,

        /// <summary>
        /// The confirmation.
        /// </summary>
        RegularExpressionRequired = 5
    }
}