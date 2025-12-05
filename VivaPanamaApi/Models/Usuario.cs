using System.ComponentModel.DataAnnotations;

namespace VivaPanamaApi.Models
{
    public class Usuario
    {
        [Key]
        public int id_usuario { get; set; }

        [Required]
        public string nombre_usuario { get; set; }

        [Required]
        public string password { get; set; }

        public string cedula_pasaporte { get; set; }
        public int? edad_usuario { get; set; }
        public string email_usuario { get; set; }

        [Required]
        public string tipo_usuario { get; set; }
    }
}