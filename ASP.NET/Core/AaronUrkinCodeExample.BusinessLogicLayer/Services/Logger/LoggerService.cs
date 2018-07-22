using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Logger;
using AaronUrkinCodeExample.BusinessLogicLayer.Extensions;
using AaronUrkinCodeExample.DataAccessLayer.Logger;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Logger
{
    public class LoggerService : ILoggerService
    {
        private readonly IMapper mapper;
        private readonly LoggerDbContext context;

        public LoggerService(LoggerDbContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public Task<PagedList<LogEntryDto>> RetrieveAsync(int page, int entries)
        {
            return this.context.LogEntries
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAtUtc)
                .ToPagedListAsync(this.mapper.Map<LogEntryDto>, page > 0 ? page : 1, entries > 0 ? entries : 5);
        }
    }
}
