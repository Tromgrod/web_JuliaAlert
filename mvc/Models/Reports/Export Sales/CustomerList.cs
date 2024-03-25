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
    [Bo(DisplayName = "Анализ по клиентам", CustomPage = true)]
    public class CustomerList : ReportBase
    {
        public override string GetLink() => "Report/OrderList/Client/JuliaAlert.Models.Objects/" + Client.Id.ToString();

        [Common(DisplayName = "Клиент", _Sortable = true, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public Client Client { get; set; }

        [Common(DisplayName = "Канал продаж"),
         Template(Mode = Template.SearchDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public SalesChannel SalesChannel { get; set; }

        [Common(DisplayName = "Заказов шт", _Sortable = false),
         Template(Mode = Template.LabelString),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string OrdersPerClient { get; set; }

        [Common(DisplayName = "Заказано шт", _Sortable = false),
         Template(Mode = Template.LabelString),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string UnitsPerClient { get; set; }

        [Common(DisplayName = "Заказано шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public NumbersRange ProductOrderCount { get; set; }

        [Common(DisplayName = "Заказано $", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange OrderSUM { get; set; }

        [Common(DisplayName = "Заказано %", Postfix = " %", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange OrderPercent { get; set; }

        [Common(DisplayName = "Возврат шт", _Sortable = false),
         Template(Mode = Template.LabelString),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string ReturnsPerClient { get; set; }

        [Common(DisplayName = "Возврат шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public NumbersRange ReturnCount { get; set; }

        [Common(DisplayName = "Возврат $", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange ReturnSUM { get; set; }

        [Common(DisplayName = "Возврат %", Postfix = " %", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange ReturnPercent { get; set; }

        [Common(DisplayName = "Продажа шт", _Sortable = false),
         Template(Mode = Template.LabelString),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string PurchasePerClient { get; set; }

        [Common(DisplayName = "Продажа шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public NumbersRange SalesCount { get; set; }

        [Common(DisplayName = "Продажа $", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange SalesSUM { get; set; }

        [Common(DisplayName = "Продажа %", Postfix = " %", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DecimalNumberRange SalesPercent { get; set; }

        [Common(DisplayName = "Дата заказов", EditTemplate = EditTemplates.DateRange, _Searchable = true, _Visible = false),
         Access(DisplayMode = DisplayMode.Search)]
        public DateRange OrderDate { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales);
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand("CustomerList_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("CurrencyIds", SqlDbType.NVarChar, 100) { Value = string.Join(",", (long)Currency.Enum.USD, (long)Currency.Enum.EUR) });
            cmd.Parameters.Add(new SqlParameter("SortColumn", SqlDbType.NVarChar, 100) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Field : nameof(OrderSUM) });
            cmd.Parameters.Add(new SqlParameter("SortType", SqlDbType.NVarChar, 4) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Direction : DbSortMode.Desc.ToString() });

            if (item != null && item is CustomerList)
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
                    var list = new CustomerList
                    {
                        Client = new Client(Convert.ToInt64(dr[nameof(Client) + nameof(Client.Id)]))
                        {
                            Name = dr[nameof(Client) + nameof(Client.Name)].ToString()
                        },
                        OrdersPerClient = dr[nameof(OrdersPerClient)].ToString(),
                        UnitsPerClient = dr[nameof(UnitsPerClient)].ToString(),
                        ProductOrderCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(ProductOrderCount)]) },
                        OrderSUM = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(OrderSUM)]) },
                        OrderPercent = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(OrderPercent)]) },
                        ReturnsPerClient = dr[nameof(ReturnsPerClient)].ToString(),
                        ReturnCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(ReturnCount)]) },
                        ReturnSUM = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(ReturnSUM)]) },
                        ReturnPercent = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(ReturnPercent)]) },
                        PurchasePerClient = dr[nameof(PurchasePerClient)].ToString(),
                        SalesCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(SalesCount)]) },
                        SalesSUM = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(SalesSUM)]) },
                        SalesPercent = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(SalesPercent)]) }
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