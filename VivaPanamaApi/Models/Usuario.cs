using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VivaPanamaApi.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        public int id_usuario { get; set; }

        [Required]
        public string nombre { get; set; }

        public string? cedula_pasaporte { get; set; }

        public int? edad { get; set; }

        public string? email { get; set; }

        
        public string? password { get; set; }


        public string? tipo_usuario { get; set; }
    }
}
