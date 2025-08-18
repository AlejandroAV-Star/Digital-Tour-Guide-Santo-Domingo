using DTourGuide.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DTourGuide.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Place> Places => Set<Place>();
        public DbSet<Photo> Photos => Set<Photo>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Place>()
                .HasMany(p => p.Photos)
                .WithOne(p => p.Place)
                .HasForeignKey(p => p.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Place>().HasData(
                new Place
                {
                    Id = 1,
                    Name = "Catedral Primada de América",
                    Category = Category.Iglesia,
                    Latitude = 18.472028,
                    Longitude = -69.882141,
                    Address = "Calle Arzobispo Meriño, Zona Colonial",
                    OpeningHours = "L-D 9:00-16:30",
                    Description = "Primera catedral de América."
                },
                new Place
                {
                    Id = 2,
                    Name = "Alcázar de Colón",
                    Category = Category.Museo,
                    Latitude = 18.473396,
                    Longitude = -69.881012,
                    Address = "Plaza de España, Zona Colonial",
                    OpeningHours = "M-D 10:00-18:00",
                    Description = "Palacio virreinal del siglo XVI."
                },
                new Place
                {
                    Id = 3,
                    Name = "Parque Colón",
                    Category = Category.Plaza,
                    Latitude = 18.472693,
                    Longitude = -69.882905,
                    Address = "Zona Colonial",
                    OpeningHours = "Abierto 24h",
                    Description = "Plaza principal con estatua de Colón."
                }
            );
        }
    }
}

