using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Money
    , ModulesAccess = (long)(Modulesenum.ControlPanel)
    , DisplayName = "Валюты"
    , SingleName = "Валюту"
    , DoCancel = true
    , LogRevisions = true
    , DeleteAccess = (long)BasePermissionenum.SuperAdmin
    , CreateAccess = (long)BasePermissionenum.SuperAdmin)]
    public class Currency : ItemBase
    {
        #region Constructors
        public Currency()
            : base(0) { }

        public Currency(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Валюта"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion

        #region Enum
        public enum Enum : long
        {
            None,
            USD,
            EUR,
            MDL,
            RUB
        }

        public enum NBM_Id : int
        {
            None,
            USD = 44,
            EUR = 47,
            RUB = 36
        }
        #endregion
    }
}