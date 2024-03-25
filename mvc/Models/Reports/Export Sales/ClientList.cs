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
    [Bo(DisplayName = "Список клиентов",
    CustomPage = true,
    OpenInNewTab = false)]
    public class ClientList : ReportBase
    {
        public override string GetLink() => string.Empty;

        [Common(DisplayName = "Клиент", _Sortable = true, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Db(Sort = DbSortMode.Desc),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Client Client { get; set; }

        [Common(DisplayName = "Страна", _Sortable = true, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Countries Countries { get; set; }

        [Common(DisplayName = "Город", _Sortable = true, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public City City { get; set; }

        [Common(DisplayName = "Email", _Sortable = true, _Searchable = true),
         Template(Mode = Template.Email),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public string Email { get; set; }

        [Common(DisplayName = "Сумма заказов $", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
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
        public string Edit => $"<a class=\"mini-button\" onclick=\"open_simple_popup('~/Views/Client/ClientInfo.cshtml', '{Client.GetType().FullName}', {Client.Id})\">Изменить</a>";

        [Common(DisplayName = "Удалить"),
         Template(Mode = Template.LabelString),
         Db(_Ignore = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Advanced)]
        public new string Delete => $"<a class=\"mini-button button-red\" onclick=\"delete_client({this.Client.Id},'{this.Client.GetType().FullName}')\">Удалить</a>";

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default) => new List<LinkModel>
        {
            new LinkModel() { Caption = "Назад к общему списку клиентов", Href = URLHelper.GetUrl("Report/ClientList_Global"), Class = "button" },
        };

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales);
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand("ClientList_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("CurrencyIds", SqlDbType.NVarChar, 100) { Value = string.Join(",", (long)Currency.Enum.USD, (long)Currency.Enum.EUR) });
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
                    var list = new ClientList
                    {
                        Client = new Client(Convert.ToInt64(dr[nameof(Client) + nameof(Client.Id)]))
                        {
                            Name = dr[nameof(Client.Name)].ToString()
                        },
                        Countries = new Countries { Name = dr[nameof(Countries) + nameof(Countries.Name)].ToString() },
                        City = new City { Name = dr[nameof(City) + nameof(City.Name)].ToString() },
                        Email = dr[nameof(Email)].ToString(),
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