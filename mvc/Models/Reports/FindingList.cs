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
using LIB.Tools.Security;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Склад основ фурнитур", CustomPage = true)]
    public class FindingList : ReportBase
    {
        public override string GetLink() => string.Empty;

        public override string GetAction() => $"open_report_popup('{nameof(FindingColorList)}', '{nameof(this.Finding)}', '{this.Finding.GetType().Namespace}', {this.Finding.Id})";

        [Common(DisplayName = "Основа фурнитуры", _Searchable = true, _Sortable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Finding Finding { get; set; }

        [Common(DisplayName = "Вид фурнитуры", _Searchable = true, OnChangeSearch = "set_finding_specie(this.value)"),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public FindingSpecie FindingSpecie { get; set; }

        [Common(DisplayName = "Подвид фурнитуры", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public FindingSubspecie FindingSubspecie { get; set; }

        [Common(DisplayName = "Склад", _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Search)]
        public LocationStorage LocationStorage { get; set; }

        [Common(DisplayName = "Цвета", _Sortable = false),
         Template(Mode = Template.String),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string Colors { get; set; }

        [Common(DisplayName = "Кол-во", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange Count { get; set; }

        [Common(DisplayName = "Цена за ед", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange Price { get; set; }

        [Common(DisplayName = "Сумма", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Db(_Populate = false, DeepFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange TotalPrice { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Production);
        }

        public override Dictionary<long, ItemBase> PopulateReport(SqlConnection conn, ItemBase item, int iPagingStart, int iPagingLen, string sSearch, List<SortParameter> SortParameters, LIB.BusinessObjects.User sUser, out long idisplaytotal, out Dictionary<string, decimal> ColumsSum)
        {
            var cmd = new SqlCommand(nameof(FindingList) +  "_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SortColumn", SqlDbType.NVarChar, 100) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Field : nameof(Count) });
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
                    var findingSpecie = new FindingSpecie 
                    {
                        Name = dr[nameof(FindingSpecie) + nameof(FindingSpecie.Name)].ToString() 
                    };

                    var findingSubspecie = new FindingSubspecie
                    {
                        Name = dr[nameof(FindingSubspecie) + nameof(FindingSubspecie.Name)].ToString(),
                        FindingSpecie = findingSpecie
                    };

                    var splitColors = dr[nameof(Colors)].ToString().Split(',');
                    string colors = string.Empty;

                    for (var index = 0; index < splitColors.Length; index++)
                    {
                        if (index != default && index % 4 == 0)
                            colors += "<br>";

                        colors += splitColors[index];

                        if (index != splitColors.Length - 1)
                            colors += ", ";
                    }

                    var list = new FindingList
                    {
                        Finding = new Finding(Convert.ToInt64(dr[nameof(Finding) + nameof(Finding.Id)])) { FindingSubspecie = findingSubspecie },
                        Colors = colors,
                        Count = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(Count)]) },
                        Price = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(Price)]) },
                        TotalPrice = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(TotalPrice)]) }
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