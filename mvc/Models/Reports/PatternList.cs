using LIB.AdvancedProperties;
using JuliaAlertLib.BusinessObjects;
using JuliaAlert.Models.Objects;
using DisplayMode = LIB.AdvancedProperties.DisplayMode;
using LIB.Tools.Security;
using LIB.BusinessObjects;

namespace JuliaAlert.Models.Reports
{
    [Bo(DisplayName = "Список лекал",
        CustomPage = true)]
    public class PatternList : ReportBase
    {
        public override string GetLink() => "DocControl/Pattern/" + Pattern.Id;

        [Common(DisplayName = "Название лекало", _Searchable = true),
         Template(Mode = Template.SearchSelectList),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Pattern Pattern { get; set; }

        [Common(DisplayName = "Код", _Sortable = true),
         Template(Mode = Template.String),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Excell)]
        public string Code { get; set; }

        [Common(DisplayName = "Конструктор", _Sortable = true, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Constructor Constructor { get; set; }

        [Common(DisplayName = "Коллекция", _Sortable = true, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public Collection Collection { get; set; }

        [Common(DisplayName = "Место хранения", _Sortable = true, _Searchable = true),
         Template(Mode = Template.ParentDropDown),
         Access(DisplayMode = DisplayMode.Simple | DisplayMode.Search | DisplayMode.Excell)]
        public LocationStorage LocationStorage { get; set; }

        public override bool HaveAccess(string fullModel = default, string id = default)
        {
            var currentUser = Authentication.GetCurrentUser();

            return currentUser.HasAtLeastOnePermission((long)BasePermissionenum.Production);
        }
    }
}