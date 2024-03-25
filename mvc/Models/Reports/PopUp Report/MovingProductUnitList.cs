using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Controls;
using LIB.Tools.Security;
using LIB.BusinessObjects;
using LIB.Tools.Utils;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "История перемещений модели", CustomPage = true)]
    public class MovingProductUnitList : ReportBase
    {
        public override string GetLink() => "DocControl/MovingProduct/" + MovingProduct.Id;

        [Common(DisplayName = "№ Документа", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public MovingProduct MovingProduct { get; set; }

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

        [Common(DisplayName = "Из склада", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        public Stock StockFrom { get; set; }

        [Common(DisplayName = "В склад", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        public Stock StockTo { get; set; }

        [Common(DisplayName = "Перемещено ед", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        public NumbersRange MovingCount { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)(BasePermissionenum.Production | BasePermissionenum.MDAccess));
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand(nameof(MovingProductUnitList) + "_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

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
                    var list = new MovingProductUnitList
                    {
                        MovingProduct = new MovingProduct(Convert.ToInt64(dr[nameof(MovingProduct) + nameof(MovingProduct.Id)])),
                        ProductName = dr[nameof(ProductName)].ToString(),
                        Code = dr[nameof(Code)].ToString(),
                        ProductSize = new ProductSize { Name = dr[nameof(ProductSize) + nameof(ProductSize.Name)].ToString() },
                        StockFrom = new Stock { Name = dr[nameof(StockFrom) + nameof(StockFrom.Name)].ToString() },
                        StockTo = new Stock { Name = dr[nameof(StockTo) + nameof(StockTo.Name)].ToString() },
                        MovingCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(MovingCount)]) },
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