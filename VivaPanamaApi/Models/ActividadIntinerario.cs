using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class ActividadItinerario
    {
        [Key]
        public int id_actividad_itinerario { get; set; }
        public int id_dia { get; set; }
        public int id_actividad { get; set; }
        public int orden { get; set; }
        public TimeSpan? hora_inicio { get; set; }
        public TimeSpan? hora_fin { get; set; }
        public string notas_actividad { get; set; }
        public string estado { get; set; }
        public decimal? costo_real { get; set; }

        // Navigation properties
        public DiaItinerario dia { get; set; }
        public ActividadLugar actividad { get; set; }
    }
}