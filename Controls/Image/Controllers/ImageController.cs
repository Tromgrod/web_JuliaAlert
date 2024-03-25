using System.Web.Mvc;

namespace Controls.Image.Controllers
{
    using Controls.Image.Models;

    public class ImageController : Controller
    {
        public ActionResult Default(object model)
        {
            return View("Default", (ImageModel)model);
        }
    }
}
