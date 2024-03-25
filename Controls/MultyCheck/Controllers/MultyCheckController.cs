using System.Web.Mvc;

namespace Controls.MultyCheck.Controllers
{
    using Controls.MultyCheck.Models;

    public class MultyCheckController : Controller
    {
        public ActionResult Default(object model)
        {
            return View("Default", (MultyCheckModel)model);
        }
    }
}
