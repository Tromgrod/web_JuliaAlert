using System.Web.Mvc;

namespace Controls.File.Controllers
{
    using Controls.File.Models;

    public class FileController : Controller
    {
        public ActionResult Default(object model)
        {
            return View("Default", (FileModel)model);
        }
    }
}
