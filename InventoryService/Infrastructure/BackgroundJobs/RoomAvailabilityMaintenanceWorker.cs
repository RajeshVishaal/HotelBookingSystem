using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.BackgroundJobs;

public class RoomAvailabilityMaintenanceWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RoomAvailabilityMaintenanceWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var cutoff = today.AddDays(-1);
            var newDate = today.AddDays(730);

            await db.RoomAvailabilities
                .Where(r => r.Date < cutoff)
                .ExecuteDeleteAsync(stoppingToken);

            var newRecords = await db.RoomCategories
                .AsNoTracking()
                .Select(rc => new RoomAvailability
                {
                    HotelId = rc.HotelId,
                    RoomCategoryId = rc.Id,
                    Date = newDate,
                    TotalCount = rc.TotalCount,
                    TotalBooked = 0,
                    IsAvailable = true
                })
                .ToListAsync(stoppingToken);

            if (newRecords.Any())
            {
                await db.RoomAvailabilities.AddRangeAsync(newRecords, stoppingToken);
                await db.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(GetDelayUntilNextRun(), stoppingToken);
        }
    }


    // Job runs at 1am everynight
    private static TimeSpan GetDelayUntilNextRun(int hour = 1)
    {
        var now = DateTime.Now;
        var nextRun = now.Date.AddDays(1).AddHours(hour);
        return nextRun - now;
    }
}