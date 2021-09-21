namespace PlatformService.SyncDataServices.Http
{
    using System.Threading.Tasks;
    using PlatformService.Dtos;

    public interface ICommandDataClient{
        Task SendPlatformToCommand(PlatformReadDto platformReadDto);
    }
}