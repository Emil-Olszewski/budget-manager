using Core.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    public static class ServicesRegistration
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Context>(x =>
            {
                x.EnableDetailedErrors();
                x.EnableSensitiveDataLogging();
                x.UseSqlServer(configuration.GetConnectionString("Default"));
            });

            services.AddScoped<IContext, Context>();
        }
    }
}