using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Web.Api.Middlewares
{
    internal sealed class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                await HandleError(context.Response, exception);
            }
        }

        private async Task HandleError(HttpResponse response, Exception exception)
        {
            response.ContentType = "application/json";

            var responseModel = new ErrorResponse(exception.Message);

            switch (exception)
            {
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Message = exception.Message;
                    break;
            }

            await response.WriteAsync(Serialize(responseModel));
        }

        private string Serialize(ErrorResponse responseModel)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.SerializeObject(responseModel, serializerSettings);
        }
    }
}