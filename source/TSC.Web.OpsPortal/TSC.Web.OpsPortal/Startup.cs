using System;
using System.IO;
using System.Reflection;
using Hangfire;
using Hangfire.JobsLogger;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TCS.Web.OpsPortal;
using TSC.Web.OpsPortal.Common;

namespace TSC.Web.OpsPortal
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly Settings _settings;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _settings = CreateSettingsFromConfiguration(configuration);

        }


        public IConfiguration Configuration { get; }

        private Settings CreateSettingsFromConfiguration(IConfiguration configuration)
        {
            var settings = new Settings();
            configuration.Bind(settings);

            return settings;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "XsltTransform  API",
                    Version = "v1",
                    Description = "The XsltTransform API used to manage the XsltTransform  via a REST endpoint"
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            var mvcBuilder = services.AddControllersWithViews();

            // enable razor compilation during local development so razor recompiles views on page save
            if (_env.IsDevelopment()) mvcBuilder.AddRazorRuntimeCompilation();

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                config.UseColouredConsoleLogProvider();
                config.UseSimpleAssemblyNameTypeSerializer();
                config.UseDefaultTypeSerializer();
                config.UseRecommendedSerializerSettings();
                config.UseJobsLogger();
                config.UseSqlServerStorage(_settings.ConnectionStrings.HangfireComposition, GetSqlServerStorageOptions());
            });

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseHangfireDashboard("/HangfireComposition", new DashboardOptions()
            {
                DashboardTitle = "TCS Composition",
                DisplayStorageConnectionString = true,
                Authorization = new[] { new HangfireAuthorizationFilter() { AllowedGroupNamesConfigKey = "AuthorizedGlobalGroups" } }
            }, new SqlServerStorage(_settings.ConnectionStrings.HangfireComposition));

            app.UseHangfireDashboard("/HangfireDelivery", new DashboardOptions()
            {
                DashboardTitle = "TCS Delivery",
                DisplayStorageConnectionString = true,
                Authorization = new[] { new HangfireAuthorizationFilter() { AllowedGroupNamesConfigKey = "AuthorizedGlobalGroups" } }
            }, new SqlServerStorage(_settings.ConnectionStrings.HangfireDelivery));


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        SqlServerStorageOptions GetSqlServerStorageOptions()
        {
            return new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                UsePageLocksOnDequeue = true,
                DisableGlobalLocks = true
            };
        }
    }
}
