using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlatformService.Config;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureOptions(services);

            var cfg = Configuration.GetSection("Storage").Get<StorageConfig>();

            if(cfg.UseInMemory){
                Console.WriteLine("Use in memory database");
                services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMem"));
            } else{
                Console.WriteLine("Use Postgresql database");
                services.AddDbContext<AppDbContext>(options => options.UseNpgsql(cfg.ConnectionString));
            }

            services.AddScoped<IPlatformRepository, PlatformRepository>();

            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });
        }

        public void ConfigureOptions(IServiceCollection services)
        {
            services.Configure<CommandServiceConfig>(Configuration.GetSection("CommandService"));
            services.Configure<StorageConfig>(Configuration.GetSection("Storage"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            PrepDb.PrepPopulation(app, !Configuration.GetSection("Storage").Get<StorageConfig>().UseInMemory);
        }
    }
}
