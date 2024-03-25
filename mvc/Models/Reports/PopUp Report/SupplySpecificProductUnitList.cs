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
    public class SupplySpecificProductUnitList : ReportBase
    {
        public override string GetLink() => "DocControl/SupplySpecificProduct/" + this.SupplySpecificProduct.Id;

        [Common(DisplayName = "№ Документа", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public SupplySpecificProduct SupplySpecificProduct { get; set; }

        [Common(DisplayName = "Модель", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public UniqueProduct UniqueProduct { get; set; }

        [Common(DisplayName = "Код основы модели", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public Product Product { get; set; }

        [Common(DisplayName = "Название модели", _Sortable = false),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple)]
        public string ProductName { get; set; }

        [Common(DisplayName = "Код модели", _Sortable = false),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple)]
        public string Code { get; set; }

        [Common(DisplayName = "Размер модели", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        public ProductSize ProductSize { get; set; }

        //[Common(DisplayName = "Фабрика пошива", _Searchable = true),
        // Template(Mode = Template.ParentDropDown),
        // Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        //public Factory FactoryTailoring { get; set; }

        //[Common(DisplayName = "Стоимость пошива", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
        // Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        //public DecimalNumberRange TailoringCost { get; set; }

        [Common(DisplayName = "Фабрика кроя", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        public Factory FactoryCut { get; set; }

        [Common(DisplayName = "Стоимость кроя", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange CutCost { get; set; }

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
            var cmd = new SqlCommand("SupplySpecificProductUnitList_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            if (item != null && item is SupplySpecificProductUnitList)
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
                    var list = new SupplySpecificProductUnitList
                    {
                        SupplySpecificProduct = new SupplySpecificProduct(Convert.ToInt64(dr[nameof(SupplySpecificProduct) + nameof(SupplySpecificProduct.Id)])),
                        ProductName = dr[nameof(ProductName)].ToString(),
                        Code = dr[nameof(Code)].ToString(),
                        ProductSize = new ProductSize { Name = dr[nameof(ProductSize) + nameof(ProductSize.Name)].ToString() },
                        //FactoryTailoring = new Factory { Name = dr[nameof(FactoryTailoring) + nameof(FactoryTailoring.Name)].ToString() },
                        FactoryCut = new Factory { Name = dr[nameof(FactoryCut) + nameof(FactoryCut.Name)].ToString() },
                        //TailoringCost = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(TailoringCost)]) },
                        CutCost = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(CutCost)]) },
                        ScheduledCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(ScheduledCount)]) },
                        SupplyCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(SupplyCount)]) }
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