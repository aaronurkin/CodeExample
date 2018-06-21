using AaronUrkinCodeExample.BusinessLogicLayer.Extensions;
using Microsoft.AspNetCore.Builder;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Middleware
{
    /// <summary>
    /// The pipeline localizing application using specified within <see cref="RequestLocalizationOptions"/> options properties and providers
    /// </summary>
    public class LocalizationPipeline
    {
        public void Configure(IApplicationBuilder app, RequestLocalizationOptions options)
        {
            app.UseRequestLocalization(options);
            app.UseRedirectUnsupportedCultures();
        }
    }
}
