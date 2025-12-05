using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class Itinerario
    {
        [Key]
        public int id_itinerario { get; set; }
        public int id_usuario { get; set; }
        public string nombre_itinerario { get; set; }
        public string descripcion_itinerario { get; set; }
        public DateTime fecha_creacion { get; set; }
        public DateOnly? fecha_inicio { get; set; }
        public DateOnly? fecha_fin { get; set; }
        public decimal? presupuesto_total { get; set; }
        public string estado { get; set; }
        public bool es_publico { get; set; }

        // Navigation property
        public Usuario Usuario { get; set; }

        // Collection navigation
        public ICollection<DiaItinerario> Dias { get; set; }
    }
}