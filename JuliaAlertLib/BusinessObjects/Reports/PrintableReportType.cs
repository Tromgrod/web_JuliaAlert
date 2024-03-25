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
    using LIB.BusinessObjects;
    using LIB.Tools.BO;
    using LIB.Tools.AdminArea;
    using System.Collections.Generic;

    /// <summary>
    /// The Contact.
    /// </summary>
    [Serializable]
    [Bo(ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Tipuri raport de tipar"
       , SingleName = "Tip raport de tipar"
       , DoCancel = true
       , LogRevisions = true)]
    public class PrintableReportType : ItemBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        public PrintableReportType()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public PrintableReportType(long id)
            : base(id)
        {
        }
        #endregion

        #region Properties

        [Common(DisplayName = "Denumire"), Template(Mode = Template.Name), InputBox(OnChange = "UpdateTag(this)")]
        public string Name { get; set; }

        [Template(Mode = Template.Name)]
        public string Tag { get; set; }
        
        [Common(DisplayName="Activ"),Template(Mode = Template.CheckBox)]
        public bool Enabled { get; set; }

        #endregion
    }
}

