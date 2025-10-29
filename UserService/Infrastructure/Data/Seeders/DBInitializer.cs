using Bogus;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data.Seeders;

public static class DBInitializer
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync())
            return;

        var tx = await db.Database.BeginTransactionAsync();
        try
        {
            var faker = new Faker("en_GB");
            var users = new List<User>();

            for (var i = 0; i < 20; i++)
            {
                var first = faker.Name.FirstName();
                var last = faker.Name.LastName();
                var email = $"{first.ToLower()}.{last.ToLower()}@{faker.Internet.DomainName()}";

                var user = new User
                {
                    FirstName = first,
                    LastName = last,
                    EmailAddress = email.ToLowerInvariant(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123")
                };
                users.Add(user);
            }

            await db.Users.AddRangeAsync(users);
            await db.SaveChangesAsync();
            await tx.CommitAsync();

            Console.WriteLine("Seeded users dataset successfully.");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            Console.WriteLine($"User seeding failed: {ex.Message}");
            throw;
        }
    }
}