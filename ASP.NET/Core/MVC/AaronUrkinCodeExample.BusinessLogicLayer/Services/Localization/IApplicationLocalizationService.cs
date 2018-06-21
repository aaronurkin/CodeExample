using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Localization
{
    /// <summary>
    /// Defines methods to retrieve translations
    /// </summary>
    public interface IApplicationLocalizationService
    {
        /// <summary>
        /// Returns all translations retrieved from a source
        /// </summary>
        IEnumerable<TranslationDto> Translations { get; }

        /// <summary>
        /// Retrieves translations for the specified scope
        /// </summary>
        /// <param name="scope">Application scope must be translated</param>
        /// <returns>Filtered by scope translations</returns>
        IEnumerable<TranslationDto> RetrieveScopeTranslations(string scope);

        /// <summary>
        /// Retrieves translations for the specified culture
        /// </summary>
        /// <param name="cultureCode">Culture name</param>
        /// <returns>Filtered by culture translations</returns>
        IEnumerable<TranslationDto> RetrieveCultureTranslations(string cultureCode);

        /// <summary>
        /// Retrieves single translation for specified phrase and culture that declared within specified scope
        /// </summary>
        /// <param name="key">Phrase must be translated</param>
        /// <param name="scope">Application scope</param>
        /// <param name="culture"><see cref="CultureInfo"/></param>
        /// <returns></returns>
        TranslationDto FindTranslation(string key, string scope, CultureInfo culture);

        /// <summary>
        /// Translates phrase specified by <paramref name="source"/>
        /// </summary>
        /// <param name="source">Phrase must be translated</param>
        /// <param name="scope">Application scope</param>
        /// <param name="culture"><see cref="CultureInfo"/></param>
        /// <param name="arguments">The values to format the value with.</param>
        /// <returns>Translation as <see cref="LocalizedString"/></returns>
        LocalizedString TranslateString(string source, string scope, CultureInfo culture, object[] arguments);

        /// <summary>
        /// Translates html specified by <paramref name="source"/>
        /// </summary>
        /// <param name="source">Phrase must be translated</param>
        /// <param name="scope">Application scope</param>
        /// <param name="culture"><see cref="CultureInfo"/></param>
        /// <param name="arguments">The values to format the value with.</param>
        /// <returns>Translation as <see cref="LocalizedHtmlString"/></returns>
        LocalizedHtmlString TranslateHtml(string source, string scope, CultureInfo culture, object[] arguments);
    }
}
