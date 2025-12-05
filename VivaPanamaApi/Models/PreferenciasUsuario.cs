using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VivaPanamaApi.Models
{
    [Table("preferencias_usuario")]
    public class PreferenciasUsuario
    {
        [Key]
        public int id_preferencia { get; set; }

        [ForeignKey("usuario")]  
        public int id_usuario { get; set; }

        public decimal? presupuesto_maximo { get; set; }

        public string? tipo_viajero { get; set; }

        public string? intereses { get; set; }

        public string? nivel_actividad { get; set; }

        public string? tipo_alojamiento_preferido { get; set; }

       
        public Usuario? usuario { get; set; }
    }
}
