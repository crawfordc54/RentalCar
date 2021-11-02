using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Lab6.Data
{
    public class ReservationSystemContext : DbContext
    {
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<RentableVehicle> RentableVehicles { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options
                .UseLazyLoadingProxies()
                .UseSqlite($@"Data Source={Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}ReservationSystem.sqlite");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("NOCASE");
        }
    }
}
