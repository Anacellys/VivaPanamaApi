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
    }
}
