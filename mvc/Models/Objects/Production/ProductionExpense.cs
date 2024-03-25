using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Production
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Расходы производства"
       , SingleName = "Новый расход производства"
       , AllowEdit = false
       , DoCancel = true
       , LogRevisions = true)]
    public class ProductionExpense : ItemBase
    {
        #region Constructors
        public ProductionExpense()
            : base(0) { }

        public ProductionExpense(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Расходы производства"), Template(Mode = Template.Decimal)]
        public decimal Expense { get; set; }
        #endregion
    }
}