namespace Controls.File
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using LIB.Mvc;

    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Controls.File",
                 "Controls/File",
                 new { controller = "File", action = "Index" },
                 new[] { "Controls.File.Controllers" }
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
