// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Validation.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Validation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Validation : PropertyItem
    {
        /// <summary>
        /// The _ alert message.
        /// </summary>
        private string alertMessage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Validation"/> class.
        /// </summary>
        public Validation()
        {
            this.ValidationType = ValidationTypes.None;
            this.ValidationFunction = string.Empty;
        }

        /// <summary>
        /// Gets or sets the validation type.
        /// </summary>
        public ValidationTypes ValidationType { get; set; }

        /// <summary>
        /// Gets or sets the regular expression.
        /// </summary>
        public string RegularExpression { get; set; }

        /// <summary>
        /// Gets or sets the validation function.
        /// </summary>
        public string ValidationFunction { get; set; }


        /// <summary>
        /// Gets or sets the alert message.
        /// </summary>
        public string AlertMessage
        {
            get
            {
                if (!string.IsNullOrEmpty(this.alertMessage))
                {
                    return Tools.Utils.Translate.GetTranslatedValue(this.alertMessage, "BO", this.alertMessage.Replace("Admin_", string.Empty).Replace("_", " "));
                }
                return "";
            }
            set
            {
                this.alertMessage = value;
            }

        }


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
                if (this.ValidationType == ValidationTypes.None)
                {
                    this.ValidationType = ((Validation)item).ValidationType;
                }
                if (string.IsNullOrEmpty(this.RegularExpression))
                {
                    this.RegularExpression = ((Validation)item).RegularExpression;
                }
                if (string.IsNullOrEmpty(this.ValidationFunction))
                {
                    this.ValidationFunction = ((Validation)item).ValidationFunction;
                }
                if (string.IsNullOrEmpty(this.AlertMessage))
                {
                    this.AlertMessage = ((Validation)item).AlertMessage;
                }
            }
        }
    }
}