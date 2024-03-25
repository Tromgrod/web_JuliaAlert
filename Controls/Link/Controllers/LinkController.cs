using System.Web.Mvc;

namespace Controls.Link.Controllers
{
    using Controls.Link.Models;

    public class LinkController : Controller
    {
        public ActionResult Default(object model)
        {
            return View("Default", (LinkModel)model);
        }
    }
}
