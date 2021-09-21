namespace PlatformService.SyncDataServices.Http
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using PlatformService.Dtos;

    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpCommandDataClient> _logger;
        private readonly CommandServiceConfig _commandConfig;

        public HttpCommandDataClient(HttpClient httpClient, IOptions<CommandServiceConfig> commandConfigOption, ILogger<HttpCommandDataClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _commandConfig = commandConfigOption.Value;
        }
        public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platformReadDto),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{_commandConfig.Endpoint}/api/commands/Platforms/", httpContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("--> Sync POST to CommandService was OK!");
            }
            else
            {
                _logger.LogError($"--> Sync POST to CommandService failed! {response.StatusCode}");
            }
        }
    }
}