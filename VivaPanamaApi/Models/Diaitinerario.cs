using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class DiaItinerario
    {
        [Key]
        public int id_dia { get; set; }
        public int id_itinerario { get; set; }
        public int numero_dia { get; set; }
        public DateOnly? fecha_dia { get; set; }
        public decimal? presupuesto_dia { get; set; }
        public string notas_dia { get; set; }

        // Navigation properties
        public Itinerario Itinerario { get; set; }
        public ICollection<ActividadItinerario> Actividades { get; set; }
        public ICollection<HotelItinerario> Hoteles { get; set; }
        public ICollection<RestauranteItinerario> Restaurantes { get; set; }
        public ICollection<LugarItinerario> Lugares { get; set; }
    }
}