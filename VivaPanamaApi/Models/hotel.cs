using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class Hotel
    {
        [Key]
        public int id_hotel { get; set; }
        public int id_lugar { get; set; }
        public string nombre_hotel { get; set; }
        public string descripcion_hotel { get; set; }
        public decimal? precio_noche { get; set; }
        public decimal? calificacion_promedio { get; set; }
        public string servicios_hotel { get; set; }
        public string telefono_hotel { get; set; }

      
        public Lugar Lugar { get; set; }
    }
}