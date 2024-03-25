using System;
using System.Linq;
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
    [Bo(DisplayName = "Склад моделей", CustomPage = true)]
    public class UniqueProductList : ReportBase
    {
        public override string GetLink() => string.Empty;

        public override string GetAction() => $"open_report_popup('SpecificProductList', 'UniqueProduct', 'JuliaAlert.Models.Objects', {UniqueProduct.Id})";

        [Common(DisplayName = "№ Документа", _Searchable = true),
         Template(Mode = Template.Image),
         Access(DisplayMode = DisplayMode.Simple)]
        public Graphic Image { get; set; }

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

        [Common(DisplayName = "Текущее количество", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public NumbersRange CurrentCount { get; set; }

        [Common(DisplayName = "Группа моделей", _Searchable = true, OnChangeSearch = "get_type_model(this.value)"),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public GroupProduct GroupProduct { get; set; }

        [Common(DisplayName = "Тип моделей", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public TypeProduct TypeProduct { get; set; }

        [Common(DisplayName = "Состав", _Sortable = true, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Compound Compound { get; set; }

        [Common(DisplayName = "Цвет", _Sortable = true, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public ColorProduct ColorProduct { get; set; }

        [Common(DisplayName = "Декор", _Sortable = true, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Decor Decor { get; set; }

        [Common(DisplayName = "Размер", _Sortable = true, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public ProductSize ProductSize { get; set; }

        [Common(DisplayName = "Дата склада", _Sortable = true, _Searchable = true),
         Template(Mode = Template.Date),
         Access(DisplayMode = DisplayMode.Search)]
        public DateTime DateStock { get; set; }

        [Common(DisplayName = "Склад", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Db(PopulateDropDown = nameof(Stock.GetStocksByUser)),
         Access(DisplayMode = DisplayMode.Search)]
        public Stock Stock { get; set; }

        [Common(DisplayName = "Товары с текущем количеством", _Searchable = true),
         Template(Mode = Template.CheckBox),
         Access(DisplayMode = DisplayMode.Search)]
        public bool ProductWithCurrentCount { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)(BasePermissionenum.Production | BasePermissionenum.Sales | BasePermissionenum.MDAccess));
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand("UniqueProductList_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SortColumn", SqlDbType.NVarChar, 100) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Field : nameof(CurrentCount) });
            cmd.Parameters.Add(new SqlParameter("SortType", SqlDbType.NVarChar, 4) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Direction : DbSortMode.Asc.ToString() });

            var stocks = Stock.GetStocksByUser();
            var stocksStr = string.Join(",", stocks.Values.Select(s => s.Id));

            cmd.Parameters.Add(new SqlParameter("StockIds", SqlDbType.NVarChar, 100) { Value = stocksStr });

            var uniqueProductList = item as UniqueProductList;

            uniqueProductList = uniqueProductList ?? new UniqueProductList();

            if (uniqueProductList != null)
            {
                if (uniqueProductList.DateStock == default)
                    uniqueProductList.DateStock = DateTime.Now;

                SetSearchProperties(ref cmd, uniqueProductList);
            }

            DateStock = uniqueProductList.DateStock;

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
                    var image = new Graphic
                    {
                        BOName = dr[nameof(Image) + nameof(Image.BOName)].ToString(),
                        Name = dr[nameof(Image) + nameof(Image.Name)].ToString(),
                        Ext = dr[nameof(Image) + nameof(Image.Ext)].ToString()
                    };

                    var list = new UniqueProductList
                    {
                        Image = image,
                        UniqueProduct = new UniqueProduct(Convert.ToInt64(dr[nameof(UniqueProduct) + nameof(UniqueProduct.Id)])),
                        ProductName = dr[nameof(ProductName)].ToString(),
                        Code = dr[nameof(Code)].ToString(),
                        CurrentCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(CurrentCount)]) },
                        TypeProduct = new TypeProduct { Name = dr[nameof(TypeProduct) + nameof(TypeProduct.Name)].ToString() },
                        Compound = new Compound { Name = dr[nameof(Compound) + nameof(Compound.Name)].ToString() },
                        ColorProduct = new ColorProduct { Name = dr[nameof(ColorProduct) + nameof(ColorProduct.Name)].ToString() },
                        Decor = new Decor { Name = dr[nameof(Decor) + nameof(Decor.Name)].ToString() }
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