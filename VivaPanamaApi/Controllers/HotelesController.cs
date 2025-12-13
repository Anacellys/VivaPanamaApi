using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Data;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HotelesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Hoteles - Todos los hoteles con filtros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHoteles(
            [FromQuery] string provincia = null,
            [FromQuery] decimal? precioMin = null,
            [FromQuery] decimal? precioMax = null,
            [FromQuery] decimal? calificacionMin = null)
        {
            var query = _context.hotel
                .Include(h => h.Lugar)
                .AsQueryable();

            if (!string.IsNullOrEmpty(provincia))
                query = query.Where(h => h.Lugar.provincia == provincia);

            if (precioMin.HasValue)
                query = query.Where(h => h.precio_noche >= precioMin.Value);

            if (precioMax.HasValue)
                query = query.Where(h => h.precio_noche <= precioMax.Value);


            var hoteles = await query
                .Select(h => new
                {
                    h.id_hotel,
                    // la tabla tiene "nombre"; se expone como nombre_hotel para el front
                    nombre_hotel = h.nombre,
                    // estos campos pueden no existir en la tabla, pero se exponen igual si están en el modelo
                    h.precio_noche,
                    servicios_hotel = h.servicios,
                    Lugar = new
                    {
                        h.Lugar.id_lugar,
                        h.Lugar.nombre,
                        h.Lugar.provincia
                    },
                    Imagenes = _context.imagen
                        .Where(i => i.id_lugar == h.id_lugar)
                        .Select(i => new { url = i.url, descripcion = i.descripcion })
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(hoteles);
        }

        // GET: api/Hoteles/5 - Hotel específico
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetHotel(int id)
        {
            var hotel = await _context.hotel
                .Include(h => h.Lugar)
                .FirstOrDefaultAsync(h => h.id_hotel == id);

            if (hotel == null)
                return NotFound();

            var imagenes = await _context.imagen
                .Where(i => i.id_lugar == hotel.id_lugar)
                .Select(i => new
                {
                    id_imagen = i.id_foto,
                    i.url,
                    i.descripcion
                })
                .ToListAsync();

            var calificaciones = await _context.calificacion
                .Where(c => c.tipo_entidad == "hotel" && c.id_entidad == id)
                .Include(c => c.usuario)
                .Select(c => new
                {
                    c.id_calificacion,
                    c.puntuacion,
                    c.comentario,
                    c.fecha_calificacion,
                    Usuario = new { c.usuario.nombre }
                })
                .OrderByDescending(c => c.fecha_calificacion)
                .Take(10)
                .ToListAsync();

            var promedioCalificaciones = calificaciones.Any()
                ? calificaciones.Average(c => c.puntuacion)
                : 0;

            var hotelesSimilares = await _context.hotel
                .Include(h => h.Lugar)
                .Where(h => h.Lugar.provincia == hotel.Lugar.provincia && h.id_hotel != id)
                .Take(3)
                .Select(h => new
                {
                    h.id_hotel,
                    nombre_hotel = h.nombre,
                    h.precio_noche,
                    Imagen = _context.imagen
                        .Where(i => i.id_lugar == h.id_lugar)
                        .Select(i => i.url)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(new
            {
                Hotel = new
                {
                    hotel.id_hotel,
                    nombre_hotel = hotel.nombre,
                    hotel.precio_noche,
                    servicios_hotel = hotel.servicios,
                    Lugar = new
                    {
                        hotel.Lugar.id_lugar,
                        hotel.Lugar.nombre,
                        hotel.Lugar.descripcion,
                        hotel.Lugar.provincia,
                        hotel.Lugar.tipo_lugar
                    }
                },
                Imagenes = imagenes,
                Calificaciones = new
                {
                    Lista = calificaciones,
                    Promedio = Math.Round(promedioCalificaciones, 1),
                    Total = calificaciones.Count
                },
                Hoteles_Similares = hotelesSimilares
            });
        }

        // POST: api/Hoteles
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(Hotel hotel)
        {
            var lugarExiste = await _context.lugar.AnyAsync(l => l.id_lugar == hotel.id_lugar);
            if (!lugarExiste)
            {
                _context.lugar.Add(hotel.Lugar);
                await _context.SaveChangesAsync();
                hotel.id_lugar = hotel.Lugar.id_lugar;
            }

            _context.hotel.Add(hotel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHotel", new { id = hotel.id_hotel }, hotel);
        }

        // PUT: api/Hoteles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, Hotel hotel)
        {
            if (id != hotel.id_hotel)
                return BadRequest();

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Hoteles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _context.hotel.FindAsync(id);
            if (hotel == null)
                return NotFound();

            _context.hotel.Remove(hotel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Hoteles/buscar?q=term
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarHoteles([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Término de búsqueda requerido");

            var hoteles = await _context.hotel
                .Include(h => h.Lugar)
                .Where(h => h.nombre.Contains(q) ||
                            h.Lugar.nombre.Contains(q) ||
                            h.Lugar.provincia.Contains(q))
                .Select(h => new
                {
                    h.id_hotel,
                    nombre_hotel = h.nombre,
                    h.precio_noche,
                    Lugar = new
                    {
                        h.Lugar.nombre,
                        h.Lugar.provincia
                    },
                    Imagen = _context.imagen
                        .Where(i => i.id_lugar == h.id_lugar)
                        .Select(i => i.url)
                        .FirstOrDefault()
                })
                .Take(10)
                .ToListAsync();

            return Ok(hoteles);
        }

        // GET: api/Hoteles/mejores-ofertas
        [HttpGet("mejores-ofertas")]
        public async Task<ActionResult<IEnumerable<object>>> GetMejoresOfertas()
        {
            var ofertas = await _context.hotel
                .Include(h => h.Lugar)
                .Where(h => h.precio_noche > 0)
                .OrderBy(h => h.precio_noche)
                .Take(6)
                .Select(h => new
                {
                    h.id_hotel,
                    nombre_hotel = h.nombre,
                    h.precio_noche,
                    Descuento = h.precio_noche * 0.8m,
                    Lugar = h.Lugar.provincia,
                    Imagen = _context.imagen
                        .Where(i => i.id_lugar == h.id_lugar)
                        .Select(i => i.url)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(ofertas);
        }

        private bool HotelExists(int id)
        {
            return _context.hotel.Any(e => e.id_hotel == id);
        }
    }
}
