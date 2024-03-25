using LIB.BusinessObjects;
using JuliaAlert.Dashboards.UtilityModel;

namespace JuliaAlert.Dashboards
{
    public class SalesDashboard : BaseDashboard
    {
        public override DashboardItem[] DashboardItems => new[] 
        { 
            DashboardItem.OrderDashboard,
            DashboardItem.OrderLocalDashboard 
        };

        public override bool CheckUser(User currentUser) => currentUser.HasAtLeastOnePermission((long)BasePermissionenum.SalesDashboard);

        public override string BreadcrumbCaption => "Продажи";

        public override string IconGroup => "SalesIcons";
    }
}