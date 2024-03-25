using LIB.AdvancedProperties;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using LIB.Tools.Controls;
using LIB.BusinessObjects;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;
using LIB.Tools.Security;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Анализ по моделям",
        CustomPage = true,
        RecordsPerPage = int.MaxValue,
        AfterPaginAdditionalQuery = "" +
        "t.UniqueProductId," +
        "t.ProductId," +
        "t.ProductCode," +
        "t.ColorProductId," +
        "t.ColorProductCode," +
        "t.DecorId," +
        "t.DecorCode," +
        "t.ProductSizeID," +
        "t.ProductSizeName," +
        "SUM(t.ProductOrderCount) AS ProductOrderCount," +
        "SUM(t.OrderSUM) AS OrderSUM," +
        "SUM(t.ReturnCount) AS ReturnCount," +
        "SUM(t.ReturnSUM) AS ReturnSUM," +
        "SUM(t.OrderSUM - t.ReturnSUM) AS SalesSUM," +
        "SUM(t.ProductOrderCount - t.ReturnCount) AS SalesCount," +
        "CAST(SUM(t.ReturnCount) AS FLOAT) * 100 / SUM(t.ProductOrderCount) AS ReturnPercent ",
        DefaultQuery = " t.UniqueProductId, p.ProductId, p.Code AS ProductCode, cp.ColorProductId, cp.Code AS ColorProductCode, d.DecorId, d.Code AS DecorCode,")]
    public class SpecificProduct_ModelList : ReportBase
    {
        public override string GetLink() => "Report/OrderList/Product/JuliaAlert.Models.Objects/" + this.UniqueProduct.Product.Id;

        public override string GetAdditionalJoinQuery() => " " +
            "LEFT JOIN Product p ON p.ProductId = t.ProductId AND p.DeletedBy IS NULL " +
            "JOIN ColorProduct cp ON cp.ColorProductId = t.ColorProductId " +
            "JOIN Decor d ON d.DecorId = t.DecorId ";

        public override string QueryFilter(LIB.BusinessObjects.User User) => $"AND t.CurrencyId IN ({(long)Currency.Enum.USD}, {(long)Currency.Enum.EUR})";

        [Common(DisplayName = "Название модели", _Visible = false, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public UniqueProduct UniqueProduct { get; set; }

        [Common(DisplayName = "Код модели", _Sortable = false),
         Template(Mode = Template.VisibleString),
         Db(_Ignore = true),
         Access(DisplayMode = DisplayMode.Simple)]
        public string Code => UniqueProduct.GetCode();

        [Common(DisplayName = "Размер"),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple)]
        public ProductSize ProductSize { get; set; }

        [Common(DisplayName = "Заказ шт", EditTemplate = EditTemplates.NumberRange, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public NumbersRange ProductOrderCount { get; set; }

        [Common(DisplayName = "Заказ $", EditTemplate = EditTemplates.DecimalNumberRange, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange OrderSUM { get; set; }

        [Common(DisplayName = "Возврат шт", EditTemplate = EditTemplates.NumberRange, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public NumbersRange ReturnCount { get; set; }

        [Common(DisplayName = "Возврат $", EditTemplate = EditTemplates.DecimalNumberRange, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange ReturnSUM { get; set; }

        [Common(DisplayName = "Возврат %", Postfix = " %", EditTemplate = EditTemplates.DecimalNumberRange, DecimalRound = 2),
         Db(_Populate = false, DeepFilter = true),
         Access(DisplayMode = DisplayMode.Simple, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange ReturnPercent { get; set; }

        [Common(DisplayName = "Куплено шт", EditTemplate = EditTemplates.NumberRange, TotalSum = true),
         Db(_Populate = false, DeepFilter = true),
         Access(DisplayMode = DisplayMode.Simple, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public NumbersRange SalesCount { get; set; }

        [Common(DisplayName = "Куплено $", EditTemplate = EditTemplates.DecimalNumberRange, DecimalRound = 2, TotalSum = true),
         Db(Sort = DbSortMode.Desc, _Populate = false, DeepFilter = true),
         Access(DisplayMode = DisplayMode.Simple, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange SalesSUM { get; set; }

        [Common(EditTemplate = EditTemplates.DateRange, DisplayName = "Дата заказов", _Searchable = true, _Visible = false),
         Access(DisplayMode = DisplayMode.Search)]
        public DateRange OrderDate { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales);
        }

        public override string GetAdditionalGroupQuery(AdvancedProperty property = null) =>
            "\nGROUP BY t.UniqueProductId, t.ProductId, t.ProductCode, t.ColorProductId, t.ColorProductCode, t.DecorId, t.DecorCode, t.ProductSizeID, t.ProductSizeName \n";
    }
}