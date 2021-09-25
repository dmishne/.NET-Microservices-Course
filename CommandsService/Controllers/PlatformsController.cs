namespace CommandsService.Controllers
{
    using System;
    using System.Collections.Generic;
    using AutoMapper;
    using CommandsService.Data;
    using CommandsService.Dtos;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("api/commands/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PlatformsController> _logger;

        public PlatformsController(ICommandRepository commandRepository, IMapper mapper, ILogger<PlatformsController> logger)
        {
            _commandRepository = commandRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult GetPlatforms()
        {
            _logger.LogInformation("--> GetPlatforms");
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(_commandRepository.GetAllPlatforms()));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            _logger.LogInformation("--> Inbound POST # Command Service");
            return Ok();
        }
    }
}