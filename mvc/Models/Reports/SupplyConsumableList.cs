using System.Collections.Generic;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using LIB.Models;
using LIB.Tools.Utils;
using LIB.Tools.Controls;
using LIB.BusinessObjects;
using LIB.Tools.Security;
using LIB.AdvancedProperties;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Общий список приходов фурнитуры / ткани",
    CustomPage = true)]
    public class SupplyConsumableList : ReportBase
    {
        public override string GetLink() => $"DocControl/{TypeSupplyLink}/" + SupplyId;

        [Common(_Sortable = false, _Searchable = false, _Visible = false),
         Template(Mode = Template.Number),
         Access(DisplayMode = DisplayMode.None)]
        public long SupplyId { get; set; }

        [Common(_Sortable = false, _Searchable = false, _Visible = false),
         Template(Mode = Template.String)]
        public string TypeSupplyLink { get; set; }

        [Common(DisplayName = "Тип прихода", _Sortable = false, _Searchable = false),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple)]
        public string TypeSupplyName { get; set; }

        [Common(DisplayName = "№ Документа", _Sortable = true, _Searchable = true),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public string DocumentNumber { get; set; }

        [Common(DisplayName = "Дата", EditTemplate = EditTemplates.DateRange, _Sortable = true, _Searchable = true),
         Db(Sort = DbSortMode.Desc),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search)]
        public DateRange Date { get; set; }

        [Common(DisplayName = "Поставщик", _Sortable = true, _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Provider Provider { get; set; }

        private string _Description;
        [Common(DisplayName = "Коментарий", _Sortable = false, _Searchable = false),
         Template(Mode = Template.String),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string Description
        {
            get => !string.IsNullOrEmpty(_Description) && _Description.Length > 20 ? _Description.Substring(0, 20).Trim() + "..." : _Description;
            set => _Description = value;
        }

        [Common(DisplayName = "Сумма $", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange TotalPrice { get; set; }

        [Common(DisplayName = "Возврат $", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange ReturnPrice { get; set; }

        [Common(DisplayName = "Конечная $", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange FinalPrice { get; set; }

        [Common(DisplayName = "Возврат", _Sortable = false, _Searchable = true),
         Template(Mode = Template.CheckBox),
         Access(DisplayMode = DisplayMode.Search)]
        public bool IsReturn { get; set; }

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default) => new List<LinkModel>
        {
            new LinkModel() { Caption = "Cписок приходов фурнитур", Href = URLHelper.GetUrl("Report/SupplyFindingList"), Class = "button" },
            new LinkModel() { Caption = "Cписок приходов тканей", Href = URLHelper.GetUrl("Report/SupplyTextileList"), Class = "button" },
        };

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Production);
        }
    }
}