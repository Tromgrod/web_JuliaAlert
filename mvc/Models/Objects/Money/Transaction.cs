using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.AdminArea;
using JuliaAlertLib.BusinessObjects;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Money
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Транзакции"
       , SingleName = "Транзакциию"
       , DoCancel = true
       , LogRevisions = true)]
    public class Transaction : ModelBase
    {
        #region Constructors
        public Transaction()
            : base(0) { }

        public Transaction(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Номер транцакции"), Template(Mode = Template.VisibleString)]
        public string TransactionNumber { get; set; }

        [Common(DisplayName = "Клиент"), Template(Mode = Template.ParentDropDown)]
        public Client Client { get; set; }

        [Common(DisplayName = "Способ оплаты"), Template(Mode = Template.ParentDropDown)]
        public PayMethod PayMethod { get; set; }

        [Common(DisplayName = "Сумма"), Template(Mode = Template.Decimal)]
        public decimal Sum { get; set; }

        [Common(DisplayName = "Время транзакции"), Template(Mode = Template.DateTime)]
        public DateTime TransactionTime { get; set; }

        [Common(DisplayName = "Примечание"), Template(Mode = Template.Description)]
        public string Note { get; set; }
        #endregion

        public override string GetName() => this.TransactionNumber;

        public override string GetCaption() => nameof(this.TransactionNumber);
    }
}