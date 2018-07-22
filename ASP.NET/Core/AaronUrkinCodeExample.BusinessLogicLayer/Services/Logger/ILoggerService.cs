using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Logger;
using System.Threading.Tasks;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Logger
{
    public interface ILoggerService
    {
        Task<PagedList<LogEntryDto>> RetrieveAsync(int page, int rows);
    }
}
