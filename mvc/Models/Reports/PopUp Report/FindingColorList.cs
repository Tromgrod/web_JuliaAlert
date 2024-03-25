using LIB.AdvancedProperties;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using LIB.Tools.Controls;
using LIB.BusinessObjects;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;
using LIB.Tools.Security;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Склад фурнитур",
        CustomPage = true,
        DefaultQuery = "t.FindingSubspecieCode, t.FindingSpecieCode, t.ColorProductCode, t.CurrentCount * t.Price TotalPrice, ")]
    public class FindingColorList : ReportBase
    {
        public override string GetLink() => "DocControl/FindingColor/" + FindingColor.Id;

        [Common(_Searchable = true, _Visible = false),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search)]
        public Finding Finding { get; set; }

        [Common(DisplayName = "Фурнитура", _Sortable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public FindingColor FindingColor { get; set; }

        [Common(DisplayName = "Код", _Sortable = true),
         Template(Mode = Template.VisibleString),
         Db(_Ignore = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string Code => FindingColor?.GetCode();

        [Common(_Searchable = false, _Sortable = false)]
        public FindingSpecie FindingSpecie { get; set; }

        [Common(_Searchable = false, _Sortable = false)]
        public FindingSubspecie FindingSubspecie { get; set; }

        [Common(DisplayName = "Цвет", _Sortable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public ColorProduct ColorProduct => FindingColor?.ColorProduct;

        [Common(DisplayName = "Кол-во", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = false, _Searchable = false, DecimalRound = 2),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange CurrentCount { get; set; }

        [Common(DisplayName = "Последняя $ за ед", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = false, _Searchable = false, DecimalRound = 2),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange Price { get; set; }

        [Common(DisplayName = "Сумма", EditTemplate = EditTemplates.DecimalNumberRange, _Sortable = false, _Searchable = false, DecimalRound = 2, TotalSum = true),
         Db(_Populate = false, DeepFilter = true),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell, VisibleFor = (long)BasePermissionenum.MoneyInReportsAccess)]
        public DecimalNumberRange TotalPrice { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Production);
        }
    }
}