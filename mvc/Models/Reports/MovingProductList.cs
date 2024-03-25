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
    [Bo(DisplayName = "История перемещений моделей", CustomPage = true)]
    public class MovingProductList : ReportBase
    {
        public override string GetAction() => $"open_report_popup('{typeof(MovingProductUnitList).Name}', '{typeof(MovingProduct).Name}', '{typeof(MovingProduct).Namespace}', {MovingProduct.Id})";

        public override string GetLink() => string.Empty;

        [Common(DisplayName = "№ Документа", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public MovingProduct MovingProduct { get; set; }

        [Common(DisplayName = "Модель", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public UniqueProduct UniqueProduct { get; set; }

        [Common(DisplayName = "Код основы модели", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public Product Product { get; set; }

        [Common(DisplayName = "Дата", EditTemplate = EditTemplates.DateRange, _Sortable = true, _Searchable = true),
         Db(Sort = DbSortMode.Desc),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DateRange Date { get; set; }

        [Common(DisplayName = "Коментарий", _Sortable = false, _Searchable = false),
         Template(Mode = Template.String),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string Description { get; set; }

        [Common(DisplayName = "Из склада", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public Stock StockFrom { get; set; }

        [Common(DisplayName = "В склад", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public Stock StockTo { get; set; }

        [Common(DisplayName = "Перемещено ед", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public NumbersRange MovingCount { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)(BasePermissionenum.Production | BasePermissionenum.MDAccess));
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand(nameof(MovingProductList) + "_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SortColumn", SqlDbType.NVarChar, 100) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Field : nameof(Date) });
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
                    var documentNumber = dr[nameof(MovingProduct.DocumentNumber)].ToString();

                    var list = new MovingProductList
                    {
                        MovingProduct = new MovingProduct(Convert.ToInt64(dr[nameof(MovingProduct) + nameof(MovingProduct.Id)])) { DocumentNumber = documentNumber },
                        Date = new DateRange() { From = Convert.ToDateTime(dr[nameof(Date)]) },
                        Description = !string.IsNullOrEmpty(dr[nameof(Description)].ToString()) && dr[nameof(Description)].ToString().Length > 20 ? dr[nameof(Description)].ToString().Substring(0, 20).Trim() + "..." : dr[nameof(Description)].ToString(),
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