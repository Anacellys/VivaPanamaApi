using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class Actividad
    {
        [Key]
        public int id_actividad { get; set; }  

        public int id_Lugar { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public decimal costo { get; set; }
        public string horario { get; set; }
        public string disponibilidad { get; set; }

        public Lugar? Lugar { get; set; }
    }
}
