namespace VivaPanamaApi.Models
{
    public class HotelHospedaje
    {
        public int IdHotel { get; set; }
        public int IdLugar { get; set; }
        public string Nombre { get; set; }
        public decimal? PrecioNoche { get; set; }
        public string Servicios { get; set; }
    }
}
