// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuType.cs" company="JuliaAlert">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The MenuType.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertLib.BusinessObjects
{
    using System;
    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Tools.BO;
    using LIB.Tools.AdminArea;

    /// <summary>
    /// The MenuType.
    /// </summary>
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Navigation
      , ModulesAccess = (long)(Modulesenum.SMI)
      , DisplayName = "Типы меню"
      , SingleName = "тип меню"
      , LogRevisions = true
      , Icon = "indent")]
    public class MenuType : ItemBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuType"/> class.
        /// </summary>
        public MenuType()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuType"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public MenuType(long id)
            : base(id)
        {
        }
        #endregion

        public override string GetAdditionalSelectQuery(AdvancedProperty property)
        {
            return ",[" + property.PropertyName + "].Alias" + " AS " + property.PropertyName + "Alias";
        }

        #region Properties

        [Common(Order = 0), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(Order = 1), Template(Mode = Template.Name)]
        public string Alias { get; set; }

        [Common(Order = 2), Template(Mode = Template.Name)]
        public string Controller { get; set; }

        #endregion
    }
}