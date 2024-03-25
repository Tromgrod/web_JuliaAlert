using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.Tools.Controls;
using LIB.BusinessObjects;
using LIB.AdvancedProperties;
using LIB.Tools.Security;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Список приходов моделей", CustomPage = true)]
    public class SupplySpecificProductList : ReportBase
    {
        public override string GetAction() => $"open_report_popup('SupplySpecificProductUnitList', 'SupplySpecificProduct', 'JuliaAlert.Models.Objects', {SupplySpecificProduct.Id})";

        public override string GetLink() => string.Empty;

        [Common(DisplayName = "№ Документа", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public SupplySpecificProduct SupplySpecificProduct { get; set; }

        [Common(DisplayName = "Дата", EditTemplate = EditTemplates.DateRange, _Sortable = true, _Searchable = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        public DateRange Date { get; set; }

        [Common(DisplayName = "Название модели", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public UniqueProduct UniqueProduct { get; set; }

        [Common(DisplayName = "Код основы модели", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public Product Product { get; set; }

        [Common(DisplayName = "Размер модели", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public ProductSize ProductSize { get; set; }

        [Common(DisplayName = "Фабрика пошива", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public Factory FactoryTailoring { get; set; }

        //[Common(DisplayName = "Средняя стоимость пошива", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2),
        // Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        //public DecimalNumberRange TailoringCostAvg { get; set; }

        [Common(DisplayName = "Фабрика кроя", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public Factory FactoryCut { get; set; }

        [Common(DisplayName = "Средняя стоимость кроя", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange CutCostAvg { get; set; }

        [Common(DisplayName = "Отправлено", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public NumbersRange ScheduledCount { get; set; }

        [Common(DisplayName = "Получено", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public NumbersRange SupplyCount { get; set; }

        public override bool HaveAccess(string fullModel = null, string Id = null)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Production);
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand("SupplySpecificProductList_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SortColumn", SqlDbType.NVarChar, 100) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Field : nameof(Date) });
            cmd.Parameters.Add(new SqlParameter("SortType", SqlDbType.NVarChar, 4) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Direction : DbSortMode.Desc.ToString() });

            if (item != null && item is SupplySpecificProductList)
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
                    var list = new SupplySpecificProductList
                    {
                        SupplySpecificProduct = new SupplySpecificProduct(Convert.ToInt64(dr[nameof(SupplySpecificProduct) + nameof(SupplySpecificProduct.Id)])) 
                        {
                            DocumentNumber = dr[nameof(SupplySpecificProduct.DocumentNumber)].ToString()
                        },
                        Date = new DateRange() { From = Convert.ToDateTime(dr[nameof(Date)]) },
                        //TailoringCostAvg = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(TailoringCostAvg)]) },
                        CutCostAvg = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(CutCostAvg)]) },
                        ScheduledCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(ScheduledCount)]) },
                        SupplyCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(SupplyCount)]) },
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