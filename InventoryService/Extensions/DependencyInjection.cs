using Common.Interfaces;
using InventoryService.Application.Services;
using InventoryService.Application.Services.Interfaces;
using InventoryService.Infrastructure.BackgroundJobs;
using InventoryService.Infrastructure.Data.Repositories;
using InventoryService.Infrastructure.Data.Repositories.Interfaces;

namespace InventoryService.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(Repository<>));
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IRoomAvailabilityRepository, RoomAvailabilityRepository>();

        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IRoomAvailabilityService, RoomAvailabilityService>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Background worker periodically cleans up expired records and generates future availability entries.
        // services.AddHostedService<RoomAvailabilityMaintenanceWorker>();

        return services;
    }
}