using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VivaPanamaApi.Models
{
    [Table("favorito")]
    public class Favorito
    {
        [Key]
        public int id_favorito { get; set; }

        [ForeignKey("Usuario")]   
        public int id_usuario { get; set; }

        public string tipo_entidad { get; set; }

        public int id_entidad { get; set; }

        public DateTime fecha_agregado { get; set; }

        public string? notas { get; set; }  // <-- CAMBIO 2

        // Navigation property
        public Usuario Usuario { get; set; }
    }
}
