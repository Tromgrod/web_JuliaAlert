using LIB.BusinessObjects;
using JuliaAlert.Dashboards.UtilityModel;

namespace JuliaAlert.Dashboards
{
    public class WorkDashboard : BaseDashboard
    {
        public override DashboardItem[] DashboardItems => new[] 
        { 
            DashboardItem.OrderLocalDashboard,
            new DashboardItem(ListDashboard.SpecificProductSubsidiaryStockList)
        };

        public override bool CheckUser(User currentUser) => currentUser.HasAtLeastOnePermission((long)BasePermissionenum.WorkDashboard);

        public override string BreadcrumbCaption => "Меню работника";

        public override string IconGroup => "WorkIcons";
    }
}