using AaronUrkinCodeExample.BusinessLogicLayer.Localization;
using AaronUrkinCodeExample.BusinessLogicLayer.Services.Localization;
using AaronUrkinCodeExample.BusinessLogicLayer.Services.Security;
using AaronUrkinCodeExample.DataAccessLayer.Localization;
using AaronUrkinCodeExample.DataAccessLayer.Logger;
using AaronUrkinCodeExample.DataAccessLayer.Security;
using AaronUrkinCodeExample.DataAccessLayer.Security.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds security entities to database
        /// </summary>
        /// <param name="services">Services collection to add application services to.</param>
        /// <param name="connectionStrings">Connection strings collection from applicatiion settings</param>
        /// <returns>Extended services collection</returns>
        public static IServiceCollection AddSecurityDbContext(this IServiceCollection services, IDictionary<string, string> connectionStrings)
        {
            return services.AddDbContext<SecurityDbContext>(options =>
                options.UseSqlServer(connectionStrings["ExampleAppDatabase"], serverOptions =>
                    serverOptions.MigrationsHistoryTable(SecurityDbContext.MigrationTableName, SecurityDbContext.DefaultSchema)));
        }

        /// <summary>
        /// Adds localization entities to database
        /// </summary>
        /// <param name="services">Services collection to add application services to.</param>
        /// <param name="connectionStrings">Connection strings collection from applicatiion settings</param>
        /// <returns>Extended services collection</returns>
        public static IServiceCollection AddLocalizationDbContext(this IServiceCollection services, IDictionary<string, string> connectionStrings)
        {
            return services.AddDbContext<LocalizationDbContext>(options =>
                options.UseSqlServer(connectionStrings["ExampleAppDatabase"], serverOptions =>
                    serverOptions.MigrationsHistoryTable(LocalizationDbContext.MigrationTableName, LocalizationDbContext.DefaultSchema)));
        }

        /// <summary>
        /// Adds logger entities to database
        /// </summary>
        /// <param name="services">Services collection to add application services to.</param>
        /// <param name="connectionStrings">Connection strings collection from applicatiion settings</param>
        /// <param name="defaultSchema">Default schema for logger tables</param>
        /// <returns>Extended services collection</returns>
        public static IServiceCollection AddLoggerDbContext(this IServiceCollection services, IDictionary<string, string> connectionStrings, string defaultSchema)
        {
            return services.AddDbContext<LoggerDbContext>(options =>
                options.UseSqlServer(connectionStrings["ExampleAppDatabase"], serverOptions =>
                    serverOptions.MigrationsHistoryTable(LoggerDbContext.MigrationTableName, defaultSchema ?? LoggerDbContext.DefaultSchema)));
        }

        /// <summary>
        /// Adds services for localization from .json files instead of .resx
        /// </summary>
        /// <param name="services">Services collection to add application services to.</param>
        /// <param name="config"><see cref="LocalizationConfig"/></param>
        /// <returns>Extended services collection</returns>
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services, LocalizationConfig config)
        {
            services.AddCommonLocalizationServices(config);
            services.AddSingleton<IApplicationLocalizationService, JsonLocalizationService>();

            return services;
        }

        /// <summary>
        /// Adds services for localization from database instead of .resx or .json files
        /// </summary>
        /// <param name="services">Services collection to add application services to.</param>
        /// <param name="config"><see cref="LocalizationConfig"/></param>
        /// <returns>Extended services collection</returns>
        public static IServiceCollection AddDatabaseLocalization(this IServiceCollection services, LocalizationConfig config)
        {
            services.AddCommonLocalizationServices(config);
            services.AddSingleton<IApplicationLocalizationService, DatabaseLocalizationService>();

            return services;
        }

        /// <summary>
        /// Adds identity
        /// </summary>
        /// <param name="services">Services collection to add application services to.</param>
        /// <returns>Identity builder</returns>
        public static IdentityBuilder AddApplicationIdentity(this IServiceCollection services)
        {
            services.AddTransient<IApplicationUserService, ApplicationUserService>();

            return services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddUserManager<ApplicationUserManager>()
                .AddSignInManager<ApplicationSignInManager>()
                .AddEntityFrameworkStores<SecurityDbContext>()
                .AddErrorDescriber<ApplicationIdentityErrorDescriber>()
                .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();
        }

        /// <summary>
        /// Adds common localization services
        /// </summary>
        /// <param name="services">Services collection to add application services to.</param>
        /// <param name="config"><see cref="LocalizationConfig"/></param>
        /// <returns>Services collection extended with common localization</returns>
        private static IServiceCollection AddCommonLocalizationServices(this IServiceCollection services, LocalizationConfig config)
        {
            // List of supported cultures to add to localization options
            var supportedCultures = config.SupportedCultures
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Select(c => new CultureInfo(c))
                    .ToList();

            // Localization options for localization config
            var options = new RequestLocalizationOptions
            {
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                DefaultRequestCulture = new RequestCulture(config.DefaultCulture)
            };

            // Adding culture provider, using url prefix to localize
            options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider { Options = options, RouteDataStringKey = config.SegmentName });

            services.ConfigureApplicationCookie(o =>
            {
                // Overriding login path and return url parameter
                o.LoginPath = "/login";
                o.ReturnUrlParameter = "to";

                // Localizing redirect to login page
                o.Events.OnRedirectToLogin = context =>
                {
                    var requestedCulture = context.HttpContext.GetRouteValue(config.SegmentName)?.ToString();
                    var returnUrl = WebUtility.UrlEncode($"{context.Request.Path}{context.Request.QueryString}");
                    var culture = supportedCultures.Any(c => string.Equals(requestedCulture, c.Name, System.StringComparison.OrdinalIgnoreCase))
                        ? requestedCulture
                        : CultureInfo.CurrentUICulture.Name;

                    // Redirect to localized login page with correct return url
                    return Task.Run(() => context.Response.Redirect($"/{culture}{o.LoginPath}?{o.ReturnUrlParameter}={returnUrl}"));
                };
            });

            // Adding singleton RequestLocalizationOptions
            services.AddSingleton(options);

            // Adding singleton custom html localizer factory
            services.AddSingleton<IHtmlLocalizerFactory, ApplicationHtmlLocalizerFactory>();

            // Adding singleton custom string localizer factory
            services.AddSingleton<IStringLocalizerFactory, ApplicationStringLocalizerFactory>();

            // Adding per request html localizer
            services.AddTransient(typeof(IHtmlLocalizer<>), typeof(HtmlLocalizer<>));

            // Adding per request string localizer
            services.AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

            return services;
        }
    }
}
