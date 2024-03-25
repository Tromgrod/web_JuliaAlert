using System;
using System.Collections.Generic;
using System.Web.Mvc;
using JuliaAlert.Models.Objects;
using LIB.Models;
using LIB.Tools.Utils;
using Weblib.Controllers;

namespace JuliaAlert.Controllers
{
    public class CustomReportController : BaseController
    {
        public ActionResult LoadCalendar(string TypeCalendar = "Сalendar_Order", string YearStr = "")
        {
            int.TryParse(YearStr, out int Year);

            Year = Year <= 0 ? DateTime.Today.Year : Year;

            Dictionary<DateTime, int> Counts;

            var Breadcrumbs = new List<LinkModel>();

            switch (TypeCalendar)
            {
                case "Сalendar_Order":
                    Counts = ProductForOrder.PopulateCountByYear(Year);
                    Breadcrumbs.Add(new LinkModel() { Caption = "Годовой отчет по возвратам", Href = URLHelper.GetUrl("CustomReport/CountOrder/Сalendar_Return/" + Year), Class = "button" });
                    break;

                case "Сalendar_Return":
                    Counts = Return.PopulateCountByYear(Year);
                    Breadcrumbs.Add(new LinkModel() { Caption = "Годовой отчет по продажам", Href = URLHelper.GetUrl("CustomReport/CountOrder/Сalendar_Order/" + Year), Class = "button" });
                    break;

                default:
                    throw new Exception("Unknown type calendar: " + TypeCalendar);
            }

            ViewData["YearForm"] = Year;
            ViewData["TypeCalendar"] = TypeCalendar;
            ViewData["Breadcrumbs"] = Breadcrumbs;

            return View("CountOrder", Counts);
        }

        public ActionResult UpdateCalendar()
        {
            int.TryParse(Request.Form["Year"], out int Year);
            int.TryParse(Request.Form["SalesChannel"], out int SalesChannelId);
            var TypeCalendar = Request.Form["TypeCalendar"];

            Year = Year <= 0 ? DateTime.Today.Year : Year;

            Dictionary<DateTime, int> Counts;

            var Breadcrumbs = new List<LinkModel>();

            switch (TypeCalendar)
            {
                case "Сalendar_Order":
                    Counts = ProductForOrder.PopulateCountByYear(Year, SalesChannelId > 0 ? new SalesChannel(SalesChannelId) : null);
                    Breadcrumbs.Add(new LinkModel() { Caption = "Годовой отчет по возвратам", Href = URLHelper.GetUrl("CustomReport/CountOrder/Сalendar_Return/" + Year), Class = "button" });
                    break;

                case "Сalendar_Return":
                    Counts = Return.PopulateCountByYear(Year, SalesChannelId > 0 ? new SalesChannel(SalesChannelId) : null);
                    Breadcrumbs.Add(new LinkModel() { Caption = "Годовой отчет по продажам", Href = URLHelper.GetUrl("CustomReport/CountOrder/Сalendar_Order/" + Year), Class = "button" });
                    break;

                default:
                    throw new Exception("Unknown type calendar: " + TypeCalendar);
            }

            ViewData["YearForm"] = Year;
            ViewData["SalesChannelId"] = SalesChannelId;
            ViewData["TypeCalendar"] = TypeCalendar;
            ViewData["Breadcrumbs"] = Breadcrumbs;

            return View(TypeCalendar, Counts);
        }
    }
}