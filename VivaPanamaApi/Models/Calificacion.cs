using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VivaPanamaApi.Models
{
    public class Calificacion
    {
        [Key]
        public int id_calificacion { get; set; }

        [ForeignKey("usuario")]
        public int id_usuario { get; set; }

        public string tipo_entidad { get; set; } = string.Empty;

        public int id_entidad { get; set; }

        public int puntuacion { get; set; }

        public string? comentario { get; set; }

        public DateTime? fecha_calificacion { get; set; }

        public Usuario? usuario { get; set; }
    }
}
