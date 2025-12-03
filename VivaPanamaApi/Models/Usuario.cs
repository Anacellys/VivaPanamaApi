using Microsoft.Win32;

namespace VivaPanamaApi.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        public string Nombre { get; set; }

        // Cedula o pasaporte del usuario (debe ser único)
        public string CedulaPasaporte { get; set; }

        public int Edad { get; set; }

        // Email único
        public string Email { get; set; }

        // Relación 1 a 1s con Preferencia
        public Preferencias Preferencia { get; set; }

        // Solo lectura (no vas a hacer CRUD de estos de momento)
        public List<Calificacion> Calificaciones { get; set; }
    }
}
