namespace PlatformService.SyncDataServices.Grpc
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using PlatformService.Data;
    using static PlatformService.GrpcPlatform;

    public class GrpcPlatformService : GrpcPlatformBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GrpcPlatformService> _logger;

        public GrpcPlatformService(IPlatformRepository platformRepository, IMapper mapper, ILogger<GrpcPlatformService> logger)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, global::Grpc.Core.ServerCallContext context)
        {
            _logger.LogInformation("GrpcPlatformService -> GetAllPlatforms has been called");

            var platforms = _platformRepository.GetAllPlatforms();
            var response = new PlatformResponse();
            response.Platforms.AddRange(_mapper.Map<IEnumerable<GrpcPlatformModel>>(platforms));
            return Task.FromResult(response);
        }
    }
}