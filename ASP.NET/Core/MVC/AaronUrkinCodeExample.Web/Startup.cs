using AaronUrkinCodeExample.BusinessLogicLayer.Extensions;
using AaronUrkinCodeExample.BusinessLogicLayer.Localization;
using AaronUrkinCodeExample.BusinessLogicLayer.Middleware;
using AaronUrkinCodeExample.BusinessLogicLayer.Services;
using AaronUrkinCodeExample.BusinessLogicLayer.Services.Logger;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using System.Linq;

namespace AaronUrkinCodeExample.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var localizationSection = this.Configuration.GetSection("Localization");
            var resourcesPath = localizationSection.GetValue<string>("ResourcesPath");
            var connectionStrings = this.Configuration.GetSection("ConnectionStrings").AsEnumerable(true).ToDictionary(i => i.Key, i => i.Value);

            services.AddAutoMapper();
            Mapper.AssertConfigurationIsValid();

            services.Configure<LocalizationConfig>(localizationSection);
            services.Configure<ApplicationErrorHandlerOptions>(this.Configuration.GetSection("ErrorHandler"));

            // Add and configure business logic services
            services.AddSecurityDbContext(connectionStrings);
            services.AddLocalizationDbContext(connectionStrings);
            services.AddLoggerDbContext(connectionStrings, this.Configuration["NLog:DbSchema"]);
            services.AddApplicationIdentity();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<ILoggerService, LoggerService>();

            /*
            There are two ways to localize the app:

                1.  From .json files resources.
                2.  From database 'Translations' table.

                To localize from database, replace services.AddJsonLocalization(localizationSection.Get<LocalizationConfig>());
                with services.AddDatabaseLocalization(localizationSection.Get<LocalizationConfig>());
            */
            services.AddJsonLocalization(localizationSection.Get<LocalizationConfig>());

            // Add and configure MVC and localization
            services
                .AddMvc(options => options.Filters.Insert(0, new MiddlewareFilterAttribute(typeof(LocalizationPipeline))))
                .AddViewLocalization(o => { o.ResourcesPath = resourcesPath; })
                .AddDataAnnotationsLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, RequestLocalizationOptions options)
        {
            this.ConfigureNLog(env, loggerFactory);

            app.UseApplicationErrorHandler();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvcWithLocalizedRoutes(options);

            this.MigrateDatabases(app);
        }

        // Configures NLog to save logs into database
        private void ConfigureNLog(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();

            env.ConfigureNLog("nlog.config");

            LogManager.Configuration.Variables["connectionString"] = this.Configuration.GetConnectionString("ExampleAppDatabase");

            LogManager.Configuration.Variables["logTableName"] = string.IsNullOrEmpty(this.Configuration["NLog:DbSchema"])
                ? this.Configuration["NLog:Table"]
                : $"{this.Configuration["NLog:DbSchema"]}.{this.Configuration["NLog:Table"]}";
        }

        // Invokes database migrations on application startup
        private void MigrateDatabases(IApplicationBuilder app)
        {
            app.UseSecurityMigrations();
            app.UseLocalizationMigrations();
            app.UseLoggerMigrations();
        }
    }
}
