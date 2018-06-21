using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Localization
{
    /// <summary>
    /// Implements <see cref="IHtmlLocalizer"/> to get translations using an instance <see cref="IApplicationLocalizationService"/>
    /// </summary>
    public class ApplicationHtmlLocalizer : IHtmlLocalizer
    {
        private CultureInfo culture;
        private readonly string scope;
        private readonly IApplicationLocalizationService service;

        /// <summary>
        /// Initializes <see cref="ApplicationHtmlLocalizer"/> for specified scope and using specified <see cref="IApplicationLocalizationService"/>
        /// </summary>
        /// <param name="scope">The scope to localize</param>
        /// <param name="service">The service retrieving translations from source</param>
        public ApplicationHtmlLocalizer(string scope, IApplicationLocalizationService service)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Initializes <see cref="ApplicationHtmlLocalizer"/> for specified culture and scope and using specified <see cref="IApplicationLocalizationService"/>
        /// </summary>
        /// <param name="scope">The scope to localize</param>
        /// <param name="culture">Culture to retrieve translations for</param>
        /// <param name="service">The service retrieving translations from source</param>
        public ApplicationHtmlLocalizer(string scope, CultureInfo culture, IApplicationLocalizationService service)
            : this(scope, service)
        {
            this.culture = culture;
        }

        public LocalizedHtmlString this[string name] => this.service.TranslateHtml(name, this.scope, this.culture, null);

        public LocalizedHtmlString this[string name, params object[] arguments] => this.service.TranslateHtml(name, this.scope, this.culture, arguments);

        public LocalizedString GetString(string name)
        {
            return this.GetString(name, this.culture, null);
        }

        public LocalizedString GetString(string name, params object[] arguments)
        {
            return this.service.TranslateString(name, this.scope, this.culture, arguments);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return this.service.Translations?.Select(t => new LocalizedString(t.Key, t.Value, true));
        }

        public IHtmlLocalizer WithCulture(CultureInfo culture)
        {
            return new ApplicationHtmlLocalizer(this.scope, culture, this.service);
        }
    }
}
