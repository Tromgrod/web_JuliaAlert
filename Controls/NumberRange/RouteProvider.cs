namespace Controls.NumberRange
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using LIB.Mvc;

    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Controls.NumberRange",
                 "Controls/NumberRange",
                 new { controller = "NumberRange", action = "Index" },
                 new[] { "Controls.NumberRange.Controllers" }
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
