using System;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Models
{
    public class registro
    {
        public int Id { get; set; } 

        public int Id_Usuario { get; set; }
        public int Id_Lugar { get; set; }  

        public DateTime Fecha_Entrada { get; set; }
        public DateTime Fecha_Salida { get; set; }

        public Usuario? Usuario { get; set; }
        public Lugar? Lugar { get; set; }
    }
}
