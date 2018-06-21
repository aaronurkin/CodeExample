using AaronUrkinCodeExample.DataAccessLayer.Localization.Entities;
using AutoMapper;

namespace AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Localization.Mappers
{
    public class TranslationMapper : Profile
    {
        public TranslationMapper()
        {
            this.CreateMap<Translation, TranslationDto>().ReverseMap();
        }
    }
}
