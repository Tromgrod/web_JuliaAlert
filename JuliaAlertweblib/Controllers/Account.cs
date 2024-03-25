// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Account.cs" company="Natur Bravo">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the Account type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertweblib.Controllers
{
    using System.Web.Mvc;
    using JuliaAlertLib.BusinessObjects;

    public class Account : Weblib.Controllers.AccountController
    {
        public ActionResult Login(string returnUrl) => base.Login(returnUrl);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user, string returnUrl) => base.Login(user, returnUrl);

        public ActionResult CPLogin(string returnUrl) => base.CPLogin(returnUrl);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CPLogin(User user, string returnUrl) => base.CPLogin(user, returnUrl);

        public ActionResult SMILogin(string returnUrl) => base.SMILogin(returnUrl);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SMILogin(User user, string returnUrl) => base.SMILogin(user, returnUrl);
    }
}