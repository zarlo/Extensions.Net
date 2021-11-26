using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace ZarlosExtensions.Net.Serilog
{


    public class TraceSerilogMiddleware  
    {
        private readonly RequestDelegate _next;

        public TraceSerilogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            context.Request.Headers.TryGetValue(context.TraceIdentifier, out StringValues correlationId);
            
            string Token = correlationId.FirstOrDefault();
            if(Token == null)
            {
                await _next.Invoke(context);
            }
            else
            {
                using (LogContext.PushProperty("CorrelationId", Token))
                {
                    await _next.Invoke(context);
                }
            }

        }
    }

    public static class WithTraceEx
    {

        public static void AddTraceToSerilog(this IApplicationBuilder app) {
            app.UseMiddleware<TraceSerilogMiddleware>();
            
        }

    }
}
