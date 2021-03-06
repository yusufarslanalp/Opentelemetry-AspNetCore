using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using System.Threading;
using Jaeger;
using Microsoft.Extensions.Logging;

namespace ProjectA
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            
            services.AddHttpClient("ProjectB")
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5002"));

            services.AddOpenTelemetryTracing(builder =>
            {
                builder.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                //.AddProcessor()
                .AddJaegerExporter();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env )
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
                    Thread.Sleep(2000);

                    context.Response.Headers.Add("Request-Id", Activity.Current?.TraceId.ToString() ?? string.Empty);

                    using var client = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("ProjectB");
                    var content = client.GetStringAsync("/");

                    await context.Response.WriteAsync("Hello From Project A\n");
                    await context.Response.WriteAsync(await content);


                    

                    /*
                    content = client.GetStringAsync("/");
                    await context.Response.WriteAsync(await content);
                    content = client.GetStringAsync("/");
                    await context.Response.WriteAsync(await content);
                    content = client.GetStringAsync("/");
                    await context.Response.WriteAsync(await content);
                    */

                });

                endpoints.MapGet("/testAPI", async context =>
                {

                    using var client = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("ProjectB");
                    var fastResponse = client.GetStringAsync("/fast");
                    var slowResponse = client.GetStringAsync("/slow");
                    Task<HttpResponseMessage> greetResponse = client.GetAsync("/greet/error");

                    await context.Response.WriteAsync("Hello From Project A\n");
                    await context.Response.WriteAsync(await fastResponse);
                    await context.Response.WriteAsync(await slowResponse);

                    greetResponse.Wait();
                    bool success = greetResponse.Result.StatusCode == System.Net.HttpStatusCode.OK;
                    if ( success == false )
                    {
                        await context.Response.WriteAsync( "error in greeting" );
                    }
                    else
                    {
                        Task<string> taskStr = greetResponse.Result.Content.ReadAsStringAsync();
                        taskStr.Wait();

                        await context.Response.WriteAsync( taskStr.Result );
                    }
                    


                });

                endpoints.MapGet("/returnerror/{isError:alpha}", async context =>
                {
                    string isError = context.Request.RouteValues["isError"].ToString();
                    if( isError == "true" )
                    {
                        String nullStr = null;
                        Console.WriteLine( nullStr.Length );
                    }

                    

                    await context.Response.WriteAsync($"value of is error: {isError}");
                });


                endpoints.MapGet("/earthquakeAPI", async context =>
                {
                    Console.WriteLine("HEREEEEEEEEEEEEEEEEEEEEEEEE11");

                    using var client = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("name");
                    HttpResponseMessage response = await client.GetAsync("https://earthquake.usgs.gov/fdsnws/event/1/count?starttime=2020-09-24T08:03:57");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    await context.Response.WriteAsync("Hello From Project A\n");
                    await context.Response.WriteAsync(responseBody);
                });

            });
        }
    }
}
