using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace ZarlosExtensions.Net.ASP
{
    public class ExceptionHandlerManager
    {

        public Dictionary<Type, Func<dynamic>> Handlers { get; private set; }
        public Func<dynamic> Default { get; private set; }

        public void SetDefault(Func<dynamic> callback)
        {
            Default = callback;
        }
        public void AddHandler(Type exception, Func<dynamic> callback)
        {
            Handlers.Add(exception, callback);
        }

        public async Task<dynamic> ExceptionHandler(HttpContext context)
        {

            var errorType = context.Features.Get<IExceptionHandlerFeature>().Error.GetType();

            if(Handlers.ContainsKey(errorType))
            {
                return Handlers[errorType].Invoke();
            }
            else
            {
                return Default.Invoke();
            }

        }

        

    }
}