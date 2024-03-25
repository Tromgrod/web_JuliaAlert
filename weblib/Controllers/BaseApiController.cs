using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Weblib.Helpers;
using Weblib.Models.Api;

namespace Weblib.Controllers
{
    public class BaseApiController : ApiController
    {
        public HttpResponseMessage CreateResponse(ApiResponseModel response, HttpStatusCode code = HttpStatusCode.OK)
        {
            return Request.CreateResponse(code, response, WebApiHelper.CamelCaseConfiguration);
        }
    }
}
