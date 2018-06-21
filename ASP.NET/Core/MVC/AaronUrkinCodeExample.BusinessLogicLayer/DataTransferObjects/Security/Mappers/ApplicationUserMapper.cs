using AaronUrkinCodeExample.DataAccessLayer.Security.Entities;
using AutoMapper;

namespace AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Security.Mappers
{
    public class ApplicationUserMapper : Profile
    {
        public ApplicationUserMapper()
        {
            this.CreateMap<ApplicationUser, ApplicationUserDto>()
                .ForMember(dto => dto.EmailConfirmationToken, opts => opts.Ignore())
                .ReverseMap();
        }
    }
}
