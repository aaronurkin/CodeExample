using AaronUrkinCodeExample.DataAccessLayer.Logger.Entities;
using AutoMapper;

namespace AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Logger.Mappers
{
    public class LogEntryMapper : Profile
    {
        public LogEntryMapper()
        {
            this.CreateMap<LogEntry, LogEntryDto>().ReverseMap();
        }
    }
}
