namespace VivaPanamaApi.Models
{
    public class Calificacion
    {
        public int Id_Calificacion { get; set; }

        public int Id_Usuario { get; set; }

        public int Id_Lugar { get; set; }

        public int Puntuacion { get; set; }

        public string? Comentario { get; set; }

        public DateTime? Fecha { get; set; }

        public Usuario? Usuario { get; set; }
        public Lugar? Lugar { get; set; }
    }
}
