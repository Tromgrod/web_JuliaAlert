using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using System.Collections.Generic;
using LIB.AdvancedProperties;
using LIB.Models;
using LIB.Models.Common;
using LIB.Tools.Utils;
using LIB.Tools.Controls;
using LIB.BusinessObjects;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;
using LIB.Tools.Security;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Список приходов тканей",
    CustomPage = true)]
    public class SupplyTextileList : ReportBase
    {
        public override string GetLink() => "DocControl/SupplyTextile/" + SupplyTextile.Id;

        [Common(DisplayName = "№ Документа", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public SupplyTextile SupplyTextile { get; set; }

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

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default) => new List<LinkModel>
        {
            new LinkModel() { Caption = "Обший список приходов", Href = URLHelper.GetUrl("Report/SupplyConsumableList"), Class = "button" }
        };

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Production);
        }
    }
}