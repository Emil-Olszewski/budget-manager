using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application;

public static class ServiceRegistration
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}