using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class Restaurante
    {
        [Key]
        public int id_restaurante { get; set; }
        public int id_lugar { get; set; }
        public string nombre_restaurante { get; set; }
        public string descripcion_restaurante { get; set; }
        public string tipo_cocina_restaurante { get; set; }
        public decimal? precio_promedio { get; set; }
        public decimal? calificacion_promedio { get; set; }
        public TimeSpan? horario_apertura { get; set; }
        public TimeSpan? horario_cierre { get; set; }

        public Lugar Lugar { get; set; }
    }
}