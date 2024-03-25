namespace JuliaAlert.Dashboards.UtilityModel
{
    public struct DashboardItem
    {
        public readonly static DashboardItem OrderDashboard = new DashboardItem(ListDashboard.OrderList, ChartDashboard.OrderChart);
        public readonly static DashboardItem OrderLocalDashboard = new DashboardItem(ListDashboard.OrderLocalList, ChartDashboard.OrderLocalChart);

        public DashboardItem(ListDashboard ListDashboard, ChartDashboard ChartDashboard = default)
        {
            this.ChartDashboard = ChartDashboard;
            this.ListDashboard = ListDashboard;
        }

        public readonly ChartDashboard ChartDashboard;
        public readonly ListDashboard ListDashboard;
    }
}