using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using LIB.AdvancedProperties;
using LIB.Tools.Controls;
using LIB.BusinessObjects;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;
using LIB.Tools.Security;

namespace JuliaAlert.Models.Reports
{
    public class TotalPayMethods
    {
        public const string AdditionalApply =
            "\n" +
            "CROSS APPLY (\n" +
                "SELECT STRING_AGG(t.[Name], ', ') PayMethods \n" +
                "FROM( \n" +
                    "SELECT DISTINCT t.[Name]  FROM ( \n" +
                        "SELECT pm.[Name], tr.PayMethodId, tr.TransactionTime \n" +
                        "FROM [Transaction] tr \n" +
                        "JOIN PayMethod pm ON pm.PayMethodId = tr.PayMethodId \n" +
                        "WHERE tr.ClientId = t.ClientId \n" +
                    ") t \n" +
                    "WHERE 1 = 1 \n" +
                ") t \n" +
            ") total \n" +
            "\n" +
            "CROSS APPLY (" +
                "SELECT SUM(t.Sum) Sum FROM (\n" +
                    "SELECT SUM(pfo.FinalPrice * pfo.Count) + o.TAX + o.Delivery Sum \n" +
                    "FROM Client cl \n" +
                    "JOIN [Order] o ON o.ClientId = cl.ClientId AND o.DeletedBy IS NULL \n" +
                    "JOIN ProductForOrder pfo ON pfo.OrderId = o.OrderId AND pfo.DeletedBy IS NULL AND pfo.ProductForOrderId NOT IN (SELECT ProductForOrderId FROM [Return] WHERE DeletedBy IS NULL) \n" +
                    "JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId AND sc.DeletedBy IS NULL \n" +
                    "WHERE cl.ClientId = t.ClientId \n" +
                    "GROUP BY o.OrderId, o.TAX, o.Delivery \n" +
                ") t" +
            ") [order] \n" +
            "\n";
    }

    [Bo(DisplayName = "Список транзакций",
    CustomPage = true,
    OpenInNewTab = false,
    AdditionalJoin = TotalPayMethods.AdditionalApply,
    AfterPaginAdditionalQuery = "" +
        "t.ClientId," +
        "t.ClientName," +
        "[order].Sum SumOrder," +
        "SUM(t.TransactionSum) TransactionSum," +
        "[order].Sum - SUM(t.TransactionSum) FinalSum," +
        "total.PayMethods")]
    public class TransactionList : ReportBase
    {
        public override string GetLink() => string.Empty;

        public override string GetAction() => $"open_report_popup('{nameof(TransactionUnitList)}', '{nameof(this.Client)}', '{this.Client.GetType().Namespace}', {this.Client.Id})";

        [Common(DisplayName = "Клиент", _Sortable = true, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Client Client { get; set; }

        private static Currency _Currency;
        [Common(DisplayName = "Валюта", _Sortable = false, _Searchable = false),
         Db(_Ignore = true)]
        public Currency Currency
        {
            get
            {
                if (_Currency == null || _Currency.Id == 0)
                {
                    _Currency = this.Client.GetCurrency();
                }

                return _Currency;
            }
        }

        [Common(DisplayName = "Способ оплаты", _Sortable = false, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Db(ApplyFilter = true),
         Access(DisplayMode = DisplayMode.Search)]
        public PayMethod PayMethod { get; set; }

        [Common(DisplayName = "Способ оплаты", _Sortable = false, _Searchable = false),
         Template(Mode = Template.String),
         Db(_Populate = false),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string PayMethods { get; set; }

        private DecimalNumberRange _SumOrder;
        [Common(DisplayName = "Сумма заказов", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = false, _Searchable = false, DecimalRound = 2, TotalSum = true),
         Db(DeepFilter = true, _Populate = false),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange SumOrder
        {
            get
            {
                if (_SumOrder != null && Currency != null)
                {
                    _SumOrder.PostFix = " " + Currency.GetName();
                }

                return _SumOrder;
            }
            set => _SumOrder = value;
        }

        private DecimalNumberRange _TransactionSum;
        [Common(DisplayName = "Сумма транзакций", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Db(DeepFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
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
        [Common(DisplayName = "Задолжность", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Db(Sort = DbSortMode.Desc, DeepFilter = true, _Populate = false),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
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

        [Common(DisplayName = "Время транзакции", EditTemplate = EditTemplates.DateRange, _Sortable = false, _Searchable = true),
         Db(ApplyFilter = true),
         Access(DisplayMode = DisplayMode.Search)]
        public DateRange TransactionTime { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales);
        }

        public override string GetAdditionalGroupQuery(AdvancedProperty property = null) => "\nGROUP BY t.ClientId, t.ClientName, total.PayMethods, [order].Sum\n";
    }
}