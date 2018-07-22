using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Middleware
{
    /// <summary>
    /// Redirecting requests with unsupported culture segment to actual culture urls
    /// </summary>
    public class RedirectUnsupportedCulturesMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string cultureSegment;

        public RedirectUnsupportedCulturesMiddleware(RequestDelegate next, RequestLocalizationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.next = next ?? throw new ArgumentNullException(nameof(next));

            var provider = options.RequestCultureProviders.OfType<RouteDataRequestCultureProvider>().First();
            this.cultureSegment = provider.RouteDataStringKey;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestedCulture = context.GetRouteValue(this.cultureSegment)?.ToString();
            var cultureFeature = context.Features.Get<IRequestCultureFeature>();
            var actualCulture = cultureFeature?.RequestCulture.Culture.Name;

            if (string.IsNullOrEmpty(requestedCulture) || !string.Equals(requestedCulture, actualCulture, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Redirect(this.OverrideUrl(context, actualCulture));
                return;
            }

            await this.next.Invoke(context);
        }

        /// <summary>
        /// Extends url with actual culture
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/></param>
        /// <param name="culture">Actual culture name</param>
        /// <returns></returns>
        private string OverrideUrl(HttpContext context, string culture)
        {
            var routeData = context.GetRouteData();
            var router = routeData.Routers[0];
            var virtualPathContext = new VirtualPathContext(
                context,
                routeData.Values,
                new RouteValueDictionary { { this.cultureSegment, culture } });

            return router.GetVirtualPath(virtualPathContext).VirtualPath.ToLower();
        }
    }
}
