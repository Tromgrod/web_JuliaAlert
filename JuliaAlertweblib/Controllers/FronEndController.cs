namespace JuliaAlertweblib.Controllers
{
    using LIB.Tools.Security;
    using LIB.Tools.Utils;
    using Weblib.Helpers;

    [AuthAction]
    public class FrontEndController : Weblib.Controllers.FrontEndController
    {
        public FrontEndController()
        {
            if (System.Web.HttpContext.Current.Session[SessionItems.Person] == null && Authentication.GetCurrentUser()!=null)
            {
                JuliaAlertLib.BusinessObjects.Person.AddPersonInfo(Authentication.GetCurrentUser());
            }
            ViewBag.Title = "JuliaAlert";
        }
    }
}