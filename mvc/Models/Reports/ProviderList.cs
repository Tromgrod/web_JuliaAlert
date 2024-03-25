using System.Collections.Generic;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using LIB.AdvancedProperties;
using LIB.Tools.Controls;
using LIB.BusinessObjects;
using LIB.Models.Common;
using LIB.Models;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;
using LIB.Tools.Security;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Список поставщиков",
        CustomPage = true,
        OpenInNewTab = false,
        DefaultQuery = "t.ReceiptSum - t.PayoutSum DebtSum, ")]
    public class ProviderList : ReportBase
    {
        public override string GetLink() => string.Empty;

        public override string GetAction() => $"open_simple_popup('~/Views/Production/ProviderInfo.cshtml', '{this.Provider.GetType().FullName}', {this.Provider.Id})";

        [Common(DisplayName = "Поставщик", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Provider Provider { get; set; }

        [Common(DisplayName = "Фискальный код", _Searchable = true),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public string FiscalCode { get; set; }

        [Common(DisplayName = "Номер телефона", _Searchable = true),
         Template(Mode = Template.VisibleString),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public string PhoneNumber { get; set; }

        [Common(DisplayName = "Сумма приходов", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
        Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange ReceiptSum { get; set; }

        [Common(DisplayName = "Сумма выплат", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange PayoutSum { get; set; }

        [Common(DisplayName = "Сумма долга (аванса)", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = true, _Searchable = true, DecimalRound = 2, TotalSum = true),
         Db(_Populate = false, DeepFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell | DisplayMode.Search, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange DebtSum { get; set; }

        public override List<LinkModel> LoadBreadcrumbs(string viewModel = default) => new List<LinkModel>
        {
            new LinkModel() { Caption = "Добавить поставщика", Action = $"open_simple_popup('~/Views/Production/ProviderInfo.cshtml', '{typeof(Provider).FullName}', 0)", Class = "button" }
        };

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Production);
        }
    }
}