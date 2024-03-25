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
       , DisplayName = "Типы каналов продаж"
       , SingleName = "Тип канала продаж"
       , DoCancel = true
       , LogRevisions = true)]
    public class TypeSalesChannel : ItemBase
    {
        #region Constructors
        public TypeSalesChannel()
            : base(0) { }

        public TypeSalesChannel(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Название"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion
    }
}