using System.Web.Mvc;
using System.Web.Routing;

namespace JuliaAlert
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "AccountLogin",
                url: "Account/{action}",
                defaults: new { controller = "Account" },
                namespaces: new[] { "JuliaAlert.Controllers" }
            );

            routes.MapRoute(
                name: "Account",
                url: "Account/{action}",
                defaults: new { controller = "Account" },
                namespaces: new[] { "Weblib.Controllers", "JuliaAlertweblib.Controllers" }
            );

            routes.MapRoute(
                "Print",
                "Print/Print",
                new { controller = "Print", action = "Print" },
                namespaces: new[] { "JuliaAlert.Controllers" }
            );

            routes.MapRoute(
                "BarcodePreviewPdf_CODE128",
                "BarcodeHelper/BarcodePreviewPdf_CODE128/{barcodeData}/{barcodeType}/{count}/{width}/{height}",
                new { controller = "BarcodeHelper", action = "BarcodePreviewPdf_CODE128", barcodeType = UrlParameter.Optional, count = UrlParameter.Optional, width = UrlParameter.Optional, height = UrlParameter.Optional },
                namespaces: new[] { "JuliaAlert.Controllers" }
            );

            routes.MapRoute(
                "BarcodePreviewPdf_EAN13",
                "BarcodeHelper/BarcodePreviewPdf_EAN13/{barcodeData}/{count}/{dateMoving}",
                new { controller = "BarcodeHelper", action = "BarcodePreviewPdf_EAN13", count = UrlParameter.Optional, dateMoving = UrlParameter.Optional },
                namespaces: new[] { "JuliaAlert.Controllers" }
            );

            routes.MapRoute(
                "JuliaAlertEditDelete",
                "DocControl/Delete",
                new { controller = "DocControl", action = "Delete" }
            );

            routes.MapRoute(
                "JuliaAlertEditSave",
                "DocControl/Save",
                new { controller = "DocControl", action = "Save"}
            );

            routes.MapRoute(
                "JuliaAlertEditUpdate",
                "DocControl/Update",
                new { controller = "DocControl", action = "Update" }
            );

            routes.MapRoute(
                "JuliaAlertEdit",
                "DocControl/{Model}/{Id}/{additional}",
                new { controller = "DocControl", action = "Edit", Id = UrlParameter.Optional, additional = UrlParameter.Optional }
            );

            routes.MapRoute(
                "StockProduct",
                "Stock/CountProduct/{Id}",
                new { controller = "Stock", action = "LoadGlobalStock", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "AutoComplete",
                "AutoComplete/{Namespace}",
                new { controller = "AutoComplete", action = "List", Namespace = UrlParameter.Optional }
            );

            routes.MapRoute(
                "MultySelect",
                "MultySelect/{Namespace}/{Param}",
                new { controller = "MultySelect", action = "Options", Namespace = UrlParameter.Optional, Param = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ErrorHandling",
                "Error/{action}",
                new { controller = "Error", action = "Index" }
            );

            routes.MapRoute(
                "ReportSearch",
                "Report/Search",
                new { controller = "Report", action = "Search" }
            );

            routes.MapRoute(
                "ReportPrint",
                "Report/Print",
                new { controller = "Report", action = "Print" }
            );

            routes.MapRoute(
                "ReportExcell",
                "Report/ExportExcell",
                new { controller = "Report", action = "ExportExcell" }
            );

            routes.MapRoute(
                "ReportCSV",
                "Report/ExportCSV",
                new { controller = "Report", action = "ExportCSV" }
            );

            routes.MapRoute(
                "ReportOptions",
                "Report/Options/{Model}",
                new { controller = "Report", action = "Options", Model = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ReportOptionsSave",
                "Report/OptionsSave/{Model}",
                new { controller = "Report", action = "OptionsSave", Model = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Select_Statistic",
                "Statistic",
                new { controller = "Statistic", action = "Index" }
            );

            routes.MapRoute(
                "Report_Min",
                "Report/View_Min",
                new { controller = "Report", action = "View_Min" }
            );

            routes.MapRoute(
                "Report",
                "Report/{Model}/{BOLinks}/{NamespaceLinks}/{Ids}",
                new { controller = "Report", action = "View", BOLinks = UrlParameter.Optional, NamespaceLinks = UrlParameter.Optional, Ids = UrlParameter.Optional }
            );

            routes.MapRoute(
                "CustomReport",
                "CustomReport/CountOrder/{TypeCalendar}/{YearStr}",
                new { controller = "CustomReport", TypeCalendar = UrlParameter.Optional, YearStr = UrlParameter.Optional, action = "LoadCalendar" }
            );

            routes.MapRoute(
                "Statistic_Diagram",
                "Statistic/Diagram",
                new { controller = "Statistic", action = "Diagram" }
            );

            routes.MapRoute(
                name: "CPProfile",
                url: "ControlPanel/Profile/{Id}",
                defaults: new { controller = "ControlPanel", action = "Profile", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "CPTable",
                url: "ControlPanel/Edit/{BO}/{Namespace}/{BOLink}/{NamespaceLink}/{Id}",
                defaults: new { controller = "ControlPanel", action = "Edit", BO = UrlParameter.Optional, Namespace = UrlParameter.Optional, BOLink = UrlParameter.Optional, NamespaceLink = UrlParameter.Optional, Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Create",
                url: "ControlPanel/CreateItem/{BO}/{Namespace}/{BOLink}/{NamespaceLink}/{Id}",
                defaults: new { controller = "ControlPanel", action = "CreateItem", BO = UrlParameter.Optional, Namespace = UrlParameter.Optional, BOLink = UrlParameter.Optional, NamespaceLink = UrlParameter.Optional, Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Edit",
                url: "ControlPanel/{action}/{BO}/{Namespace}/{Id}",
                defaults: new { controller = "ControlPanel", action = "Edit", BO = UrlParameter.Optional, Namespace = UrlParameter.Optional, Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SMIProfile",
                url: "SystemManagement/Profile/{Id}",
                defaults: new { controller = "SystemManagement", action = "Profile", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SMITable",
                url: "SystemManagement/Edit/{BO}/{Namespace}/{BOLink}/{NamespaceLink}/{Id}",
                defaults: new { controller = "SystemManagement", action = "Edit", BO = UrlParameter.Optional, Namespace = UrlParameter.Optional, BOLink = UrlParameter.Optional, NamespaceLink = UrlParameter.Optional, Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SMICreate",
                url: "SystemManagement/CreateItem/{BO}/{Namespace}/{BOLink}/{NamespaceLink}/{Id}",
                defaults: new { controller = "SystemManagement", action = "CreateItem", BO = UrlParameter.Optional, Namespace = UrlParameter.Optional, BOLink = UrlParameter.Optional, NamespaceLink = UrlParameter.Optional, Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SMIEdit",
                url: "SystemManagement/{action}/{BO}/{Namespace}/{Id}",
                defaults: new { controller = "SystemManagement", action = "Edit", BO = UrlParameter.Optional, Namespace = UrlParameter.Optional, Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LoadDataGrid",
                url: "DataProcessor/Load/{BO}/{Namespace}/{BOLink}/{NamespaceLink}/{Id}",
                defaults: new { controller = "DataProcessor", action = "Load", BO = UrlParameter.Optional, Namespace = UrlParameter.Optional, BOLink = UrlParameter.Optional, NamespaceLink = UrlParameter.Optional, Id = UrlParameter.Optional }
            );          

            routes.MapRoute(
                name: "Home",
                url: "{controller}/{action}",
                defaults: new { controller = "DashBoard", action = "Index" }
            );

            routes.MapRoute(
                name: "Error",
                url: "Error",
                defaults: new { controller = "Error", action = "Index" },
                namespaces: new[] { "JuliaAlert.Controllers" }
            );

            routes.MapRoute(
                name: "ErrorHandle",
                url: "Error/{action}",
                defaults: new { controller = "Error" },
                namespaces: new[] { "JuliaAlert.Controllers" }
            );
        }
    }
}