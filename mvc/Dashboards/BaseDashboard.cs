using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using LIB.BusinessObjects;
using LIB.Tools.Utils;
using LIB.Models;
using JuliaAlert.Dashboards.UtilityModel;

namespace JuliaAlert.Dashboards
{
    public abstract class BaseDashboard
    {
        public abstract DashboardItem[] DashboardItems { get; }

        public abstract string BreadcrumbCaption { get; }

        public abstract string IconGroup { get; }

        public abstract bool CheckUser(User currentUser);

        private protected virtual LinkModel GetDashboardBreadcrumb(int index) => new LinkModel()
        {
            Caption = this.BreadcrumbCaption,
            Value = index.ToString(),
            Action = $"change_dashboard(this, {index})",
            Class = "button"
        };

        public static List<LinkModel> GetBreadcrumbs(in BaseDashboard[] userDashboards)
        {
            var breadcrumbs = new List<LinkModel>();

            for (int index = default; index < userDashboards.Length; index++)
            {
                var userDashboard = userDashboards[index];

                if (userDashboard != default)
                {
                    var breadcrumb = userDashboard.GetDashboardBreadcrumb(index);
                    breadcrumbs.Add(breadcrumb);
                }
            }

            return breadcrumbs;
        }

        public static int GetDashboardIndex(User currentUser, HttpContextBase httpContext = null)
        {
            httpContext = httpContext ?? new HttpContextWrapper(HttpContext.Current);

            int dashboardIndex = default;

            var dashboardIndexObj = httpContext.Session[currentUser.Id + "-" + SessionItems.DashboardIndex];

            if (dashboardIndexObj != null)
                dashboardIndex = Convert.ToInt32(dashboardIndexObj);

            return dashboardIndex;
        }

        public static int GetDashboardItemIndex(int dashboardIndex, User currentUser, HttpContextBase httpContext = null)
        {
            httpContext = httpContext ?? new HttpContextWrapper(HttpContext.Current);

            int dashboardItemIndex = default;

            var dashboardItemIndexObj = httpContext.Session[currentUser.Id + "-" + dashboardIndex + "-" + SessionItems.DashboardItemIndex];

            if (dashboardItemIndexObj != null)
                dashboardItemIndex = Convert.ToInt32(dashboardItemIndexObj);

            return dashboardItemIndex;
        }

        public static BaseDashboard GetDashboardByIndex(int dashboardIndex, in BaseDashboard[] userDashboards)
        {
            var dashboard = userDashboards[dashboardIndex] != default ? userDashboards[dashboardIndex] : userDashboards.FirstOrDefault(d => d != default);

            return dashboard;
        }
    }
}