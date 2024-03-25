using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weblib.Models.Api
{
    public class ApiResponseModel
    {
        public object Data { get; set; }
        public ApiStatus Status { get; set; } = ApiStatus.Success;
        private string _message;
        public string Message
        {
            get => string.IsNullOrEmpty(_message) ? Status.ToString() : _message;
            set => _message = value;
        }
        public string Error { get; set; }

        public ApiResponseModel()
        {
            
        }

        public ApiResponseModel(object data, ApiStatus status, string message, string error)
        {
            Data = data;
            Status = status;
            Message = message;
            Error = error;
        }

        public enum ApiStatus
        {
            Success,
            Error
        }
    }
}
