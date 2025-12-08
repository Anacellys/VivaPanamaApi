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

            // Aplicar filtros
            if (!string.IsNullOrEmpty(provincia))
            {
                query = query.Where(h => h.Lugar.provincia == provincia);
            }

            if (precioMin.HasValue)
            {
                query = query.Where(h => h.precio_noche >= precioMin.Value);
            }

            if (precioMax.HasValue)
            {
                query = query.Where(h => h.precio_noche <= precioMax.Value);
            }

            if (calificacionMin.HasValue)
            {
                query = query.Where(h => h.calificacion_promedio >= calificacionMin.Value);
            }

            var hoteles = await query
                .Select(h => new
                {
                    h.id_hotel,
                    h.nombre_hotel,
                    h.descripcion_hotel,
                    h.precio_noche,
                    h.calificacion_promedio,
                    h.servicios_hotel,
                    h.telefono_hotel,
                    Lugar = new
                    {
                        h.Lugar.id_Lugar,
                        h.Lugar.mombre,
                        h.Lugar.provincia
                    },
                    Imagenes = _context.imagen
                        .Where(i => i.tipo_entidad == "hotel" && i.id_entidad == h.id_hotel && i.es_principal)
                        .Select(i => new { i.url_imagen, i.descripcion_imagen })
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(hoteles);
        }

        // GET: api/Hoteles/5 - Hotel específico con TODA la info
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetHotel(int id)
        {
            var hotel = await _context.hotel
                .Include(h => h.Lugar)
                .FirstOrDefaultAsync(h => h.id_hotel == id);

            if (hotel == null)
            {
                return NotFound();
            }

            // Obtener todas las imágenes del hotel
            var imagenes = await _context.imagen
                .Where(i => i.tipo_entidad == "hotel" && i.id_entidad == id)
                .OrderByDescending(i => i.es_principal)
                .Select(i => new
                {
                    i.id_imagen,
                    i.url_imagen,
                    i.descripcion_imagen,
                    i.es_principal
                })
                .ToListAsync();

            // Obtener calificaciones del hotel
            var calificaciones = await _context.calificacion
                .Where(c => c.tipo_entidad == "hotel" && c.id_entidad == id)
                .Include(c => c.usuario)
                .Select(c => new
                {
                    c.id_calificacion,
                    c.puntuacion,
                    c.comentario,
                    c.fecha_calificacion,
                    Usuario = new { c.usuario.nombre_usuario }
                })
                .OrderByDescending(c => c.fecha_calificacion)
                .Take(10)
                .ToListAsync();

            // Calcular promedio de calificaciones
            var promedioCalificaciones = calificaciones.Any()
                ? calificaciones.Average(c => c.puntuacion)
                : 0;

            // Hoteles similares (misma provincia)
            var hotelesSimilares = await _context.hotel
                .Include(h => h.Lugar)
                .Where(h => h.Lugar.provincia == hotel.Lugar.provincia && h.id_hotel != id)
                .Take(3)
                .Select(h => new
                {
                    h.id_hotel,
                    h.nombre_hotel,
                    h.precio_noche,
                    h.calificacion_promedio,
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "hotel" && i.id_entidad == h.id_hotel && i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(new
            {
                Hotel = new
                {
                    hotel.id_hotel,
                    hotel.nombre_hotel,
                    hotel.descripcion_hotel,
                    hotel.precio_noche,
                    hotel.calificacion_promedio,
                    hotel.servicios_hotel,
                    hotel.telefono_hotel,
                    Lugar = new
                    {
                        hotel.Lugar.id_Lugar,
                        hotel.Lugar.mombre,
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

        // POST: api/Hoteles - Crear nuevo hotel
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(Hotel hotel)
        {
            // Validar que el lugar existe
            var lugarExiste = await _context.lugar.AnyAsync(l => l.id_Lugar == hotel.id_lugar);
            if (!lugarExiste)
            {
                _context.lugar.Add(hotel.Lugar);
                await _context.SaveChangesAsync();
                hotel.id_lugar = hotel.Lugar.id_Lugar;
            }

            _context.hotel.Add(hotel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHotel", new { id = hotel.id_hotel }, hotel);
        }

        // PUT: api/Hoteles/5 - Actualizar hotel
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, Hotel hotel)
        {
            if (id != hotel.id_hotel)
            {
                return BadRequest();
            }

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Hoteles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _context.hotel.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            _context.hotel.Remove(hotel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Hoteles/buscar?q=term
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarHoteles([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest("Término de búsqueda requerido");
            }

            var hoteles = await _context.hotel
                .Include(h => h.Lugar)
                .Where(h => h.nombre_hotel.Contains(q) ||
                           h.descripcion_hotel.Contains(q) ||
                           h.Lugar.mombre.Contains(q) ||
                           h.Lugar.provincia.Contains(q))
                .Select(h => new
                {
                    h.id_hotel,
                    h.nombre_hotel,
                    h.precio_noche,
                    h.calificacion_promedio,
                    Lugar = new
                    {
                        h.Lugar.mombre,
                        h.Lugar.provincia
                    },
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "hotel" && i.id_entidad == h.id_hotel && i.es_principal)
                        .Select(i => i.url_imagen)
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
                    h.nombre_hotel,
                    h.precio_noche,
                    h.calificacion_promedio,
                    Descuento = h.precio_noche * 0.8m, // 20% de descuento simulado
                    Lugar = h.Lugar.provincia,
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "hotel" && i.id_entidad == h.id_hotel && i.es_principal)
                        .Select(i => i.url_imagen)
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