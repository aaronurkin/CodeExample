using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Logger;
using AutoMapper;

namespace AaronUrkinCodeExample.Web.Models.Mappers
{
    public class LogEntryMapper : Profile
    {
        public LogEntryMapper()
        {
            this.CreateMap<LogEntryDto, LogEntryViewModel>().ReverseMap();
        }
    }
}
