using AaronUrkinCodeExample.BusinessLogicLayer.Localization;
using AaronUrkinCodeExample.BusinessLogicLayer.Middleware;
using AaronUrkinCodeExample.BusinessLogicLayer.Services.Localization;
using AaronUrkinCodeExample.DataAccessLayer.Localization;
using AaronUrkinCodeExample.DataAccessLayer.Localization.Entities;
using AaronUrkinCodeExample.DataAccessLayer.Logger;
using AaronUrkinCodeExample.DataAccessLayer.Security;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds middleware logging all critical application errors
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/></param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseApplicationErrorHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApplicationErrorHandlerMiddleware>();
        }

        /// <summary>
        /// Adds middleware redirecting requests with unsupported culture segment to the <see cref="IApplicationBuilder"/> request execution pipeline
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/></param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseRedirectUnsupportedCultures(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RedirectUnsupportedCulturesMiddleware>();
        }

        /// <summary>
        /// Adds MVC with localized routes to the <see cref="IApplicationBuilder"/> request execution pipeline
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/></param>
        /// <param name="options">The <see cref="RequestLocalizationOptions"/> to configure the middleware with</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseMvcWithLocalizedRoutes(this IApplicationBuilder app, RequestLocalizationOptions options)
        {
            var provider = options.RequestCultureProviders.FirstOrDefault() as RouteDataRequestCultureProvider;
            var cultureSegment = $@"{{{provider.RouteDataStringKey}={options.DefaultRequestCulture.Culture.Name}}}";

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "home",
                    template: $"{cultureSegment}/home",
                    defaults: new { controller = "Home", action = "Index" }
                );

                routes.MapRoute(
                    name: "error",
                    template: $"{cultureSegment}/error",
                    defaults: new { controller = "Home", action = "Error" }
                );

                routes.MapRoute(
                    name: "experience",
                    template: $"{cultureSegment}/experience",
                    defaults: new { controller = "Home", action = "Experience" }
                );

                routes.MapRoute(
                    name: "register",
                    template: $"{cultureSegment}/register",
                    defaults: new { controller = "Account", action = "Register" }
                );

                routes.MapRoute(
                    name: "confirmEmail",
                    template: $"{cultureSegment}/confirm",
                    defaults: new { controller = "Account", action = "ConfirmEmail" }
                );

                routes.MapRoute(
                    name: "logIn",
                    template: $"{cultureSegment}/login",
                    defaults: new { controller = "Account", action = "Login" }
                    );

                routes.MapRoute(
                    name: "forgotPassword",
                    template: $"{cultureSegment}/forgot-password",
                    defaults: new { controller = "Account", action = "ForgotPassword" }
                );

                routes.MapRoute(
                    name: "forgotPasswordConfirmation",
                    template: $"{cultureSegment}/confirm-reset-password",
                    defaults: new { controller = "Account", action = "ForgotPasswordConfirmation" }
                );

                routes.MapRoute(
                    name: "resetPassword",
                    template: $"{cultureSegment}/reset-password",
                    defaults: new { controller = "Account", action = "ResetPassword" }
                );

                routes.MapRoute(
                    name: "resetPasswordConfirmation",
                    template: $"{cultureSegment}/password-has-been-reset",
                    defaults: new { controller = "Account", action = "ResetPasswordConfirmation" }
                );

                routes.MapRoute(
                    name: "logsList",
                    template: $"{cultureSegment}/logs",
                    defaults: new { controller = "Log", action = "List" }
                );

                routes.MapRoute(
                    name: "logEntries",
                    template: $"{cultureSegment}/log/entries",
                    defaults: new { controller = "Log", action = "LogEntries" }
                );

                routes.MapRoute(
                    name: "logOut",
                    template: $"{cultureSegment}/logout",
                    defaults: new { controller = "Account", action = "Logout" }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            return app;
        }

        /// <summary>
        /// Retrieves security context from services and invokes migration
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/></param>
        public static void UseSecurityMigrations(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<SecurityDbContext>();

                context.Database.Migrate();
            }
        }

        /// <summary>
        /// Retrieves localization context from services and invokes migration. If <paramref name="seed"/> is true, merges translations within database with .json files within 'Localization' folder
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/></param>
        /// <param name="seed">Indicates whether to seed the database or not</param>
        public static void UseLocalizationMigrations(this IApplicationBuilder app, bool seed = true)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<LocalizationDbContext>();

                context.Database.Migrate();

                if (seed)
                {
                    var mapper = scope.ServiceProvider.GetService<IMapper>();

                    var environment = scope.ServiceProvider.GetService<IHostingEnvironment>();
                    var config = scope.ServiceProvider.GetService<IOptions<LocalizationConfig>>();
                    var logger = scope.ServiceProvider.GetService<ILogger<JsonLocalizationService>>();

                    var service = new JsonLocalizationService(environment, config, logger);

                    foreach (var translation in context.Translations.AsEnumerable())
                    {
                        if (!service.Translations.Any(t => t.Key == translation.Key && t.Scope == translation.Scope && t.CultureCode == translation.CultureCode))
                        {
                            context.Translations.Remove(translation);
                        }
                    }

                    if (!context.Translations.Any())
                    {
                        context.Translations.AddRange(service.Translations.Select(mapper.Map<Translation>));
                    }
                    else
                    {
                        foreach (var dto in service.Translations)
                        {
                            var translation = context.Translations.FirstOrDefault(t => t.Key == dto.Key && t.Scope == dto.Scope && t.CultureCode == dto.CultureCode);

                            if (translation == null)
                            {
                                context.Translations.Add(mapper.Map<Translation>(dto));
                            }
                            else if (translation.Value != dto.Value)
                            {
                                translation.Value = dto.Value;
                            }
                        }
                    }

                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Retrieves logger context from services and invokes migration
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/></param>
        public static void UseLoggerMigrations(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<LoggerDbContext>();

                context.Database.Migrate();
            }
        }
    }
}
