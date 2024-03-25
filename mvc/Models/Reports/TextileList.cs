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
    [Bo(DisplayName = "Склад основ тканей", CustomPage = true)]
    public class TextileList : ReportBase
    {
        public override string GetLink() => string.Empty;

        public override string GetAction() => $"open_report_popup('{nameof(TextileColorList)}', '{nameof(this.Textile)}', '{this.Textile.GetType().Namespace}', {this.Textile.Id})";

        [Common(DisplayName = "Основа ткани", _Searchable = true, _Sortable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Textile Textile { get; set; }

        [Common(DisplayName = "Код ткани", _Searchable = true, _Sortable = true),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public string Code { get; set; }

        [Common(DisplayName = "Состав", _Searchable = true, _Sortable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Compound Compound { get; set; }

        [Common(DisplayName = "Цвета", _Sortable = true),
         Template(Mode = Template.String),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string Colors { get; set; }

        [Common(DisplayName = "Кол-во", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange CurrentCount { get; set; }

        [Common(DisplayName = "Последняя $ за ед", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2),
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
            var cmd = new SqlCommand("TextileList_Report", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("SortColumn", SqlDbType.NVarChar, 100) { Value = SortParameters != null && SortParameters.Count > 0 ? SortParameters.First().Field : nameof(CurrentCount) });
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

                    var list = new TextileList
                    {
                        Textile = new Textile(Convert.ToInt64(dr[nameof(Textile) + nameof(Textile.Id)])) 
                        {
                            Name = dr[nameof(Textile) + nameof(Textile.Name)].ToString()
                        },
                        Code = dr[nameof(Code)].ToString(),
                        Compound = new Compound { Name = dr[nameof(Compound) + nameof(Compound.Name)].ToString() },
                        Colors = colors,
                        CurrentCount = new DecimalNumberRange() { From = Convert.ToDecimal(dr[nameof(CurrentCount)]) },
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