using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.Tools.Controls;
using LIB.BusinessObjects;
using LIB.AdvancedProperties;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;
using LIB.Tools.Security;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Анализ по моделям", CustomPage = true)]
    public class ModelList : ReportBase
    {
        public override string GetLink() => string.Empty;

        public override string GetAction() => $"open_report_popup('SpecificProduct_ModelList', 'UniqueProduct', 'JuliaAlert.Models.Objects', {UniqueProduct.Id})";

        [Common(DisplayName = "Название модели", _Visible = false, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public UniqueProduct UniqueProduct { get; set; }

        [Common(DisplayName = "Название модели", _Sortable = false),
         Template(Mode = Template.String),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string ProductName { get; set; }

        [Common(DisplayName = "Код основы модели", _Visible = false, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public Product Product { get; set; }

        [Common(DisplayName = "Код модели", _Sortable = false),
         Template(Mode = Template.String),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string Code { get; set; }

        [Common(DisplayName = "Группа моделей", _Searchable = true, OnChangeSearch = "get_type_model(this.value)"),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public GroupProduct GroupProduct { get; set; }

        [Common(DisplayName = "Тип моделей", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public TypeProduct TypeProduct { get; set; }

        [Common(DisplayName = "Заказ шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public NumbersRange ProductOrderCount { get; set; }

        [Common(DisplayName = "Заказ MDL", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange OrderSUM { get; set; }

        [Common(DisplayName = "Возврат шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public NumbersRange ReturnCount { get; set; }

        [Common(DisplayName = "Возврат MDL", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange ReturnSUM { get; set; }

        [Common(DisplayName = "Возврат %", Postfix = " %", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange ReturnPercent { get; set; }

        [Common(DisplayName = "Куплено шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public NumbersRange SalesCount { get; set; }

        [Common(DisplayName = "Куплено MDL", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange SalesSUM { get; set; }

        [Common(DisplayName = "Дата заказов", EditTemplate = EditTemplates.DateRange, _Searchable = true, _Visible = false, SearchPopUpDate = true),
         Access(DisplayMode = DisplayMode.Search)]
        public DateRange OrderDate { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales);
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand("ModelList_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("CurrencyIds", SqlDbType.NVarChar, 100) { Value = string.Join(",", (long)Currency.Enum.USD, (long)Currency.Enum.EUR) });
            cmd.Parameters.Add(new SqlParameter("SortColumn", SqlDbType.NVarChar, 100) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Field : nameof(SalesSUM) });
            cmd.Parameters.Add(new SqlParameter("SortType", SqlDbType.NVarChar, 4) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Direction : DbSortMode.Desc.ToString() });

            if (item != null)
                SetSearchProperties(ref cmd, item);

            var ds = new DataSet();
            new SqlDataAdapter { SelectCommand = cmd }.Fill(ds);

            if (ds.Tables.Count <= 0)
            {
                idisplaytotal = 0;
                ColumsSum = null;
                return null;
            }

            var dataRows = ds.Tables[0].Rows;

            var rowCounter = 0;

            var lists = new Dictionary<long, ItemBase>();

            foreach (DataRow dr in dataRows)
            {
                if (rowCounter >= iPagingStart && iPagingLen > 0)
                {
                    var list = new ModelList
                    {
                        UniqueProduct = new UniqueProduct(Convert.ToInt64(dr[nameof(UniqueProduct) + nameof(UniqueProduct.Id)])),
                        ProductName = dr[nameof(ProductName)].ToString(),
                        Code = dr[nameof(Code)].ToString(),
                        TypeProduct = new TypeProduct() { Name = dr[nameof(TypeProduct) + nameof(TypeProduct.Name)].ToString() },
                        ProductOrderCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(ProductOrderCount)]) },
                        OrderSUM = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(OrderSUM)]) },
                        ReturnCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(ReturnCount)]) },
                        ReturnSUM = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(ReturnSUM)]) },
                        ReturnPercent = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(ReturnPercent)]) },
                        SalesCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(SalesCount)]) },
                        SalesSUM = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(SalesSUM)]) }
                    };

                    lists.Add(rowCounter, list);

                    if (--iPagingLen == 0)
                        break;
                }
                rowCounter++;
            }

            ColumsSum = this.GetTotalColumSumReport(dataRows);
            idisplaytotal = dataRows.Count;

            return lists;
        }
    }
}