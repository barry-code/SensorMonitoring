using Microsoft.EntityFrameworkCore;
using System.Reflection;
using SensorMonitoring.Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;

namespace SensorMonitoring.Api.Repository;

public class SensorContext : DbContext
{
    public DbSet<Sensor> Sensors { get; set; }
    public DbSet<SensorReading> SensorReadings { get; set; }

    public string DbPath { get; }

    public SensorContext(IOptions<ApiOptions> options)
    {
        DbPath = options.Value.SensorRepositoryConnection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlite($"DataSource={DbPath}");

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            foreach (var entityType in model.Model.GetEntityTypes())
            {
                var props = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTimeOffset) || p.GetType() == typeof(DateTimeOffset?));

                foreach (var prop in props)                
                {
                    model.Entity(entityType.Name)
                        .Property(prop.Name)
                        .HasConversion(new DateTimeOffsetToBinaryConverter());
                }
            }
        }

        model.Entity<Sensor>()            
            .HasKey(s => s.Id);

        model.Entity<Sensor>()
            .HasMany<SensorReading>()
            .WithOne(s => s.Sensor)
            .HasForeignKey(s => s.SensorId);

        model.Entity<SensorReading>()            
            .HasKey(sr => sr.Id);
    }

}
