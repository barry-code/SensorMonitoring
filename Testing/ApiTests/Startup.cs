using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SensorMonitoring.Api.Repository;
using SensorMonitoring.Shared.Interfaces;

namespace SensorMonitoring.ApiTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<SensorContext>(options =>
        {
            options.UseInMemoryDatabase("SensorMonitoringApiTesting");
        });

        services.AddScoped<ISensorRepository, SensorRepository>();
    }
}
