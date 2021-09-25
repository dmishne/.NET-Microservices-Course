namespace CommandsService.Data
{
    using System.Collections.Generic;
    using CommandsService.Models;

    public interface ICommandRepository
    {
        bool SaveChanges();

        // Platforms
        IEnumerable<Platform> GetAllPlatforms();

        void CreatePlatform(Platform platform);

        bool PlatformExists(int platformId);

        bool ExternalPlatformExists(int platformId);

        // Commands
        IEnumerable<Command> GetCommandsForPlatform(int platformId);

        Command GetCommand(int platformId, int commandId);

        void CreateCommand(int platformId, Command command);
    }
}