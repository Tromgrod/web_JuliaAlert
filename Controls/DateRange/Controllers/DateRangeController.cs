using System.Web.Mvc;

namespace Controls.DateRange.Controllers
{
    using Controls.DateRange.Models;

    public class DateRangeController : Controller
    {
        public ActionResult Default(object model)
        {
            return View("Default", (DateRangeModel)model);
        }
    }
}
