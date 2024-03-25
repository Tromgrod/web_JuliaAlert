using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.Tools.Controls;
using LIB.Tools.Security;
using LIB.BusinessObjects;
using LIB.Models;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Инвентаризация",
    CustomPage = true,
    OpenInNewTab = false)]
    public class InventoryList : ReportBase
    {
        public override string GetLink() => string.Empty;

        [Common(DisplayName = "Дата инвентаризации", _Searchable = true, _Visible = false),
         Validation(ValidationType = ValidationTypes.Required),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public Inventory Inventory { get; set; }

        [Common(DisplayName = "Склад", _Searchable = true),
         Validation(ValidationType = ValidationTypes.Required),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public Stock Stock { get; set; }

        public SpecificProduct SpecificProduct { get; set; }

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
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string ProductName { get; set; }

        [Common(DisplayName = "Код модели", _Sortable = true),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string Code { get; set; }

        [Common(DisplayName = "Размер", _Sortable = true, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public ProductSize ProductSize { get; set; }

        [Common(DisplayName = "Группа моделей", _Searchable = true, OnChangeSearch = "get_type_model(this.value)"),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public GroupProduct GroupProduct { get; set; }

        [Common(DisplayName = "Тип моделей", _Sortable = true, _Searchable = true),
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

        [Common(DisplayName = "Текущее количество", EditTemplate = EditTemplates.NumberRange, ViewCssClass = "CountInStock", _Searchable = true, _Sortable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public NumbersRange CountInStock { get; set; }

        [Common(DisplayName = "Фактически", EditTemplate = EditTemplates.NumberRange, ViewCssClass = "CurrentCount", ReportModifyFunc = "InventoryList_change_CurrentCount(this)", _Searchable = true, _Sortable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public NumbersRange CurrentCount { get; set; }

        [Common(DisplayName = "Разница", EditTemplate = EditTemplates.NumberRange, ViewCssClass = "Difference", _Searchable = true, _Sortable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public NumbersRange Difference { get; set; }

        [Common(DisplayName = "Инвентаризированные товары", _Searchable = true),
         Template(Mode = Template.CheckBox),
         Access(DisplayMode = DisplayMode.Search)]
        public bool IsInventoryItem { get; set; }

        [Common(DisplayName = "Товары с текущем количеством", _Searchable = true),
         Template(Mode = Template.CheckBox),
         Access(DisplayMode = DisplayMode.Search)]
        public bool ProductWithCurrentCount { get; set; }

        public override string GetHiddenInputs() => 
            GetHiddenInput("SpecificProductId", SpecificProduct.Id) +
            GetHiddenInput("InventoryId", Inventory.Id) +
            GetHiddenInput("StockId", Stock.Id);

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)(BasePermissionenum.Production | BasePermissionenum.MDAccess));
        }

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            if (GetLast<Inventory>().Date.Date != DateTime.Today)
            {
                return new List<LinkModel>
                {
                    new LinkModel { Caption = "Новая инвентаризация", Action = "InventoryList_update()", Class = "button" }
                };
            }

            return new List<LinkModel>();
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand("InventoryList_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SortColumn", SqlDbType.NVarChar, 100) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Field : nameof(CountInStock) });
            cmd.Parameters.Add(new SqlParameter("SortType", SqlDbType.NVarChar, 4) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Direction : DbSortMode.Desc.ToString() });

            var inventoryList = item as InventoryList;

            inventoryList = inventoryList ?? new InventoryList();

            if (inventoryList.Inventory == null || inventoryList.Inventory.Id <= 0)
            {
                var lastInventory = GetLast<Inventory>();

                if (lastInventory.Id <= 0)
                {
                    inventoryList.Inventory.Date = DateTime.Today;
                    inventoryList.Inventory.Insert(inventoryList.Inventory);
                }

                inventoryList.Inventory = lastInventory;
            }
            if (inventoryList.Stock == null || inventoryList.Stock.Id <= 0)
            {
                inventoryList.Stock = Stock.GetMainStock();
            }
            Inventory = inventoryList.Inventory;
            Stock = inventoryList.Stock;

            if (inventoryList != null)
                SetSearchProperties(ref cmd, inventoryList);

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
                    var list = new InventoryList
                    {
                        Inventory = inventoryList.Inventory,
                        Stock = inventoryList.Stock,
                        SpecificProduct = new SpecificProduct(Convert.ToInt64(dr[nameof(SpecificProduct) + nameof(SpecificProduct.Id)])),
                        UniqueProduct = new UniqueProduct(Convert.ToInt64(dr[nameof(UniqueProduct) + nameof(UniqueProduct.Id)])),
                        ProductName = dr[nameof(ProductName)].ToString(),
                        Code = dr[nameof(Code)].ToString(),
                        ProductSize = new ProductSize { Name = dr[nameof(ProductSize) + nameof(ProductSize.Name)].ToString() },
                        TypeProduct = new TypeProduct { Name = dr[nameof(TypeProduct) + nameof(TypeProduct.Name)].ToString() },
                        Compound = new Compound { Name = dr[nameof(Compound) + nameof(Compound.Name)].ToString() },
                        ColorProduct = new ColorProduct { Name = dr[nameof(ColorProduct) + nameof(ColorProduct.Name)].ToString() },
                        Decor = new Decor { Name = dr[nameof(Decor) + nameof(Decor.Name)].ToString() },
                        CountInStock = new NumbersRange() { From = Convert.ToInt32(dr[nameof(CountInStock)]) },
                        CurrentCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(CurrentCount)]) },
                        Difference = new NumbersRange() { From = Convert.ToInt32(dr[nameof(Difference)]) }
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