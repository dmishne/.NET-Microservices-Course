namespace CommandsService.SyncDataServices.Grpc
{
    using System;
    using System.Collections.Generic;
    using AutoMapper;
    using CommandsService.Config;
    using CommandsService.Models;
    using global::Grpc.Net.Client;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using PlatformService;

    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly GrpcPlatformConfig _grpcPlatformConfig;
        private readonly IMapper _mapper;
        private readonly ILogger<PlatformDataClient> _logger;

        public PlatformDataClient(IMapper mapper, IOptions<GrpcPlatformConfig> grpcPlatformOption, ILogger<PlatformDataClient> logger)
        {
            _grpcPlatformConfig = grpcPlatformOption.Value;
            _mapper = mapper;
            _logger = logger;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            _logger.LogInformation($"Calling GRPC Service. Endpoint: {_grpcPlatformConfig.Endpoint}");

            var channel = GrpcChannel.ForAddress(_grpcPlatformConfig.Endpoint);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platforms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not call GRPC Server");
                return null;
            }
        }
    }
}