using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class ActividadLugar
    {
        [Key]
        public int id_actividad { get; set; }
        public int id_lugar { get; set; }
        public string nombre_actividad { get; set; }
        public string descripcion_actividad { get; set; }
        public decimal? costo_actividad { get; set; }
        public int? duracion_estimada { get; set; }
        public TimeSpan? horario_apertura { get; set; }
        public TimeSpan? horario_cierre { get; set; }
        public string dificultad_actividad { get; set; }
        public string equipo_requerido { get; set; }
        public string restricciones { get; set; }

        // Navigation property
        public Lugar Lugar { get; set; }
    }
}