using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Security;
using AaronUrkinCodeExample.Web.Models.AccountViewModels;
using AutoMapper;

namespace AaronUrkinCodeExample.Web.Models.Mappers
{
    public class ApplicationUserMapper : Profile
    {
        public ApplicationUserMapper()
        {
            this.CreateMap<RegisterViewModel, ApplicationUserDto>()
                .ForMember(user => user.Id, opts => opts.Ignore())
                .ForMember(user => user.UserName, opts => opts.Ignore())
                .ForMember(user => user.EmailConfirmed, opts => opts.Ignore())
                .ForMember(user => user.NormalizedEmail, opts => opts.Ignore())
                .ForMember(user => user.NormalizedUserName, opts => opts.Ignore())
                .ForMember(user => user.PhoneNumberConfirmed, opts => opts.Ignore())
                .ForMember(user => user.EmailConfirmationToken, opts => opts.Ignore())
                .AfterMap(this.ResolveAndNormalizeUserName);

            this.CreateMap<ApplicationUserDto, RegisterViewModel>()
                .ForMember(model => model.Password, opts => opts.Ignore())
                .ForMember(model => model.ConfirmPassword, opts => opts.Ignore());
        }

        private void ResolveAndNormalizeUserName(RegisterViewModel model, ApplicationUserDto user)
        {
            string normalizedEmail = model.Email.ToUpper();

            user.UserName = model.Email;
            user.NormalizedEmail = normalizedEmail;
            user.NormalizedUserName = normalizedEmail;
        }
    }
}
