namespace VivaPanamaApi.Models
{
    public class HotelHospedaje
    {
        public int id_hotel { get; set; }
        public int id_lugar { get; set; }
        public string nombre { get; set; }
        public decimal? precio_noche { get; set; }
        public string servicios { get; set; }
    }
}
