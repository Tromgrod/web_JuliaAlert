using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using LIB.Tools.Controls;
using LIB.BusinessObjects;
using LIB.Models;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using LIB.Tools.Security;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Список клиентов MD",
    CustomPage = true,
    OpenInNewTab = false)]
    public class ClientList_LocalSales : ReportBase
    {
        public override string GetLink() => string.Empty;

        [Common(DisplayName = "Клиент", _Sortable = true, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Db(Sort = DbSortMode.Desc),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Client Client { get; set; }

        [Common(DisplayName = "Телефон", _Sortable = true, _Searchable = true),
         Template(Mode = Template.String),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public string Phone { get; set; }

        [Common(DisplayName = "Скидка", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public NumbersRange Discount { get; set; }

        [Common(DisplayName = "Д/Р", EditTemplate = EditTemplates.DateRange, _Sortable = true, _Searchable = true, ViewCssClass = "without-years"),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DateRange Birthday { get; set; }

        [Common(DisplayName = "Сумма заказов MDL", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange TotalSUM { get; set; }

        [Common(DisplayName = "Комментарий"),
         Template(Mode = Template.String),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string Comment { get; set; }

        [Common(DisplayName = "Изменить"),
         Template(Mode = Template.LabelString),
         Db(_Ignore = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Advanced)]
        public string Edit => $"<a class=\"mini-button\" onclick=\"open_simple_popup('~/Views/Client/ClientInfo_Local.cshtml', '{Client.GetType().FullName}', {Client.Id})\">Изменить</a>";

        [Common(DisplayName = "Удалить"),
         Template(Mode = Template.LabelString),
         Db(_Ignore = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Advanced)]
        public new string Delete => $"<a class=\"mini-button button-red\" onclick=\"delete_client({this.Client.Id},'{this.Client.GetType().FullName}')\">Удалить</a>";

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default)
        {
            var linkModels = new List<LinkModel>();

            var currentUser = Authentication.GetCurrentUser();

            if (currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales))
            {
                linkModels.Add(new LinkModel() { Caption = "Назад к общему списку клиентов", Href = URLHelper.GetUrl("Report/ClientList_Global"), Class = "button" });
            }

            return linkModels;
        }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)(BasePermissionenum.Sales | BasePermissionenum.MDAccess));
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand("ClientList_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("CurrencyIds", SqlDbType.NVarChar, 100) { Value = string.Join(",", (long)Currency.Enum.MDL) });
            cmd.Parameters.Add(new SqlParameter("SortColumn", SqlDbType.NVarChar, 100) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Field : nameof(Client) + nameof(Client.Id) });
            cmd.Parameters.Add(new SqlParameter("SortType", SqlDbType.NVarChar, 4) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Direction : DbSortMode.Asc.ToString() });

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
                    var list = new ClientList_LocalSales
                    {
                        Client = new Client(Convert.ToInt64(dr[nameof(Client) + nameof(Client.Id)])) 
                        { 
                            Name = dr[nameof(Client.Name)].ToString()
                        },
                        Phone = dr[nameof(Phone)].ToString(),
                        Discount = new NumbersRange() { From = Convert.ToInt32(dr[nameof(Discount)]) },
                        Birthday = new DateRange() { From = Convert.ToDateTime(dr[nameof(Birthday)]) },
                        TotalSUM = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(TotalSUM)]) },
                        Comment = dr[nameof(Comment)].ToString()
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