using LIB.AdvancedProperties;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using LIB.Tools.Controls;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;
using LIB.Tools.Security;
using LIB.BusinessObjects;

namespace JuliaAlert.Models.Reports
{
    public class TotalSumCountryStatistic
    {
        public const string AdditionalApply =
            "\n" +
            "CROSS APPLY (\n" +
                "SELECT \n" +
                "SUM(t.ProductOrderCount) AS OrderCount, \n" +
                "SUM(t.SalesCount) AS SalesCount \n" +
                "FROM ( \n" +
                    "SELECT \n" +
                    "SUM(t.ProductOrderCount) AS ProductOrderCount, \n" +
                    "SUM(t.OrderSUM) AS OrderSUM, \n" +
                    "SUM(t.ReturnCount) AS ReturnCount, \n" +
                    "SUM(t.ReturnSUM) AS ReturnSUM, \n" +
                    "SUM(t.SalesCount) AS SalesCount, \n" +
                    "SUM(t.SalesSUM) AS SalesSUM \n" +
                    "FROM CountryStatisticList t \n" +
                    "WHERE 1 = 1 \n" +
                    "GROUP BY t.CountriesId \n" +
                ") t \n" +
                "WHERE 2 = 2\n" +
            ") Total\n";
    }

    [Bo(DisplayName = "Отчет по странам",
    CustomPage = true,
    AdditionalJoin = TotalSumCountryStatistic.AdditionalApply,
    AfterPaginAdditionalQuery =
    "\n" +
    "t.CountriesID, \n" +
    "t.CountriesName, \n" +
    "SUM(t.ProductOrderCount) AS ProductOrderCount, \n" +
    "SUM(t.OrderSUM) AS OrderSUM, \n" +
    "CAST(SUM(t.ProductOrderCount) AS FLOAT) * 100 / Total.OrderCount AS OrderPercent, \n" +
    "SUM(t.ReturnCount) AS ReturnCount, \n" +
    "SUM(t.ReturnSUM) AS ReturnSUM, \n" +
    "CAST(SUM(t.ReturnCount) AS FLOAT) * 100 / SUM(t.ProductOrderCount) AS ReturnPercent, \n" +
    "SUM(t.SalesCount) AS SalesCount, \n" +
    "SUM(t.SalesSUM) AS SalesSUM, \n" +
    "CAST(SUM(t.ProductOrderCount) - SUM(t.ReturnCount) AS FLOAT) * 100 / Total.SalesCount AS SalesPercent \n")]
    public class CountryStatisticList : ReportBase
    {
        public override string GetLink() => string.Empty;

        public override string QueryFilter(LIB.BusinessObjects.User User) => $"AND t.CurrencyId IN ({(long)Currency.Enum.USD}, {(long)Currency.Enum.EUR})";

        [Common(DisplayName = "Страна", _Sortable = true, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Db(ApplyFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public Countries Countries { get; set; }

        [Common(DisplayName = "Заказ шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Db(DeepFilter = true, ApplyFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public NumbersRange ProductOrderCount { get; set; }

        [Common(DisplayName = "Заказ $", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Db(Sort = DbSortMode.Desc, DeepFilter = true, ApplyFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange OrderSUM { get; set; }

        [Common(DisplayName = "Заказ %", Postfix = " %", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Db(_Populate = false, DeepFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange OrderPercent { get; set; }

        [Common(DisplayName = "Возврат шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Db(DeepFilter = true, ApplyFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public NumbersRange ReturnCount { get; set; }

        [Common(DisplayName = "Возврат $", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Db(DeepFilter = true, ApplyFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange ReturnSUM { get; set; }

        [Common(DisplayName = "Возврат %", Postfix = " %", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2),
         Db(_Populate = false, DeepFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange ReturnPercent { get; set; }

        [Common(DisplayName = "Продажа шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Db(DeepFilter = true, ApplyFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public NumbersRange SalesCount { get; set; }

        [Common(DisplayName = "Продажа $", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Db(DeepFilter = true, ApplyFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange SalesSUM { get; set; }

        [Common(DisplayName = "Продажа %", Postfix = " %", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Db(_Populate = false, DeepFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange SalesPercent { get; set; }

        [Common(EditTemplate = EditTemplates.DateRange, DisplayName = "Дата заказов:", _Searchable = true, _Visible = false),
         Db(ApplyFilter = true),
         Access(DisplayMode = DisplayMode.Search)]
        public DateRange OrderDate { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales);
        }

        public override string GetAdditionalGroupQuery(AdvancedProperty property = null) => "\nGROUP BY t.CountriesID, t.CountriesName, Total.OrderCount, Total.SalesCount \n";
    }
}