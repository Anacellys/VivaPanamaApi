using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class Lugar
    {
        [Key]
        public int id_lugar { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string provincia { get; set; }
        public string tipo_lugar { get; set; }
    }
}
