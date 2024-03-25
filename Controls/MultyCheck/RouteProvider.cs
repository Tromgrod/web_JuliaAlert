namespace Controls.MultyCheck
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using LIB.Mvc;

    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Controls.MultyCheck",
                 "Controls/MultyCheck",
                 new { controller = "MultyCheck", action = "Index" },
                 new[] { "Controls.MultyCheck.Controllers" }
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
