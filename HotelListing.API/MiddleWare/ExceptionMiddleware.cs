using System.Net;
namespace HotelListing.API.MiddleWare
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something Went while processing {context.Request.Path}");
                await HandleExceptionAysnc(context, ex);
            }
        }

        private Task HandleExceptionAysnc(HttpContext context, Exception ex)
        {
            context.Response.ContentType= "application/json";
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            var errorDetals = new ErrorDetails
            {
                ErrorType =  "Failer",
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
            string response  = Newtonsoft.Json.JsonConvert.SerializeObject(errorDetals);
            context.Response.StatusCode = (int)statusCode;
         
            return context.Response.WriteAsync(response);
        }
    }

    public class ErrorDetails
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
