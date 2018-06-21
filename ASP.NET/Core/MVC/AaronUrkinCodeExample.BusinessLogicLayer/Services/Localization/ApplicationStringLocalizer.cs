using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Localization
{
    /// <summary>
    /// Implements <see cref="IStringLocalizer"/> to get translations using an instance <see cref="IApplicationLocalizationService"/>
    /// </summary>
    public class ApplicationStringLocalizer : IStringLocalizer
    {
        private CultureInfo culture;
        private readonly string scope;
        private readonly IApplicationLocalizationService service;

        /// <summary>
        /// Initializes <see cref="ApplicationStringLocalizer"/> for specified scope and using specified <see cref="IApplicationLocalizationService"/>
        /// </summary>
        /// <param name="scope">The scope to localize</param>
        /// <param name="service">The service retrieving translations from source</param>
        public ApplicationStringLocalizer(string scope, IApplicationLocalizationService service)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Initializes <see cref="ApplicationStringLocalizer"/> for specified culture and scope and using specified <see cref="IApplicationLocalizationService"/>
        /// </summary>
        /// <param name="scope">The scope to localize</param>
        /// <param name="culture">Culture to retrieve translations for</param>
        /// <param name="service">The service retrieving translations from source</param>
        public ApplicationStringLocalizer(string scope, CultureInfo culture, IApplicationLocalizationService service)
            : this(scope, service)
        {
            this.culture = culture;
        }

        public virtual LocalizedString this[string name] => this.service.TranslateString(name, this.scope, this.culture, null);

        public virtual LocalizedString this[string name, params object[] arguments] => this.service.TranslateString(name, this.scope, this.culture, arguments);

        public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return this.service.Translations?.Select(t => new LocalizedString(t.Key, t.Value, true));
        }

        public virtual IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new ApplicationStringLocalizer(this.scope, culture, this.service);
        }
    }
}
