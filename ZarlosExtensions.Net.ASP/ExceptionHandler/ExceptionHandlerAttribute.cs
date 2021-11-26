using System;

namespace ZarlosExtensions.Net.ASP
{

    public enum ExceptionHandlerScope {
        Local,
        Global
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ExceptionHandlerAttribute: Attribute
    {

        public Type ExceptionType { get; private set; }
        public ExceptionHandlerScope Scope { get; private set; }

        public ExceptionHandlerAttribute(Type ExceptionType, ExceptionHandlerScope Scope = ExceptionHandlerScope.Global)
        {
            this.Scope = Scope;
            this.ExceptionType = ExceptionType;
        }

    }
}
