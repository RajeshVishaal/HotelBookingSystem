using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using UserService.Application.Services;
using UserService.Application.Services.Interfaces;
using UserService.Infrastructure.Data.Repositories;

namespace UserService.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUserService, Application.Services.UserService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}