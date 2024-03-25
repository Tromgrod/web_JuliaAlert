using LIB.BusinessObjects;
using JuliaAlert.Dashboards.UtilityModel;

namespace JuliaAlert.Dashboards
{
    public class AdminDashboard : BaseDashboard
    {
        public override DashboardItem[] DashboardItems => new[]
        {
            new DashboardItem(ListDashboard.UserList)
        };

        public override bool CheckUser(User currentUser) => currentUser.HasAtLeastOnePermission((long)BasePermissionenum.SuperAdmin);

        public override string BreadcrumbCaption => "Admin";

        public override string IconGroup => "AdminIcons";
    }
}