using System;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using VibroDiagnostic.Core.Entities;

namespace VibroDiagnostic.Data.Contexts
{
    public class SensorContext : DbContext
    {
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorFile> SensorFiles { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<User> Users { get; set; }
        
        public SensorContext(DbContextOptions<SensorContext> options) : base(options)
        {
        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "System",
                    LastName = "",
                    Username = "System",
                    Password = "System",
                }
            );
            modelBuilder.Entity<Equipment>().HasData(
                new Equipment()
                {
                    Id = 1,
                    Name = "TestEquipment"
                }
            );
            modelBuilder.Entity<Sensor>().HasData(
                new Sensor()
                {
                    Id = 1,
                    EquipmentId = 1,
                    Title = "SensorId"
                }
            );
        }
    }
}

