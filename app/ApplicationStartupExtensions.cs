using Microsoft.EntityFrameworkCore;
using ApiDemo;

public static class ApplicationStartupExtensions
{
    private const string Email = "admin@owaspdemo.org";
    public static async Task EnsureVehicleDatabaseIsUpToDate(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dataContext = scope.ServiceProvider.GetRequiredService<VehicleContext>();
        await dataContext.Database.MigrateAsync();

        if (!await dataContext.Vehicles.AnyAsync())
        {
            await dataContext.Vehicles.AddRangeAsync(
                new Vehicle { VIN = "123", EngineRunning = false, Locked = true, Owner = "admin@owaspdemo.org" },
                new Vehicle { VIN = "456", EngineRunning = false, Locked = true, Owner = "admin@owaspdemo.org" },
                new Vehicle { VIN = "789", EngineRunning = false, Locked = false, Owner = "demo@owaspdemo.org" },
                new Vehicle { VIN = "007", EngineRunning = false, Locked = false, Owner = "test@owaspdemo.org" }
            );
            await dataContext.SaveChangesAsync();
        }

    }



}