using Microsoft.AspNetCore.Diagnostics;
using WebApi.Model;

namespace WebApi.Core.SharedFramework
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            this.logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            this.logger.LogError(
                "TimeStamp: { timestamp }, Exception { exception }",
                DateTimeOffset.Now, exception);

            GeneralErrorResponseBody responseBody = new GeneralErrorResponseBody()
            {
                InnerException = exception.InnerException?.Message,
                Message = exception.Message,
                RuntimeExceptionType = exception.GetType().Name,
                TargetSite = exception.TargetSite?.Name
            };

            switch (exception)
            {
                case UnauthorizedAccessException:
                    {
                        responseBody.StatusCode = StatusCodes.Status401Unauthorized;
                        break;
                    }
                case InvalidOperationException:
                case InvalidDataException:
                case ArgumentNullException:
                    {
                        responseBody.StatusCode = StatusCodes.Status400BadRequest;
                        break;
                    }
                case KeyNotFoundException:
                    {
                        responseBody.StatusCode = StatusCodes.Status404NotFound;
                        break;
                    }
                default:
                    {
                        responseBody.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                    }
            }

            httpContext.Response.StatusCode = responseBody.StatusCode;

            await httpContext
                .Response
                .WriteAsJsonAsync(responseBody, cancellationToken);


            // Return false to continue with the default behavior
            // - or - return true to signal that this exception is handled
            return await ValueTask.FromResult(true);
        }
    }
}