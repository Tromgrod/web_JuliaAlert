// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginModel.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the LoginModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Weblib.Models
{
    using Weblib.Models.Common;

    /// <summary>
    /// The login model.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public TextboxModel Password { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        public TextboxModel Login { get; set; }

    }
}
