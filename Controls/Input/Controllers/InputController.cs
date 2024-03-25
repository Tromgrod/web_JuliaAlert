using System.Web.Mvc;

namespace Controls.Input.Controllers
{
    using Controls.Input.Models;

    public class InputController : Controller
    {
        public ActionResult Default(object model)
        {
            if (((InputModel)model).TextBox.Type == Weblib.Models.Common.Enums.TextboxType.MultiLine)
            {
                return View("TextArea", (InputModel)model);
            }
            if (((InputModel)model).TextBox.Type == Weblib.Models.Common.Enums.TextboxType.HTML)
            {
                return View("HTML", (InputModel)model);
            }
            return View("Input", (InputModel)model);
        }
    }
}
