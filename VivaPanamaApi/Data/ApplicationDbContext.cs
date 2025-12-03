using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Data
{
    // Esta clase es el "puente" entre tu código C# y la base de datos PostgreSQL
    public class ApplicationDbContext : DbContext
    {
        // El constructor recibe las opciones (cadena de conexión, proveedor, etc.)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet = representa la tabla "usuario" de tu base de datos
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
