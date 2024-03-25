namespace Controls.Input
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using LIB.Mvc;

    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Controls.Input",
                 "Controls/Input",
                 new { controller = "Input", action = "Index" },
                 new[] { "Controls.Input.Controllers" }
            );
        }
        public int Priority => 0;
    }
}