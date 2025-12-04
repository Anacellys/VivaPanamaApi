using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Contexts  
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Restaurante> Restaurantes { get; set; }
        public DbSet<Preferencias> Preferencias { get; set; }
        public DbSet<Lugar> Lugares { get; set; }
        public DbSet<HotelHospedaje> HotelHospedajes { get; set; }
        public DbSet<Actividad> Actividades { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Fotos> Fotos { get; set; }
        public DbSet<registro> Registro { get; set; }
    }
}
