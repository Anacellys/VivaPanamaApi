using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class Calificacion
    {
        [Key]
        public int id_calificacion { get; set; }

        public int id_usuario { get; set; }

        public int id_lugar { get; set; }

        public int puntuacion { get; set; }

        public string? comentario { get; set; }

        public DateTime? fecha { get; set; }

        public Usuario? Usuario { get; set; }
        public Lugar? Lugar { get; set; }
    }
}
