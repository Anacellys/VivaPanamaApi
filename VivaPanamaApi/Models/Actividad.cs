using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VivaPanamaApi.Models
{
    [Table("actividad")]
    public class Actividad
    {
        [Key]
        public int id_actividad { get; set; }

        public int id_lugar { get; set; }

        public string nombre { get; set; }

        public string descripcion { get; set; }

        public decimal costo { get; set; }

        public string horario { get; set; }

        public string disponibilidad { get; set; }
        public string? imagen_url { get; set; }

        public Lugar Lugar { get; set; }

    }
}
