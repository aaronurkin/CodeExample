using AaronUrkinCodeExample.BusinessLogicLayer.Middleware;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AaronUrkinCodeExample.Web.Controllers
{
    //[MiddlewareFilter(typeof(LocalizationPipeline))]
    [Authorize]
    public abstract class BaseController : Controller
    {
        public BaseController(
            IMapper mapper,
            ILogger logger)
        {
            this.Logger = logger;
            this.AutoMapper = mapper;
        }

        public ILogger Logger { get; set; }

        public IMapper AutoMapper { get; set; }
    }
}