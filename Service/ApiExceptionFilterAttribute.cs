using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AddressBook
{
    /// <summary>
    /// Reports exceptions with appropriate HTTP status codes.
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly bool _isDevelopment;
        private readonly ILogger<ApiExceptionFilterAttribute> _logger;

        public ApiExceptionFilterAttribute(IWebHostEnvironment env, ILogger<ApiExceptionFilterAttribute> logger)
        {
            _isDevelopment = env.IsDevelopment();
            _logger = logger;
        }

        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            var (statusCode, logLevel) = GetStatusCodeAndLogLevel(context.Exception);
            var request = context.HttpContext.Request;

            context.HttpContext.Response.StatusCode = (int)statusCode;
            context.Result = BuildResult(context.Exception, statusCode);

            _logger.Log(logLevel, context.Exception, "Responded to HTTP {0} {1} with {2} due to exception.",
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
}
