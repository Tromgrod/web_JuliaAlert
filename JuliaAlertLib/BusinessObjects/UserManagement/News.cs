// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Contacts.cs" company="JuliaAlert">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The Contact.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertLib.BusinessObjects
{
    using System;
    using LIB.AdvancedProperties;
    using LIB.Tools.BO;

    public class News : ItemBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        public News()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public News(long id)
            : base(id)
        {
        }
        #endregion

        public override string GetCaption() => nameof(this.Title);

        #region Properties
        [Template(Mode = Template.Name)]
        public string Title { get; set; }

        [Template(Mode = Template.Date)]
        public DateTime Date { get; set; }
        
        [Template(Mode = Template.Html)]
        public string Text { get; set; }

        [Template(Mode = Template.CheckBox)]
        public bool Active { get; set; }
        #endregion
    }
}