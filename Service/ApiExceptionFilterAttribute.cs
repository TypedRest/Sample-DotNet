using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AddressBook;

/// <summary>
/// Reports exceptions with appropriate HTTP status codes.
/// </summary>
public class ApiExceptionFilterAttribute(IHostEnvironment env, ILogger<ApiExceptionFilterAttribute> logger) : ExceptionFilterAttribute
{
    private readonly bool _isDevelopment = env.IsDevelopment();

    public override void OnException(ExceptionContext context)
    {
        var (statusCode, logLevel) = GetStatusCodeAndLogLevel(context.Exception);
        var request = context.HttpContext.Request;

        context.HttpContext.Response.StatusCode = (int)statusCode;
        context.Result = BuildResult(context.Exception, statusCode);

        logger.Log(logLevel, context.Exception, "Responded to HTTP {Method} {Url} with {Status} due to exception",
            request.Method, request.GetEncodedPathAndQuery(), statusCode);

        base.OnException(context);
    }

    private static (HttpStatusCode, LogLevel) GetStatusCodeAndLogLevel(Exception exception)
        => exception switch
        {
            AuthenticationException _ => (HttpStatusCode.Unauthorized, LogLevel.Debug),
            UnauthorizedAccessException _ => (HttpStatusCode.Forbidden, LogLevel.Debug),
            InvalidDataException _ => (HttpStatusCode.BadRequest, LogLevel.Information),
            KeyNotFoundException _ => (HttpStatusCode.NotFound, LogLevel.Information),
            InvalidOperationException _ => (HttpStatusCode.Conflict, LogLevel.Warning),
            TimeoutException _ => (HttpStatusCode.RequestTimeout, LogLevel.Warning),
            _ => (HttpStatusCode.InternalServerError, LogLevel.Error)
        };

    private ObjectResult BuildResult(Exception exception, HttpStatusCode statusCode)
    {
        var problem = new ProblemDetails
        {
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Status = (int)statusCode
        };
        if (_isDevelopment)
            problem.Extensions["stackTrace"] = exception.StackTrace;

        return new ObjectResult(problem) {ContentTypes = {"application/problem+json"}};
    }
}
