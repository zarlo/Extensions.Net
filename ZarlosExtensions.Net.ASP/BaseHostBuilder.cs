using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Core.Enrichers;

namespace ZarlosExtensions.Net.ASP {

    public class BaseHostBuilder
    {
        public class BaseStartup 
        {

            public BaseStartup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                app.UseSerilogRequestLogging();
            }

        }
        public static int RunHostBuilder<T>(string[] args, Func<IHostBuilder, IHostBuilder> builder = null, bool SetLogger = true) where T : class
        {
            if(SetLogger)
            {
                Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            }

            var host = BaseHostBuilder.CreateHostBuilder<T>(args);
            if(builder != null)
                host = builder.Invoke(host);
            try
            {
                Log.Information("Starting web host");
                host.Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder<T>(string[] args) where T : class
        {
            return Host.CreateDefaultBuilder(args)
                    .UseSerilog()
                    .ConfigureAppConfiguration((builderContext, config) =>
                    {
                        var env = builderContext.HostingEnvironment;

                        config.SetBasePath(AppContext.BaseDirectory);
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                        
                        config.AddEnvironmentVariables();
                        config.AddCommandLine(args);
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {                        
                        webBuilder.UseStartup<BaseStartup>();
                        webBuilder.UseStartup<T>();
                    });
        }
    }


}
