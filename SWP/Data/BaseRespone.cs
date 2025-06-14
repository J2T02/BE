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


    }
}
