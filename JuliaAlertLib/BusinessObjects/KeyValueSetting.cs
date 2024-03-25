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
    using LIB.AdvancedProperties;
    using LIB.Tools.BO;

    public class KeyValueSetting : ItemBase
    {
        #region Constructors
        public KeyValueSetting()
            : base(0) { }

        public KeyValueSetting(long id)
            : base(id) { }
        #endregion

        public override string GetName() => this.Key;

        #region Properties
        [Template(Mode = Template.Name)]
        public string Key { get; set; }

        [Template(Mode = Template.Name)]
        public string Value { get; set; }
        #endregion
    }
}