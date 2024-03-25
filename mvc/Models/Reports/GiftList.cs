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
    [Bo(DisplayName = "Список подарков", CustomPage = true)]
    public class GiftList : ReportBase
    {
        public override string GetLink() => "DocControl/Order/" + this.OrderId;

        [Db(_Ignore = true)]
        private long OrderId { get; set; }

        [Common(DisplayName = "Подарок", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public UniqueProduct UniqueProduct { get; set; }

        [Common(DisplayName = "Код основы подарка", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Search)]
        public Product Product { get; set; }

        [Common(DisplayName = "Название подарка", _Sortable = false),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple)]
        public string ProductName { get; set; }

        [Common(DisplayName = "Код подарока", _Sortable = false),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple)]
        public string Code { get; set; }

        [Common(DisplayName = "Количество", EditTemplate = EditTemplates.NumberRange, _Sortable = true, _Searchable = true, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public NumbersRange Count { get; set; }

        [Common(DisplayName = "Клиент", _Sortable = true, _Searchable = true),
         Template(Mode = Template.ParentSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public Client Client { get; set; }

        [Common(DisplayName = "Дата подарка", EditTemplate = EditTemplates.DateRange, _Sortable = true, _Searchable = true),
         Db(Sort = DbSortMode.Desc),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DateRange GiftDate { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Sales);
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand("GiftList_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SortColumn", SqlDbType.NVarChar, 100) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Field : nameof(GiftDate) });
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
                    var list = new GiftList
                    {
                        OrderId = Convert.ToInt64(dr[nameof(OrderId)]),
                        ProductName = dr[nameof(ProductName)].ToString(),
                        Code = dr[nameof(Code)].ToString(),
                        Count = new NumbersRange() { From = Convert.ToInt32(dr[nameof(Count)]) },
                        Client = new Client { Name = dr[nameof(Client) + nameof(Client.Name)].ToString() },
                        GiftDate = new DateRange() { From = Convert.ToDateTime(dr[nameof(GiftDate)]) }
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