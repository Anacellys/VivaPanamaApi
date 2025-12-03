namespace VivaPanamaApi.Models
{
    public class Actividad
    {
        public int IdActividad { get; set; }
        public int IdLugar { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal? Costo { get; set; }
        public string Horario { get; set; }
        public string Disponibilidad { get; set; }
    }
}
