using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.Tools.Security;
using LIB.Tools.Controls;
using LIB.BusinessObjects;
using LIB.AdvancedProperties;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Список заказов", CustomPage = true)]
    public class OrderList_LocalSales : ReportBase
    {
        public override string GetLink() => "DocControl/Local_Order/" + this.OrderId;

        public long OrderId { get; set; }

        [Common(DisplayName = "Модель", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public UniqueProduct UniqueProduct { get; set; }

        [Common(DisplayName = "Код основы модели", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public Product Product { get; set; }

        [Common(DisplayName = "Группа моделей", _Searchable = true, OnChangeSearch = "get_type_model(this.value)"),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public GroupProduct GroupProduct { get; set; }

        [Common(DisplayName = "Тип моделей", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public TypeProduct TypeProduct { get; set; }

        [Common(DisplayName = "Дата заказов", EditTemplate = EditTemplates.DateRange, _Sortable = true, _Searchable = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DateRange OrderDate { get; set; }

        [Common(DisplayName = "Коды товаров", _Sortable = false, _Searchable = false),
         Template(Mode = Template.LabelString),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string Codes { get; set; }

        [Common(DisplayName = "Коментарий", _Sortable = false, _Searchable = false),
         Template(Mode = Template.String),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string Description { get; set; }

        [Common(DisplayName = "Статус заказа", _Sortable = true, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public OrderState OrderState { get; set; }

        [Common(DisplayName = "Канал продаж", _Sortable = true, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Db(PopulateDropDown = nameof(SalesChannel.PopulateByMD)),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public SalesChannel SalesChannel { get; set; }

        [Common(DisplayName = "Клиент", _Sortable = true, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public Client Client { get; set; }

        [Common(DisplayName = "Номер клиента", _Searchable = true),
         Template(Mode = Template.String),
         Access(DisplayMode = DisplayMode.Search)]
        public string Phone { get; set; }

        [Common(DisplayName = "Заказ MDL", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange TotalSUM { get; set; }

        [Common(DisplayName = "Возврат MDL", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange ReturnSUM { get; set; }

        [Common(DisplayName = "Продажи MDL", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange SalesSUM { get; set; }

        [Common(DisplayName = "Заказ шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public NumbersRange TotalCount { get; set; }

        [Common(DisplayName = "Возврат шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public NumbersRange ReturnCount { get; set; }

        [Common(DisplayName = "Продажи шт", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public NumbersRange SalesCount { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)(BasePermissionenum.Sales | BasePermissionenum.MDAccess));
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand("OrderList_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("CurrencyIds", SqlDbType.NVarChar, 100) { Value = string.Join(",", (long)Currency.Enum.MDL) });
            cmd.Parameters.Add(new SqlParameter("SortColumn", SqlDbType.NVarChar, 100) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Field : nameof(OrderDate) });
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
                    string CurrencyName = dr[nameof(CurrencyName)].ToString();
                    decimal SalesChannelInterestRate = Convert.ToDecimal(dr[nameof(SalesChannelInterestRate)]);

                    var list = new OrderList_LocalSales
                    {
                        OrderId = Convert.ToInt64(dr[nameof(OrderId)]),
                        OrderDate = new DateRange() { From = Convert.ToDateTime(dr[nameof(OrderDate)]) },
                        Codes = dr[nameof(Codes)].ToString(),
                        Description = !string.IsNullOrEmpty(dr[nameof(Description)].ToString()) && dr[nameof(Description)].ToString().Length > 20 ? dr[nameof(Description)].ToString().Substring(0, 20).Trim() + "..." : dr[nameof(Description)].ToString(),
                        OrderState = new OrderState
                        {
                            Name = dr[nameof(OrderState) + nameof(OrderState.Name)].ToString(),
                            Color = dr[nameof(OrderState) + nameof(OrderState.Color)].ToString()
                        },
                        SalesChannel = new SalesChannel
                        {
                            Name = dr[nameof(SalesChannel) + nameof(SalesChannel.Name)].ToString() + (SalesChannelInterestRate > 0 ? SalesChannelInterestRate.ToString("F") : string.Empty)
                        },
                        Client = new Client { Name = dr[nameof(Client) + nameof(Client.Name)].ToString() },
                        TotalSUM = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(TotalSUM)]), PostFix = CurrencyName },
                        ReturnSUM = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(ReturnSUM)]), PostFix = CurrencyName },
                        SalesSUM = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(SalesSUM)]), PostFix = CurrencyName },
                        TotalCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(TotalCount)]) },
                        ReturnCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(ReturnCount)]) },
                        SalesCount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(SalesCount)]) }
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