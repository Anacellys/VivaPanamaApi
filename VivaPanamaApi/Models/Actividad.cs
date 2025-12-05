namespace VivaPanamaApi.Models
{
    public class Actividad
    {
        public int id_actividad { get; set; }
        public int id_lugar { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public decimal? costo { get; set; }
        public string horario { get; set; }
        public string disponibilidad { get; set; }
    }
}
