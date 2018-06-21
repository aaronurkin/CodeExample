using Microsoft.Extensions.Localization;
using System;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Localization
{
    /// <summary>
    /// Creates an instance of <see cref="ApplicationStringLocalizer"/>
    /// </summary>
    public class ApplicationStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IApplicationLocalizationService service;

        public ApplicationStringLocalizerFactory(IApplicationLocalizationService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Creates an instance of <see cref="ApplicationStringLocalizer"/> using type to calculate scope
        /// </summary>
        /// <param name="resourceSource">Source type</param>
        /// <returns><see cref="ApplicationStringLocalizer"/></returns>
        public IStringLocalizer Create(Type resourceSource)
        {
            return this.Create(resourceSource.FullName, resourceSource.Assembly.GetName().Name);
        }

        /// <summary>
        /// Creates an instance of <see cref="ApplicationStringLocalizer"/> using full resource name and it's location to calculate scope
        /// </summary>
        /// <param name="baseName">Resource name</param>
        /// <param name="location">Resource location</param>
        /// <returns><see cref="ApplicationStringLocalizer"/></returns>
        public IStringLocalizer Create(string baseName, string location)
        {
            return this.Create(baseName.Replace($"{location}.", string.Empty));
        }

        /// <summary>
        /// Creates an instance of <see cref="ApplicationStringLocalizer"/> for the calculated scope
        /// </summary>
        private IStringLocalizer Create(string scope)
        {
            return new ApplicationStringLocalizer(scope, this.service);
        }
    }
}
