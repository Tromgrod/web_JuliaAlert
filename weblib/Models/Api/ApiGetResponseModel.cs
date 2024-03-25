using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weblib.Models.Api
{
    public class ApiGetResponseModel : ApiResponseModel
    {
        public int StartIndex { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
        public string Order { get; set; }
        public string OrderBy { get; set; }
    }
}
