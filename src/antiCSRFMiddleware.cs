using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace antiCSRFTest.Middleware
{
    public class antiCSRFMiddleware
    {
        private readonly RequestDelegate _next;

        public antiCSRFMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            
            //All other code is in a helper static helper class right now. Will eventually evolve to more proper implementation.
            httpContext = antiCSRFMiddlewareHelper(httpContext);

            return Task.CompletedTask;

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class antiCSRFMiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<antiCSRFMiddleware>();
        }
    }
}
