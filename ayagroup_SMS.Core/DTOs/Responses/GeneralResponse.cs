using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ayagroup_SMS.Core.DTOs.Responses
{
    public class GeneralResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public object Data { get; set; }

        public GeneralResponse(string message = null, bool success = true, int statusCode = 200, object data = null)
        {
            Message = message ?? (success ? "Success" : "Failure");
            Success = success;
            StatusCode = statusCode;
            Data = data;
        }
        public static GeneralResponse Ok(string message = "Success", object data = null) => new GeneralResponse(message, true, 200, data);
        public static GeneralResponse BadRequest(string message = "Bad Request", object data = null) => new GeneralResponse(message, false, 400, data);
        public static GeneralResponse NotFound(string message = "Not Found", object data = null) => new GeneralResponse(message, false, 404, data);
        public static GeneralResponse Unauthorized(string message = "Unauthorized", object data = null) => new GeneralResponse(message, false, 401, data);
        public static GeneralResponse InternalError(string message = "Internal Server Error", object data = null) => new GeneralResponse(message, false, 500, data);
    }
}
