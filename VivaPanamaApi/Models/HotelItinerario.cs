using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class HotelItinerario
    {
        [Key]
        public int id_hotel_itinerario { get; set; }   // MOD: nombre igual a la BD

        public int id_dia { get; set; }               // MOD: coincide con la BD
        public int id_hotel { get; set; }             // MOD: coincide con la BD

        public DateOnly? fecha_checkin { get; set; }  // MOD: nombre idéntico a la BD
        public DateOnly? fecha_checkout { get; set; } // MOD

        public int? numero_noches { get; set; }       // MOD: respeta nombre igual a la BD

        public decimal? costo_total { get; set; }     // MOD: numeric(10,2)

        public string? notas_hospedaje { get; set; }  // MOD: TEXT → string?, puede ser null

        // Navigation properties
        public DiaItinerario? dia_itinerario { get; set; } // MOD: nombre según relación
        public Hotel? hotel { get; set; }                  // MOD
    }
}
