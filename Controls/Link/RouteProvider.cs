namespace Controls.Link
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using LIB.Mvc;

    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Controls.Link",
                 "Controls/Link",
                 new { controller = "Link", action = "Index" },
                 new[] { "Controls.Link.Controllers" }
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
