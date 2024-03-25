using JuliaAlert.Helpers;
using LIB.Models;
using LIB.Tools.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JuliaAlert.Controllers
{
    public class StatisticController : Controller
    {
        public ActionResult Index()
        {
            var breadcrumbs = new List<LinkModel>
            {
                new LinkModel
                {
                    Caption = "Главный экран",
                    Href = URLHelper.GetUrl(""),
                    Class = "button"
                }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View("SelectStatistic");
        }

        public ActionResult Map()
        {
            var breadcrumbs = new List<LinkModel>
            {
                new LinkModel
                {
                    Caption = "Главный экран",
                    Href = URLHelper.GetUrl(""),
                    Class = "button"
                },
                new LinkModel
                {
                    Caption = "Диаграммы",
                    Href = URLHelper.GetUrl("Statistic/Diagram"),
                    Class = "button"
                }
            };

            if (this.HttpContext.Session["StatisticMap_Years"] == null)
                this.HttpContext.Session["StatisticMap_Years"] = DateTime.Now.Year.ToString();

            if (this.HttpContext.Session["StatisticMap_MonthFrom"] == null)
                this.HttpContext.Session["StatisticMap_MonthFrom"] = 1;

            if (this.HttpContext.Session["StatisticMap_MonthTo"] == null)
                this.HttpContext.Session["StatisticMap_MonthTo"] = 12;

            if (this.HttpContext.Session["StatisticMap_CountingType"] == null)
                this.HttpContext.Session["StatisticMap_CountingType"] = 1;

            ViewData["Breadcrumbs"] = breadcrumbs;

            return View("Map");
        }

        public async Task<JsonResult> MapData(CancellationToken cancellationToken)
        {
            var mapData = await StatisticHelpers.GetMapDataAsync(cancellationToken);

            return Json(mapData);
        }

        public ActionResult Diagram()
        {
            var breadcrumbs = new List<LinkModel>
            {
                new LinkModel
                {
                    Caption = "Главный экран",
                    Href = URLHelper.GetUrl(""),
                    Class = "button"
                },
                new LinkModel
                {
                    Caption = "Карта",
                    Href = URLHelper.GetUrl("Statistic/Map"),
                    Class = "button"
                }
            };

            ViewData["Breadcrumbs"] = breadcrumbs;

            if (this.HttpContext.Session["StatisticDiagram_StatisticGroup"] == null)
                this.HttpContext.Session["StatisticDiagram_StatisticGroup"] = StatisticGroup.Sales;

            if (this.HttpContext.Session["StatisticDiagram_StatisticSqlGroup"] == null)
                this.HttpContext.Session["StatisticDiagram_StatisticSqlGroup"] = StatisticSqlGroup.Months;

            if (this.HttpContext.Session["StatisticDiagram_Years"] == null)
                this.HttpContext.Session["StatisticDiagram_Years"] = DateTime.Now.Year.ToString();

            if (this.HttpContext.Session["StatisticDiagram_MonthFrom"] == null)
                this.HttpContext.Session["StatisticDiagram_MonthFrom"] = 1;

            if (this.HttpContext.Session["StatisticDiagram_MonthTo"] == null)
                this.HttpContext.Session["StatisticDiagram_MonthTo"] = 12;

            if (this.HttpContext.Session["StatisticDiagram_CountingType"] == null)
                this.HttpContext.Session["StatisticDiagram_CountingType"] = 1;

            return View("Diagram");
        }

        public async Task<JsonResult> Search(CancellationToken cancellationToken)
        {
            string years = Request.Form["Years"];
            if (!int.TryParse(Request.Form["MonthFrom"], out var monthFrom))
                monthFrom = 1;
            if (!int.TryParse(Request.Form["MonthTo"], out var monthTo))
                monthTo = 12;
            if (!int.TryParse(Request.Form["CountingType"], out var countingType))
                countingType = 1;
            Enum.TryParse<StatisticGroup>(Request.Form["GroupBy"], out var statisticGroup);
            string dynamicFilterValues = Request.Form["DynamicFilter"];
            string uniqueProducts = Request.Form["UniqueProducts"];
            string salesChannels = Request.Form["SalesChannels"];
            string typeProducts = Request.Form["TypeProducts"];
            string countries = Request.Form["Countries"];

            StatisticSqlGroup statisticSqlGroup;

            if (years.Split(',').Count() > 3)
            {
                statisticSqlGroup = StatisticSqlGroup.Years;
            }
            else
            {
                if ((monthTo - monthFrom) >= 3)
                    statisticSqlGroup = StatisticSqlGroup.Months;
                else
                    statisticSqlGroup = StatisticSqlGroup.Days;
            }

            this.HttpContext.Session["StatisticDiagram_StatisticGroup"] = (int)statisticGroup;
            this.HttpContext.Session["StatisticDiagram_StatisticSqlGroup"] = (int)statisticSqlGroup;
            this.HttpContext.Session["StatisticDiagram_Years"] = years;
            this.HttpContext.Session["StatisticDiagram_MonthFrom"] = monthFrom;
            this.HttpContext.Session["StatisticDiagram_MonthTo"] = monthTo;
            this.HttpContext.Session["StatisticDiagram_DynamicFilterValues"] = dynamicFilterValues;
            this.HttpContext.Session["StatisticDiagram_CountingType"] = countingType;
            this.HttpContext.Session["StatisticDiagram_UniqueProducts"] = uniqueProducts;
            this.HttpContext.Session["StatisticDiagram_SalesChannels"] = salesChannels;
            this.HttpContext.Session["StatisticDiagram_TypeProducts"] = typeProducts;
            this.HttpContext.Session["StatisticDiagram_Countries"] = countries;

            var frameData = await StatisticHelpers.GetFrameDataAsync(cancellationToken, statisticGroup, statisticSqlGroup, years, monthFrom, monthTo, countingType, uniqueProducts, salesChannels, typeProducts, countries, dynamicFilterValues);
            var salesTypeChartData = await StatisticHelpers.GetSalesTypeChartDataAsync(cancellationToken, statisticGroup, years, monthFrom, monthTo, countingType, uniqueProducts, salesChannels, typeProducts, countries, dynamicFilterValues);
            var clientsBarData = await StatisticHelpers.GetBarDataAsync(cancellationToken, statisticGroup, years, monthFrom, monthTo, countingType, uniqueProducts, salesChannels, typeProducts, countries, dynamicFilterValues, BarType.Client);
            var productsBarData = await StatisticHelpers.GetBarDataAsync(cancellationToken, statisticGroup, years, monthFrom, monthTo, countingType, uniqueProducts, salesChannels, typeProducts, countries, dynamicFilterValues, BarType.Product);
            var typeProductsBarData = await StatisticHelpers.GetBarDataAsync(cancellationToken, statisticGroup, years, monthFrom, monthTo, countingType, uniqueProducts, salesChannels, typeProducts, countries, dynamicFilterValues, BarType.TypeProduct);
            var countriesBarData = await StatisticHelpers.GetBarDataAsync(cancellationToken, statisticGroup, years, monthFrom, monthTo, countingType, uniqueProducts, salesChannels, typeProducts, countries, dynamicFilterValues, BarType.Country);
            var salesChannelsBarData = await StatisticHelpers.GetBarDataAsync(cancellationToken, statisticGroup, years, monthFrom, monthTo, countingType, uniqueProducts, salesChannels, typeProducts, countries, dynamicFilterValues, BarType.SalesChannel);

            var statisticData = new
            {
                frameData,
                salesTypeChartData,
                clientsBarData,
                productsBarData,
                typeProductsBarData,
                countriesBarData,
                salesChannelsBarData
            };

            return Json(statisticData);
        }

        public enum StatisticGroup
        {
            [Display(Name = "Продажам")]
            Sales = 1,

            [Display(Name = "Моделям")]
            Product,

            [Display(Name = "Каналам продаж")]
            SalesChannel,

            [Display(Name = "Типам моделей")]
            TypeProduct,

            [Display(Name = "Странам")]
            Country
        }

        public enum StatisticSqlGroup
        {
            Years,
            Months,
            Days
        }

        public enum BarType
        {
            Product,
            TypeProduct,
            Client,
            SalesChannel,
            Country
        }
    }
}