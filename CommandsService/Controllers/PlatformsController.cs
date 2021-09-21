namespace CommandsService.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("api/commands/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ILogger<PlatformsController> _logger;

        public PlatformsController(ILogger<PlatformsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            _logger.LogInformation("--> Inbound POST # Command Service");
            return Ok();
        }
    }
}