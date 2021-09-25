namespace PlatformService.AsyncDataServices
{
    using PlatformService.Dtos;

    public interface IMessageBusClient
    {
        void PublishNewPlatform(PlatformPublishedDto platformPublishDto);
    }
}