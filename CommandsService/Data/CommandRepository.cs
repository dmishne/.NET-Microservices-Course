namespace CommandsService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommandsService.Models;

    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _context;

        public CommandRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool ExternalPlatformExists(int platformId)
        {
            return _context.Platforms.Any(p => p.ExternalId == platformId);
        }

        void ICommandRepository.CreateCommand(int platformId, Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            command.PlatformId = platformId;

            _context.Commands.Add(command);
        }

        void ICommandRepository.CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            _context.Platforms.Add(platform);
        }

        IEnumerable<Platform> ICommandRepository.GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        Command ICommandRepository.GetCommand(int platformId, int commandId)
        {
            return _context.Commands.FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
        }

        IEnumerable<Command> ICommandRepository.GetCommandsForPlatform(int platformId)
        {
            return _context.Commands.Where(c => c.PlatformId == platformId).OrderBy(c => c.Platform.Name);
        }

        bool ICommandRepository.PlatformExists(int platformId)
        {
            return _context.Platforms.Any(p => p.Id == platformId);
        }

        bool ICommandRepository.SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}