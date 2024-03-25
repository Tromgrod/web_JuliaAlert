using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using JuliaAlert.Dashboards;
using Weblib.Controllers;
using LIB.Tools.Security;
using LIB.Tools.Utils;
using System.Threading.Tasks;
using LIB.Models;
using LIB.BusinessObjects;

namespace JuliaAlert.Controllers
{
    public class DashBoardController : FrontEndController
    {
        private readonly BaseDashboard[] AllDashboards = new BaseDashboard[]
        {
            new AdminDashboard(),
            new SalesDashboard(),
            new ProductionDashboard(),
            new WorkDashboard()
        };

        public async Task<ActionResult> Index()
        {
            var currentUser = Authentication.GetCurrentUser(this.HttpContext);

            if (currentUser == null || currentUser.Id <= 0)
                return RedirectToAction("LogOff", "Account");

            var userDashboards = AllDashboards.Where(d => d.CheckUser(currentUser)).ToArray();

            var dashboardIndex = BaseDashboard.GetDashboardIndex(currentUser);

            var dashboard = BaseDashboard.GetDashboardByIndex(dashboardIndex, userDashboards);
            var breadcrumbs = BaseDashboard.GetBreadcrumbs(userDashboards);

            if (currentUser.HasAtLeastOnePermission((long)BasePermissionenum.SuperAdmin))
            {
                breadcrumbs.Add(this.GetStatisticBreadcrumb());
            }

            ViewData["IconsGroupType"] = dashboard.IconGroup;
            ViewData["Breadcrumbs"] = breadcrumbs;
            ViewData["DashboardIndex"] = dashboardIndex;
            ViewData["DashboardItemIndex"] = BaseDashboard.GetDashboardItemIndex(dashboardIndex, currentUser);

            var dashboardItems = await Task.Run(() => dashboard.DashboardItems);

            return View(dashboardItems);
        }

        public async Task<ViewResult> ChangeDashboard()
        {
            var currentUser = Authentication.GetCurrentUser(this.HttpContext);

            int.TryParse(Request.Form["DashboardIndex"], out var dashboardIndex);
            this.HttpContext.Session[currentUser.Id + "-" + SessionItems.DashboardIndex] = dashboardIndex;

            var userDashboards = AllDashboards.Where(d => d.CheckUser(currentUser)).ToArray();

            var dashboard = BaseDashboard.GetDashboardByIndex(dashboardIndex, userDashboards);

            ViewData["IconsGroupType"] = dashboard.IconGroup;
            ViewData["DashboardItemIndex"] = BaseDashboard.GetDashboardItemIndex(dashboardIndex, currentUser);

            var dashboardItems = await Task.Run(() => dashboard.DashboardItems);

            return View("Dashboard", dashboardItems);
        }

        public JsonResult ChangeDashboardItem()
        {
            var currentUser = Authentication.GetCurrentUser(this.HttpContext);

            int.TryParse(Request.Form["DashboardItemIndex"], out var dashboardItemIndex);
            var dashboardIndex = BaseDashboard.GetDashboardIndex(currentUser);

            this.HttpContext.Session[currentUser.Id + "-" + dashboardIndex + "-" + SessionItems.DashboardItemIndex] = dashboardItemIndex;

            var userDashboards = AllDashboards.Where(d => d.CheckUser(currentUser)).ToArray();

            var dashboard = BaseDashboard.GetDashboardByIndex(dashboardIndex, userDashboards);
            var dashboardItem = dashboard.DashboardItems[dashboardItemIndex];

            var dashboardItemInfo = new Dictionary<string, object>();

            var List = dashboardItem.ListDashboard;
            var ListData = List.Data;

            if (ListData != null && ListData.Count > 0)
                dashboardItemInfo.Add("ListView", this.RenderRazorViewToString($"~/Views/DashBoard/Lists/{List.ViewName}.cshtml", ListData));

            var Chart = dashboardItem.ChartDashboard;
            var ChartData = Chart.Data;

            if (ChartData != null && ChartData.Count > 0)
            {
                dashboardItemInfo.Add("ChartData", ChartData);
                dashboardItemInfo.Add("ChartLinkType", (long)Chart.PieChartTypeLink);
            }

            return this.Json(dashboardItemInfo);
        }

        private LinkModel GetStatisticBreadcrumb() => new LinkModel
        {
            Caption = "Статистика",
            Href = URLHelper.GetUrl("Statistic"),
            Class = "button"
        };
    }
}