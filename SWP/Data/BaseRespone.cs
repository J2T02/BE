using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SWP.Data
{
    public class BaseRespone<T>
    {

        public bool Success { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }

        public object Error { get; set; }

        public BaseRespone(T data, string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            Success = true;
            StatusCode = statusCode;
            Data = data;
            Message = message;
            Error = null;
        }

        public BaseRespone(HttpStatusCode statusCode, string message, object error = null)
        {
            Success = false;
            StatusCode = statusCode;
            Data = default(T);
            Message = message;
            Error = error;
        }

        public static BaseRespone<T> SuccessResponse(T data, string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new BaseRespone<T>(data, message, statusCode);
        }
        public static BaseRespone<T> ErrorResponse( string message, object error = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new BaseRespone<T>(statusCode, message, error);

        }
        public static BaseRespone<T> ResponseShow<T>(
            T entity,
            string successMessage,
            string failureMessage,
            object? error = null)
        {
            if (entity == null)
            {
                return BaseRespone<T>.ErrorResponse(failureMessage, error, HttpStatusCode.NotFound);
            }

            return BaseRespone<T>.SuccessResponse(entity, successMessage, HttpStatusCode.OK);
        }
    }
}
