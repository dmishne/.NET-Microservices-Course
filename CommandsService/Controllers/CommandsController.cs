namespace CommandsService.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using CommandsService.Data;
    using CommandsService.Dtos;
    using CommandsService.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("api/commands/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CommandsController> _logger;

        public CommandsController(ICommandRepository commandRepository, IMapper mapper, ILogger<CommandsController> logger)
        {
            _commandRepository = commandRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommandsForPlatform(int platformId)
        {
            _logger.LogInformation($"--> GetAllCommandsForPlatform: platform: {platformId}");
            if (!_commandRepository.PlatformExists(platformId))
            {
                return NotFound($"Platform {platformId} not exists");
            }

            var commands = _commandRepository.GetCommandsForPlatform(platformId).ToList();

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            _logger.LogInformation($"--> GetCommandForPlatform: platform: {platformId}, command: {commandId}");
            if (!_commandRepository.PlatformExists(platformId))
            {
                return NotFound($"Platform {platformId} not exists");
            }

            var command = _commandRepository.GetCommand(platformId, commandId);
            if (command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            _logger.LogInformation($"--> CreateCommandForPlatform: platform: {platformId}");
            if (!_commandRepository.PlatformExists(platformId))
            {
                return NotFound($"Platform {platformId} not exists");
            }

            var command = _mapper.Map<Command>(commandCreateDto);
            _commandRepository.CreateCommand(platformId, command);
            _commandRepository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
        }

    }
}