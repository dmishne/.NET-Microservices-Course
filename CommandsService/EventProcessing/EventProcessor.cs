namespace CommandsService.EventProcessing
{
    using System;
    using System.Text.Json;
    using AutoMapper;
    using CommandsService.Data;
    using CommandsService.Dtos;
    using CommandsService.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<EventProcessor> _logger;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper, ILogger<EventProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
            _logger = logger;
        }
        public void ProcessEvent(string message)
        {
            _logger.LogInformation("ProcessEvent");
            var eventType = GetEventType(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    ProcessPlatformPublishedEvent(message);
                    break;
                default:
                    _logger.LogError($"Event is not supported");
                    break;
            }
            _logger.LogInformation("Complete ProcessEvent");
        }

        private void ProcessPlatformPublishedEvent(string message)
        {
            _logger.LogInformation("ProcessPlatformPublishedEvent");

            try
            {
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(message);
                var platform = _mapper.Map<Platform>(platformPublishedDto);

                using var scope = _scopeFactory.CreateScope();
                var commandRepository = scope.ServiceProvider.GetService<ICommandRepository>();
                if (commandRepository.ExternalPlatformExists(platform.ExternalId))
                {
                    _logger.LogWarning($"Platform {platform.ExternalId} already exists");
                    return;
                }

                commandRepository.CreatePlatform(platform);
                commandRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not add platform to DB");
            }
        }

        private EventType GetEventType(string message)
        {
            var genericEvent = JsonSerializer.Deserialize<GenericEventDto>(message);
            _logger.LogInformation($"Received event {genericEvent.Event}");

            switch (genericEvent.Event)
            {
                case "Platform_Published":
                    return EventType.PlatformPublished;
                default:
                    return EventType.Undetermined;
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}