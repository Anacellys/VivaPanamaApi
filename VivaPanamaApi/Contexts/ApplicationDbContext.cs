using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Contexts
{
    public class ApplicationDbContext : DbContext   // ← Modificación 1: heredar de DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ← Modificación 2: agregar DbSet para la tabla calificacion
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<HotelHospedaje> HotelHospedaje { get; set; }
        public DbSet<Restaurante> Restaurantes { get; set; }
        public DbSet<Actividad> Actividades { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Actividad>()
                .ToTable("actividad")
                .HasKey(a => a.id_actividad);

            modelBuilder.Entity<Calificacion>()
                .ToTable("calificacion")
                .HasKey(c => c.Id_Calificacion);

            modelBuilder.Entity<Fotos>()
                .ToTable("fotos")
                .HasKey(f => f.IdFoto);

            modelBuilder.Entity<HotelHospedaje>()
                .ToTable("hotel_hospedaje")
                .HasKey(h => h.id_hotel);

            modelBuilder.Entity<Lugar>()
                .ToTable("lugar")
                .HasKey(l => l.IdLugar);

            modelBuilder.Entity<Preferencias>()
                .ToTable("preferencias")
                .HasKey(p => p.IdUsuario);

            modelBuilder.Entity<Restaurante>()
                .ToTable("restaurante")
                .HasKey(r => r.id_restaurante);

            modelBuilder.Entity<Usuario>()
                .ToTable("usuario")
                .HasKey(u => u.IdUsuario);

        }


    }
}
