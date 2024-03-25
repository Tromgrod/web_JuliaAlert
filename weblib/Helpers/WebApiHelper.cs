using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Weblib.Helpers
{
    public static class WebApiHelper
    {
        public static HttpConfiguration CamelCaseConfiguration
        {
            get
            {
                var config = new HttpConfiguration();
                var jsonSerializerSettings = config.Formatters.JsonFormatter.SerializerSettings;
                jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

                return config;
            }
        }
    }
}
