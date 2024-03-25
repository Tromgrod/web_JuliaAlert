namespace JuliaAlertweblib.Controllers
{
    using System.Collections.Generic;
    using JuliaAlertLib.BusinessObjects;
    using LIB.Tools.Security;

    public class ReportObjectController : FrontEndController
    {
        public ReportObjectController()
        {
            var usr = Authentication.GetCurrentUser();
            if (usr != null)
            {
                Dictionary<long, MenuGroup> menues;
                if (Session!=null && Session["MainMenu"] != null)
                {
                    menues = (Dictionary<long, MenuGroup>)Session["MainMenu"];
                }
                else
                {
                    menues = MenuGroup.Populate(usr);
                    if (Session != null)
                        Session["MainMenu"] = menues;
                }
                ViewData["MainMenu"] = menues;
            }
        }
    }
}