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
       , DisplayName = "Способы оплаты"
       , SingleName = "Способ оплаты"
       , DoCancel = true
       , LogRevisions = true)]
    public class PayMethod : ItemBase
    {
        #region Constructors
        public PayMethod()
            : base(0) { }

        public PayMethod(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Способ оплаты"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion
    }
}