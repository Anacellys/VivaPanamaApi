using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class imagen
    {
        [Key]
        public int id_imagen { get; set; }
        public string tipo_entidad { get; set; }
        public int id_entidad { get; set; }
        public string url_imagen { get; set; }
        public string descripcion_imagen { get; set; }
        public bool es_principal { get; set; }
        public DateTime fecha_subida { get; set; }
    }
}
