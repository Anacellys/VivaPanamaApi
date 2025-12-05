using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class LugarItinerario
    {
        [Key]
        public int id_lugar_itinerario { get; set; }        // MOD

        public int id_dia { get; set; }                     // MOD

        public int id_lugar { get; set; }                   // MOD

        public int? orden_visita { get; set; }              // MOD

        public int? tiempo_estimado_visita { get; set; }    // MOD

        public string? notas_visita { get; set; }           // MOD

        // Navigation properties
        public DiaItinerario? dia_itinerario { get; set; }  // MOD

        public Lugar? lugar { get; set; }                   // MOD
    }
}
