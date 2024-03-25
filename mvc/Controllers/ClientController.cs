using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Weblib.Controllers;
using JuliaAlert.Models.Objects;
using LIB.Tools.Security;
using LIB.Helpers;
using LIB.Tools.Utils;

namespace JuliaAlert.Controllers
{
    public class ClientController : BaseController
    {
        public ActionResult GetClitenData()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            long clientId = long.Parse(Request.Form["ClientId"]);

            var client = Client.PopulateById(clientId);

            var data = new Dictionary<string, object>
            {
                { nameof(Client), client },
                { nameof(Currency), client.GetCurrency() }
            };

            return this.Json(data);
        }

        public ActionResult ClientOrders()
        {
            long.TryParse(Request.Form["ClientId"], out long ClientId);
            long.TryParse(Request.Form["OrderId"], out var OrderId);

            ViewData["OrderId"] = OrderId;

            var Client = new Client(ClientId);

            switch (Client.GetCurrency().Id)
            {
                case (long)Currency.Enum.USD:
                case (long)Currency.Enum.EUR:
                    ViewData["OrderLink"] = "DocControl/Order";
                    break;

                case (long)Currency.Enum.MDL:
                    ViewData["OrderLink"] = "DocControl/Local_Order";
                    break;
            }

            var clientOrders = Order.PopulateByClient(Client);

            return this.View(clientOrders);
        }

        public ActionResult HasOrder()
        {
            long.TryParse(Request.Form["ClientId"], out long ClientId);

            return this.Json(new Client(ClientId).HasOrder());
        }
    }
}