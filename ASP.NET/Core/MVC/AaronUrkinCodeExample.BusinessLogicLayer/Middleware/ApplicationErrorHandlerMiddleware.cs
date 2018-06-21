using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Middleware
{
    /// <summary>
    /// Handles critical errors and redirects to localized error page
    /// </summary>
    public class ApplicationErrorHandlerMiddleware
    {
        private readonly string errorUrl;
        private readonly RequestDelegate next;
        private readonly ILogger<ApplicationErrorHandlerMiddleware> logger;

        public ApplicationErrorHandlerMiddleware(
            RequestDelegate next,
            IOptions<ApplicationErrorHandlerOptions> options,
            ILogger<ApplicationErrorHandlerMiddleware> logger)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.errorUrl = options.Value.ErrorUrl ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next.Invoke(context);
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex, ex.Message);

                context.Response.Redirect($"/{CultureInfo.CurrentUICulture.Name}{(this.errorUrl.StartsWith('/') ? this.errorUrl : $"/{this.errorUrl}")}");
                return;
            }
        }
    }
}
