using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Models;
using static System.Net.Mime.MediaTypeNames;

namespace VivaPanamaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets usando nombres exactos de tablas
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Lugar> lugar { get; set; }
        public DbSet<imagen> imagen { get; set; }
        public DbSet<Hotel> hotel { get; set; }
        public DbSet<Restaurante> restaurante { get; set; }
        public DbSet<ActividadLugar> actividad_lugar { get; set; }
        public DbSet<Itinerario> itinerario { get; set; }
        public DbSet<DiaItinerario> dia_itinerario { get; set; }
        public DbSet<ActividadItinerario> actividad_itinerario { get; set; }
        public DbSet<HotelItinerario> hotel_itinerario { get; set; }
        public DbSet<RestauranteItinerario> restaurante_itinerario { get; set; }
        public DbSet<LugarItinerario> lugar_itinerario { get; set; }
        public DbSet<PreferenciasUsuario> preferencias_usuario { get; set; }
        public DbSet<Calificacion> calificacion { get; set; }
        public DbSet<Favorito> favorito { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tablas
            modelBuilder.Entity<Usuario>().ToTable("usuario");
            modelBuilder.Entity<Lugar>().ToTable("lugar");
            modelBuilder.Entity<registro>().ToTable("registro");

            // Relaciones
            modelBuilder.Entity<Hotel>()
                .HasOne(h => h.Lugar)
                .WithMany()
                .HasForeignKey(h => h.id_lugar);

            modelBuilder.Entity<Restaurante>()
                .HasOne(r => r.Lugar)
                .WithMany()
                .HasForeignKey(r => r.id_lugar);

            modelBuilder.Entity<ActividadLugar>()
                .HasOne(a => a.Lugar)
                .WithMany()
                .HasForeignKey(a => a.id_lugar);

            modelBuilder.Entity<Itinerario>()
                .HasOne(i => i.Usuario)
                .WithMany()
                .HasForeignKey(i => i.id_usuario);

            modelBuilder.Entity<registro>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.Id_Usuario)
                .HasConstraintName("FK_Registro_Usuario");

            modelBuilder.Entity<registro>()
                .HasOne(r => r.Lugar)
                .WithMany()
                .HasForeignKey(r => r.Id_Lugar)
                .HasConstraintName("FK_Registro_Lugar");

            modelBuilder.Entity<Itinerario>()
                .HasMany(i => i.Dias)
                .WithOne(d => d.Itinerario)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}