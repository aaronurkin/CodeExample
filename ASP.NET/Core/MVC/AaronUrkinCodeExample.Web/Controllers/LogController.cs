using AaronUrkinCodeExample.BusinessLogicLayer.Services;
using AaronUrkinCodeExample.BusinessLogicLayer.Services.Logger;
using AaronUrkinCodeExample.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace AaronUrkinCodeExample.Web.Controllers
{
    public class LogController : BaseController
    {
        private readonly ILoggerService service;

        public LogController(ILoggerService service, IMapper mapper, ILogger<LogController> logger)
            : base(mapper, logger)
        {
            this.service = service;
        }

        public IActionResult List()
        {
            return View();
        }

        /// <summary>
        /// Retrieves and paginates all logs from db ordering by date descending
        /// </summary>
        /// <param name="p">Current page number</param>
        /// <param name="r">Number of entries on each page</param>
        /// <returns>List of logs</returns>
        public async Task<PartialViewResult> LogEntries(int p, int r)
        {
            var logs = await this.service.RetrieveAsync(p, r);
            var views = logs.Select(this.AutoMapper.Map<LogEntryViewModel>);
            var model = new PagedList<LogEntryViewModel>(views, logs.TotalCount, logs.Page, logs.PageEntries);

            return PartialView("_LogEntries", model);
        }
    }
}