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
    [Bo(DisplayName = "Склад моделей по размерам",
        CustomPage = true,
        RecordsPerPage = int.MaxValue)]
    public class SpecificProductList : ReportBase
    {
        public override string GetLink() 
        {
            var currentUser = Authentication.GetCurrentUser();

            if (currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales))
            {
                return "Report/OrderList/UniqueProduct/JuliaAlert.Models.Objects/" + this.UniqueProduct.Id;
            }
            else if (currentUser.HasAtLeastOnePermission((long)BasePermissionenum.MDAccess))
            {
                return "Report/OrderList_LocalSales/UniqueProduct/JuliaAlert.Models.Objects/" + this.UniqueProduct.Id;
            }
            else
            {
                return "DocControl/UniqueProduct/" + this.UniqueProduct.Id;
            }
        }

        public override string GetBtnHeaderLink() => URLHelper.GetUrl("DocControl/UniqueProduct/" + this.UniqueProduct.Id);

        [Common(_Visible = false, _Searchable = true),
         Template(Mode = Template.SearchSelectList)]
        public UniqueProduct UniqueProduct { get; set; }

        [Common(DisplayName = "Код модели", _Sortable = false),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple)]
        public string Code { get; set; }

        [Common(DisplayName = "Размер", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        public ProductSize ProductSize { get; set; }

        [Common(DisplayName = "Склад", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public Stock Stock { get; set; }

        [Common(DisplayName = "Текущее количество", EditTemplate = EditTemplates.NumberRange, _Sortable = false, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple)]
        public NumbersRange CurrentCount { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)(BasePermissionenum.Production | BasePermissionenum.Sales | BasePermissionenum.MDAccess));
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand(nameof(SpecificProductList) + "_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var stocks = Stock.GetStocksByUser();
            var stocksStr = string.Join(",", stocks.Values.Select(s => s.Id));

            cmd.Parameters.Add(new SqlParameter("StockIds", SqlDbType.NVarChar, 100) { Value = stocksStr });

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
                    var list = new SpecificProductList
                    {
                        UniqueProduct = new UniqueProduct(Convert.ToInt64(dr[nameof(UniqueProduct) + nameof(UniqueProduct.Id)])),
                        Code = dr[nameof(Code)].ToString(),
                        ProductSize = new ProductSize() { Name = dr[nameof(ProductSize) + nameof(ProductSize.Name)].ToString() },
                        CurrentCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(CurrentCount)]) }
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