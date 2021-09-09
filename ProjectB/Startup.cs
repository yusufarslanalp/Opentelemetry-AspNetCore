using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectB
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            services.AddOpenTelemetryTracing(builder =>
            {
                builder.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddJaegerExporter();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.Headers.Add("Request-Id", Activity.Current?.TraceId.ToString() ?? string.Empty);

                    await Task.Delay(new Random().Next(100, 1000));
                    Activity.Current?.AddEvent(new ActivityEvent("HelloProject", DateTimeOffset.UtcNow));
                    await context.Response.WriteAsync("Hello From Project B");
                });
                endpoints.MapGet("/fast", async context =>
                {
                    await context.Response.WriteAsync("Very fast response\n");
                });
                endpoints.MapGet("/slow", async context =>
                {
                    Thread.Sleep( 2000 );
                    await context.Response.WriteAsync("Slow response\n");
                });
                endpoints.MapGet("/greet/{name:alpha}", async context =>
                {
                    string name = context.Request.RouteValues["name"].ToString();
                    if (name == "error")
                    {
                        throw new Exception();
                    }
                    await context.Response.WriteAsync($"Hello {name}\n");
                });

            });
        }
    }
}
