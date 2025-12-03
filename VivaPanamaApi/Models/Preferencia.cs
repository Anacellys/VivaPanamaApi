using VivaPanamaApi.Models;

namespace VivaPanamaApi.Models
{
    public class Preferencia
    {
        public int Id { get; set; }

        // Llave foránea a Usuario
        public int UsuarioId { get; set; }

        // Ejemplos de preferencias (pon lo que pidieron en el proyecto)
        public string TemaColor { get; set; }
        public bool NotificacionesEmail { get; set; }

        // Relación
        public Usuario Usuario { get; set; }
    }
}
