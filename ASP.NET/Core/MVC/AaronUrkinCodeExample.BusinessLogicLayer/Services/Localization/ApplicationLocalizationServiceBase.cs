using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Localization
{
    /// <summary>
    /// Encapsulates logic retrieving translations from source and returning them as <see cref="LocalizedString"/> or <see cref="LocalizedHtmlString"/> 
    /// </summary>
    public abstract class ApplicationLocalizationServiceBase : IApplicationLocalizationService
    {
        private readonly List<TranslationDto> translations;

        protected readonly ILogger logger;

        public ApplicationLocalizationServiceBase(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.translations = new List<TranslationDto>(0);
        }

        public virtual IEnumerable<TranslationDto> Translations
        {
            get
            {
                if (!this.translations.Any())
                {
                    this.translations.AddRange(this.RetrieveTranslations());
                }

                return this.translations;
            }
        }

        public virtual TranslationDto FindTranslation(string key, string scope, CultureInfo culture)
        {
            return this.Translations.FirstOrDefault(t => t.Key == key && t.Scope == scope && (t.CultureCode.StartsWith(culture.Name) || t.CultureCode.StartsWith(culture.TwoLetterISOLanguageName)));
        }

        public virtual IEnumerable<TranslationDto> RetrieveScopeTranslations(string scope)
        {
            return this.Translations.Where(t => t.Scope.EndsWith(scope)) ?? this.Translations;
        }

        public virtual IEnumerable<TranslationDto> RetrieveCultureTranslations(string cultureCode)
        {
            return this.Translations.Where(t => t.CultureCode.StartsWith(cultureCode)) ?? this.Translations;
        }

        public LocalizedString TranslateString(string key, string scope, CultureInfo culture, object[] arguments)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }

            var translation = this.FindTranslation(key, scope, culture);
            var value = arguments == null
                ? translation?.Value ?? key
                : string.Format(translation?.Value ?? key, arguments);

            return new LocalizedString(key, value, translation == null);
        }

        public LocalizedHtmlString TranslateHtml(string key, string scope, CultureInfo culture, object[] arguments)
        {
            var translation = this.FindTranslation(key, scope, culture ?? CultureInfo.CurrentUICulture);

            if (arguments == null)
            {
                return new LocalizedHtmlString(key, translation?.Value ?? key, translation == null);
            }

            return new LocalizedHtmlString(key, translation?.Value ?? key, translation == null, arguments);
        }

        public abstract IEnumerable<TranslationDto> RetrieveTranslations();
    }
}
