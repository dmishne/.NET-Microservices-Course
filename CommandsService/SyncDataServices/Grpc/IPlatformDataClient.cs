namespace CommandsService.SyncDataServices.Grpc
{
    using System.Collections.Generic;
    using CommandsService.Models;

    public interface IPlatformDataClient
    {
        IEnumerable<Platform> ReturnAllPlatforms();
    }
}