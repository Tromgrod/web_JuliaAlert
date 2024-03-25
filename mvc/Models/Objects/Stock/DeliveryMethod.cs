using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Order
   , ModulesAccess = (long)(Modulesenum.ControlPanel)
   , DisplayName = "Способы доставки"
   , SingleName = "Способ доставки"
   , DoCancel = true
   , LogRevisions = true)]
    public class DeliveryMethod : ItemBase
    {
        #region Constructors
        public DeliveryMethod()
            : base(0) { }

        public DeliveryMethod(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Способ доставки"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion
    }
}