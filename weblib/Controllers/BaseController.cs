using System.Linq;
using System.IO;
using System.Web.Mvc;
using Weblib.Helpers;
using LIB.BusinessObjects;
using LIB.Tools.Utils;

namespace Weblib.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            Language CurrentLanguage;

            if (System.Web.HttpContext.Current.Session[SessionItems.Language] == null)
            {
                CurrentLanguage = (Language)new Language().Populate().Values.FirstOrDefault();
                System.Web.HttpContext.Current.Session[SessionItems.Language] = CurrentLanguage;
            }
            else
                CurrentLanguage = (Language)System.Web.HttpContext.Current.Session[SessionItems.Language];

            CultureHelper.Language = CurrentLanguage;
        }

        public static readonly string BadResponse = "Bad https";

        protected string RenderRazorViewToString(string viewName, object model = null)
        {
            if (model != null)
                ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}