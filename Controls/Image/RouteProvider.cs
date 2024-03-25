namespace Controls.Image
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using LIB.Mvc;

    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Controls.Image",
                 "Controls/Image",
                 new { controller = "Image", action = "Index" },
                 new[] { "Controls.Image.Controllers" }
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
