namespace PlatformService.Data
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using PlatformService.Models;

    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool ShouldMigrate)
        {            
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), ShouldMigrate);
            }
        }

        private static void SeedData(AppDbContext context, bool ShouldMigrate)
        {
            if(ShouldMigrate)
            {
                context.Database.Migrate();
            }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data ...");

                context.Platforms.AddRange(
                    new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "SQL Server", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "K8s", Publisher = "CNCF", Cost = "Free" }
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}