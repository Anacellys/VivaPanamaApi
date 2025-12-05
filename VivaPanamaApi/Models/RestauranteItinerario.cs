using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class RestauranteItinerario
    {
        [Key]
        public int id_restaurante_itinerario { get; set; }   // MOD

        public int id_dia { get; set; }                     // MOD

        public int id_restaurante { get; set; }             // MOD

        public string? tipo_comida { get; set; }            // MOD

        public TimeSpan? hora_reserva { get; set; }         // MOD

        public int? numero_personas { get; set; }           // MOD

        public decimal? costo_estimado { get; set; }        // MOD

        public string? notas_restaurante { get; set; }      // MOD

        // Navigation properties
        public DiaItinerario? dia_itinerario { get; set; }  // MOD

        public Restaurante? restaurante { get; set; }       // MOD
    }
}
