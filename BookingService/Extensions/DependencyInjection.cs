using BookingService.Application.Services;
using BookingService.Domain.Repositories;
using BookingService.Infrastructure.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookingService, Application.Services.BookingService>();

        return services;
    }
}