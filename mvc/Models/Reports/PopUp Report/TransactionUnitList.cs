using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using LIB.AdvancedProperties;
using LIB.Tools.Controls;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;
using LIB.Tools.Security;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Список транзакций",
    CustomPage = true,
    OpenInNewTab = true)]
    public class TransactionUnitList : ReportBase
    {
        public override string GetLink() => _PayMethod != null && _PayMethod.Id != 0 ? "DocControl/Transaction/" + this.Transaction.Id : "DocControl/Order/" + this.Transaction.Id;

        [Common(_Visible = false)]
        public Transaction Transaction { get; set; }

        [Common(DisplayName = "Номер транзакции", _Sortable = false, _Searchable = false),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple)]
        public string TransactionNumber { get; set; }

        [Common(DisplayName = "Клиент", _Sortable = false, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        public Client Client { get; set; }

        [Common(DisplayName = "Время транзакции", EditTemplate = EditTemplates.DateRange, _Sortable = false, _Searchable = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        public DateRange TransactionTime { get; set; }

        [Common(DisplayName = "Валюта", _Sortable = false, _Searchable = false),
         Db(_Ignore = true)]
        public Currency Currency => this.Client != null && this.Client.Id > 0 ? this.Client.GetCurrency() : new Currency();

        private PayMethod _PayMethod;
        [Common(DisplayName = "Способ оплаты", _Sortable = false, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        public PayMethod PayMethod
        {
            get
            {
                if (_PayMethod != null && _PayMethod.Id <= 0)
                {
                    _PayMethod.Name = "Заказ";
                }

                return _PayMethod;
            }
            set => _PayMethod = value;
        }

        private DecimalNumberRange _SumOrder;
        [Common(DisplayName = "Сумма заказа", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = false, _Searchable = false, DecimalRound = 2),
         Db(_Ignore = true),
         Access(DisplayMode = DisplayMode.Simple, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange SumOrder
        {
            get
            {
                if (_SumOrder != null && Currency != null)
                {
                    _SumOrder.PostFix = " " + Currency.GetName();
                }
                else
                    _SumOrder = new DecimalNumberRange();

                return _SumOrder;
            }
            set => _SumOrder = value;
        }

        private DecimalNumberRange _TransactionSum;
        [Common(DisplayName = "Сумма транзакции", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = false, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange TransactionSum
        {
            get
            {
                if (_TransactionSum != null && Currency != null)
                {
                    _TransactionSum.PostFix = " " + Currency.GetName();
                }

                return _TransactionSum;
            }
            set => _TransactionSum = value;
        }

        private DecimalNumberRange _FinalSum;
        [Common(DisplayName = "Задолжность", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = false, _Searchable = true, DecimalRound = 2),
         Db(_Ignore = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange FinalSum
        {
            get
            {
                if (_FinalSum != null && Currency != null)
                {
                    _FinalSum.PostFix = " " + Currency.GetName();
                }

                return _FinalSum;
            }
            set => _FinalSum = value;
        }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales);
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var transactionUnitList = base.PopulateReport(conn, item, iPagingStart, iPagingLen, sSearch, SortParameters, sUser, out idisplaytotal, out ColumsSum).Values
                .Select(tr => (TransactionUnitList)tr)
                .ToList();

            decimal tempFinalSum = 0;

            var Orders = Order.PopulateByClient(((TransactionUnitList)item).Client);

            foreach (var Order in Orders)
            {
                var transactionUnit = new TransactionUnitList
                {
                    Transaction = new Transaction(Order.Id),
                    TransactionNumber = "Заказ: " + Order.OrderNumber,
                    Client = Order.Client,
                    TransactionTime = new DateRange { From = Order.OrderDate },
                    PayMethod = new PayMethod(),
                    SumOrder = new DecimalNumberRange { From = Order.GetFinalTotalSum() },
                    TransactionSum = new DecimalNumberRange()
                };

                transactionUnitList.Add(transactionUnit);
            }


            transactionUnitList = transactionUnitList.OrderBy(tr => tr.TransactionTime.From).ToList();

            for (var index = 0; index < transactionUnitList.Count; index++)
            {
                tempFinalSum += transactionUnitList[index].SumOrder.From;
                tempFinalSum -= transactionUnitList[index].TransactionSum.From;

                transactionUnitList[index].FinalSum = new DecimalNumberRange() { From = tempFinalSum };
            }

            long incriment = 0;

            return transactionUnitList.Select(tr => (ItemBase)tr).ToDictionary(tr => incriment++);
        }
    }
}