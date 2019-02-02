using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using AntiCSRFTest.Middleware;
namespace AntiCSRFTest
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Catches any Request where method is GET or POST. GET and POST handling are the current scope for this.
            //Could certainly be extended to other methods down the road if implemented within a web api.
            //NOTE: UseWhen allows further middleware to be executed
            //if and only if the next one is invoked. This allows for the charactaristics of an authentication filter that only lets through
            //Authenticated Requests, if we choose to not invoke _next. UseWhen allows us to create a request branch and rejoin if it passes our
            //antiCSRF filter.
            app.UseWhen(context => (context.Request.Method.Equals("GET") || context.Request.Method.Equals("POST")), appBuilder =>
            {
                appBuilder.UseMiddleware<AntiCSRFMiddleware>();
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("You passed the antiCSRF filter!");
            });
        }
    }
}
