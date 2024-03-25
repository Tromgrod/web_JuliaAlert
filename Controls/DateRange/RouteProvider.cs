namespace Controls.DateRange
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using LIB.Mvc;

    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Controls.DateRange",
                 "Controls/DateRange",
                 new { controller = "DateRange", action = "Index" },
                 new[] { "Controls.DateRange.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
