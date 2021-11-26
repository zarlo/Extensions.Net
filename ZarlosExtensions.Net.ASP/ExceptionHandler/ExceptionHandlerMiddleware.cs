using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Diagnostics;

namespace ZarlosExtensions.Net.ASP
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly Func<object, Task> _clearCacheHeadersDelegate;
        private readonly DiagnosticListener _diagnosticListener;
        private readonly ExceptionHandlerManager _exceptionHandlerManager;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            DiagnosticListener diagnosticListener,
            ExceptionHandlerManager exceptionHandlerManager
            )
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ExceptionHandlerMiddleware>();
            _clearCacheHeadersDelegate = ClearCacheHeaders;
            _diagnosticListener = diagnosticListener;
            _exceptionHandlerManager = exceptionHandlerManager;
        }

        public Task Invoke(HttpContext context)
        {
            ExceptionDispatchInfo edi;
            try
            {
                var task = _next(context);
                if (!task.IsCompletedSuccessfully)
                {
                    return Awaited(this, context, task);
                }

                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                // Get the Exception, but don't continue processing in the catch block as its bad for stack usage.
                edi = ExceptionDispatchInfo.Capture(exception);
            }

            return HandleException(context, edi);

            static async Task Awaited(ExceptionHandlerMiddleware middleware, HttpContext context, Task task)
            {
                ExceptionDispatchInfo edi = null;
                try
                {
                    await task;
                }
                catch (Exception exception)
                {
                    // Get the Exception, but don't continue processing in the catch block as its bad for stack usage.
                    edi = ExceptionDispatchInfo.Capture(exception);
                }

                if (edi != null)
                {
                    await middleware.HandleException(context, edi);
                }
            }
        }

        private async Task HandleException(HttpContext context, ExceptionDispatchInfo edi)
        {

            // We can't do anything if the response has already started, just abort.
            if (context.Response.HasStarted)
            {
                edi.Throw();
            }

            PathString originalPath = context.Request.Path;

            try
            {

                var exceptionHandlerFeature = new ExceptionHandlerFeature()
                {
                    Error = edi.SourceException,
                    Path = originalPath.Value,
                };
                context.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);
                context.Features.Set<IExceptionHandlerPathFeature>(exceptionHandlerFeature);
                context.Response.StatusCode = 500;
                context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);

                await  _exceptionHandlerManager.ExceptionHandler(context);

                if (_diagnosticListener.IsEnabled() && _diagnosticListener.IsEnabled("ZarlosExtensions.Net.ASP.HandledException"))
                {
                    _diagnosticListener.Write("ZarlosExtensions.Net.ASP.HandledException", new { httpContext = context, exception = edi.SourceException });
                }

                // TODO: Optional re-throw? We'll re-throw the original exception by default if the error handler throws.
                return;
            }
            catch (Exception ex2)
            {

            }
            finally
            {
                context.Request.Path = originalPath;
            }

            edi.Throw(); // Re-throw the original if we couldn't handle it
        }

        private static Task ClearCacheHeaders(object state)
        {
            var headers = ((HttpResponse)state).Headers;
            headers[HeaderNames.CacheControl] = "no-cache";
            headers[HeaderNames.Pragma] = "no-cache";
            headers[HeaderNames.Expires] = "-1";
            headers.Remove(HeaderNames.ETag);
            return Task.CompletedTask;
        }
    }
}