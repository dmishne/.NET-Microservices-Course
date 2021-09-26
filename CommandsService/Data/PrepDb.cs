namespace CommandsService.Data
{
    using CommandsService.SyncDataServices.Grpc;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            var _logger = applicationBuilder.ApplicationServices.GetService<ILogger<Startup>>();
            _logger.LogInformation("PrepPopulation started");

            var platformDataClient = applicationBuilder.ApplicationServices.GetService<IPlatformDataClient>();

            var platforms = platformDataClient.ReturnAllPlatforms();

            using var scope = applicationBuilder.ApplicationServices.CreateScope();
            var commandRepository = scope.ServiceProvider.GetService<ICommandRepository>();

            foreach (var platform in platforms)
            {
                if (!commandRepository.ExternalPlatformExists(platform.ExternalId))
                {
                    commandRepository.CreatePlatform(platform);
                    _logger.LogInformation($"PrepPopulation: add platform {platform.Name} with external id {platform.ExternalId}");
                }
            }
            commandRepository.SaveChanges();

            _logger.LogInformation("PrepPopulation completed");
        }
    }
}