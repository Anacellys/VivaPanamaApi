using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
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

        
            modelBuilder.Entity<Usuario>().ToTable("usuario");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.nombre)
                .IsRequired();

           

            modelBuilder.Entity<Usuario>()
                .Property(u => u.tipo_usuario)
                .HasDefaultValue("cliente");

            
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.nombre)
                .IsUnique();

            
            modelBuilder.Entity<Lugar>().ToTable("lugar");
            modelBuilder.Entity<imagen>().ToTable("imagen");
            modelBuilder.Entity<Hotel>().ToTable("hotel");
            modelBuilder.Entity<Restaurante>().ToTable("restaurante");
            modelBuilder.Entity<ActividadLugar>().ToTable("actividad_lugar");
            modelBuilder.Entity<Itinerario>().ToTable("itinerario");
            modelBuilder.Entity<DiaItinerario>().ToTable("dia_itinerario");
            modelBuilder.Entity<ActividadItinerario>().ToTable("actividad_itinerario");
            modelBuilder.Entity<HotelItinerario>().ToTable("hotel_itinerario");
            modelBuilder.Entity<RestauranteItinerario>().ToTable("restaurante_itinerario");
            modelBuilder.Entity<LugarItinerario>().ToTable("lugar_itinerario");
            modelBuilder.Entity<PreferenciasUsuario>().ToTable("preferencias_usuario");
            modelBuilder.Entity<Calificacion>().ToTable("calificacion");
            modelBuilder.Entity<Favorito>().ToTable("favorito");

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

            modelBuilder.Entity<Itinerario>()
                .HasMany(i => i.Dias)
                .WithOne(d => d.Itinerario)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}