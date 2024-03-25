using LIB.BusinessObjects;
using JuliaAlert.Dashboards.UtilityModel;

namespace JuliaAlert.Dashboards
{
    public class ProductionDashboard : BaseDashboard
    {
        public override DashboardItem[] DashboardItems => new[]
        {
            new DashboardItem(ListDashboard.SpecificProductMainStockList),
            new DashboardItem(ListDashboard.SpecificProductSubsidiaryStockList)
        };

        public override bool CheckUser(User currentUser) => currentUser.HasAtLeastOnePermission((long)BasePermissionenum.ProductionDashboard);

        public override string BreadcrumbCaption => "Производство";

        public override string IconGroup => "ProductionIcons";
    }
}