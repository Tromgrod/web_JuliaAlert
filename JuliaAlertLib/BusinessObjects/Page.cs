// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Page.cs" company="JuliaAlert">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The Page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertLib.BusinessObjects
{
    using System;

    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Tools.BO;
    using System.Collections.Generic;
    using LIB.Tools.AdminArea;

    /// <summary>
    /// The Contact.
    /// </summary>
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Navigation
       , ModulesAccess = (long)(Modulesenum.SMI)
       , DisplayName = "Страницы"
       , SingleName = "Страница"
       , LogRevisions = true
       , Icon = "share-square-o")]
    public class Page : ItemBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        public Page()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public Page(long id)
            : base(id)
        {
        }
        #endregion

        public override string GetName()
        {
            return Name;
        }

        #region Properties

        [Common(Order = 0), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(Order = 1), Db(Prefix="Page"), Template(Mode = Template.SearchDropDown)]
        public MenuType MenuType { get; set; }

        [Common(Order = 2), Template(Mode = Template.SearchDropDown)]
        public PageObject PageObject { get; set; }

        [Common(Order = 3), Template(Mode = Template.PermissionsSelector), Access(DisplayMode = LIB.AdvancedProperties.DisplayMode.Advanced)]
        public long Permission { get; set; }

        [Common(Order = 4), Template(Mode = Template.CheckBox)]
        public bool Visible { get; set; }

        [Common(Order = 5), Template(Mode = Template.Number)]
        public int SortOrder { get; set; }

        #endregion
    }
}