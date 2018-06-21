using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Localization;
using AaronUrkinCodeExample.BusinessLogicLayer.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Localization
{
    /// <summary>
    /// Service retrieving translations from .json files
    /// </summary>
    public class JsonLocalizationService : ApplicationLocalizationServiceBase
    {
        private readonly string resourcesPath;
        private readonly string[] supportedCultureCodes;

        /// <summary>
        /// Initializes resources path
        /// </summary>
        /// <param name="env">Instance of <see cref="IHostingEnvironment"/> to retrieve root path from it</param>
        /// <param name="options">Instance of <see cref="IOptions<LocalizationOptions>"/> to retrieve resources path from application settings</param>
        /// <param name="logger">Instance of <see cref="ILogger"/> to be passed to parent constructor</param>
        public JsonLocalizationService(
            IHostingEnvironment env,
            IOptions<LocalizationConfig> config,
            ILogger<JsonLocalizationService> logger)
            : base(logger)
        {
            if (config?.Value == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this.supportedCultureCodes = config.Value.SupportedCultures;
            this.resourcesPath = Path.Combine(env.ContentRootPath, config.Value.ResourcesPath ?? "Localization");
        }

        /// <summary>
        /// Retrieves translations from .json files, located within resources path, specified within application settings
        /// </summary>
        /// <returns>All translations specified within .json files located within resources path</returns>
        public override IEnumerable<TranslationDto> RetrieveTranslations()
        {
            foreach (var path in Directory.GetFiles(this.resourcesPath, "*.json", SearchOption.AllDirectories))
            {
                var filePath = new StringBuilder(Path.ChangeExtension(path, string.Empty));
                var fileName = filePath.Replace(this.resourcesPath, string.Empty).Replace('\\', '.').Remove(0, 1).Remove(filePath.Length - 1, 1).ToString();
                var cultureCode = fileName.Substring(fileName.LastIndexOf('.') + 1);

                if (cultureCode == null)
                {
                    this.logger.LogError("Failed to retrieve culture name from file: {0}", path);
                    continue;
                }

                if (!this.supportedCultureCodes.Any(i => string.Equals(i, cultureCode, StringComparison.OrdinalIgnoreCase)))
                {
                    this.logger.LogWarning("{0} is invalid culture name", cultureCode);
                    continue;
                }

                Dictionary<string, string> content = null;

                try
                {
                    content = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Failed to load translations from file {0}", path);
                    continue;
                }

                if (content == null)
                {
                    this.logger.LogWarning("File is empty: {0}", path);
                }

                foreach (var key in content.Keys)
                {
                    yield return new TranslationDto
                    {
                        Key = key,
                        Value = content[key],
                        CultureCode = cultureCode,
                        Scope = fileName.Replace(cultureCode, string.Empty).TrimEnd('.')
                    };
                }
            }
        }
    }
}
