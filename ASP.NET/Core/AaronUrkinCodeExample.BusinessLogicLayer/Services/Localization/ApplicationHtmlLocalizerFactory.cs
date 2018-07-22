using Microsoft.AspNetCore.Mvc.Localization;
using System;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Localization
{
    /// <summary>
    /// Creates an instance of <see cref="ApplicationHtmlLocalizer"/>
    /// </summary>
    class ApplicationHtmlLocalizerFactory : IHtmlLocalizerFactory
    {
        private readonly IApplicationLocalizationService service;

        public ApplicationHtmlLocalizerFactory(IApplicationLocalizationService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public IHtmlLocalizer Create(Type resourceSource)
        {
            return this.Create(resourceSource.FullName, resourceSource.Assembly.GetName().Name);
        }

        public IHtmlLocalizer Create(string baseName, string location)
        {
            return this.Create(baseName.Replace($"{location}.", string.Empty));
        }

        private IHtmlLocalizer Create(string scope)
        {
            return new ApplicationHtmlLocalizer(scope, this.service);
        }
    }
}
