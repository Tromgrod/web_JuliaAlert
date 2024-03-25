namespace Controls.CheckBox
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using LIB.Mvc;

    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Controls.CheckBox",
                 "Controls/CheckBox",
                 new { controller = "CheckBox", action = "Index" },
                 new[] { "Controls.CheckBox.Controllers" }
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
