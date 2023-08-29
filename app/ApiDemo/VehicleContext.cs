

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiDemo;

public class VehicleContext : DbContext {
    public VehicleContext(DbContextOptions<VehicleContext> options): base(options)
    {
        
    }
    public DbSet<Vehicle> Vehicles { get; set; } = null!;
}

public class Vehicle
{
    [Key]
    public string VIN { get; set; } = string.Empty;
    public bool Locked { get;set; }
    public bool EngineRunning {get;set;}
    public string Owner { get;set; } = string.Empty;
}