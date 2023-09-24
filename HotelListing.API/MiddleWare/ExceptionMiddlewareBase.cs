using System.Net;

namespace HotelListing.API.MiddleWare
{
    public class ExceptionMiddlewareBase
    {

        private Task HandleExceptionAysnc(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            var errorDetals = new ErrorDetails
            {
                ErrorType = "Failer",
                ErrorMessage = ex.Message,
            };
            switch (ex)
            {
                case DirectoryNotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorDetals.ErrorType = "Not Found";
                    break;

                default:
                    break;
            }
            string response = Newtonsoft.Json.JsonConvert.SerializeObject(errorDetals);
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(response);
        }
    }
}