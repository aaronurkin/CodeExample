using AutoMapper;
using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Localization;
using AaronUrkinCodeExample.DataAccessLayer.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Localization
{
    public class DatabaseLocalizationService : ApplicationLocalizationServiceBase
    {
        private readonly IServiceProvider services;

        /// <summary>
        /// Initializes <see cref="IServiceProvider"/> private field
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> to retrieve services (<seealso cref="IMapper"/> and <seealso cref="LocalizationDbContext"/>) instead of inject them</param>
        /// <param name="logger">Instance of <see cref="ILogger"/> to be passed to parent constructor</param>
        public DatabaseLocalizationService(IServiceProvider services, ILogger<DatabaseLocalizationService> logger)
            : base(logger)
        {
            this.services = services;
        }

        /// <summary>
        /// Retrieves translations from database
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<TranslationDto> RetrieveTranslations()
        {
            // It isn't possible to inject transient db context to singleton class.
            // So creating a scope to get context from container and retrieve translations from database
            using (var scope = this.services.CreateScope())
            {
                var mapper = scope.ServiceProvider.GetService<IMapper>();
                var context = scope.ServiceProvider.GetService<LocalizationDbContext>();

                return context.Translations.AsNoTracking().Select(mapper.Map<TranslationDto>).ToArray();
            }
        }
    }
}
