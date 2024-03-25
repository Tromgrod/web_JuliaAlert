using System.Web.Mvc;

namespace Controls.NumberRange.Controllers
{
    using Controls.NumberRange.Models;

    public class NumberRangeController : Controller
    {
        public ActionResult Default(object model)
        {
            return View("Default", (NumberRangeModel)model);
        }
    }
}
