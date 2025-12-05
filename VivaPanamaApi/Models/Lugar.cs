using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class Lugar
    {
        [Key]
        public int id_Lugar { get; set; }
        public string mombre_lugar { get; set; }
        public string descripcion_lugar { get; set; }
        public string provincia_lugar { get; set; }
        public string tipo_lugar { get; set; }
    }
}
