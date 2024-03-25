using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JuliaAlert.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            return View("Error");
        }
        public ActionResult NotFound()
        {
            return View("Not_Found");
        }
        public ActionResult Unavailable()
        {
            return View("Unavailable");
        }
        public ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
	}
}