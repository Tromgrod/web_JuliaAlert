using System.Web.Mvc;

namespace Controls.CheckBox.Controllers
{
    using Controls.CheckBox.Models;

    public class CheckBoxController : Controller
    {
        public ActionResult Default(object model)
        {
            return View("Default", (CheckBoxModel)model);
        }
    }
}
