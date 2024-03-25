using System.Web.Mvc;

namespace Controls.Select.Controllers
{
    using Controls.Select.Models;

    public class SelectController : Controller
    {
        public ActionResult Default(object model)
        {
            if (model is SelectListModel)
                return View("SelectList", ((SelectListModel)model));

            if (((SelectModel)model).MultyCheckModel != null)
                return View("MultyCheck", ((SelectModel)model).MultyCheckModel);

            if (((SelectModel)model).DropDown.Multiple)
                return View("MultiSelect", (SelectModel)model);

            return View("Select", (SelectModel)model);
        }
    }
}
