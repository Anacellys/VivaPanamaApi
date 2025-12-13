using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Data;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RestaurantesController(AppDbContext context)
        {
            _context = context;
        }

        // ==================== GET GENERAL ====================
        [HttpGet]
        public async Task<IActionResult> GetRestaurantes(
            [FromQuery] string provincia = null,
            [FromQuery] string tipoCocina = null,
            [FromQuery] decimal? precioMin = null,
            [FromQuery] decimal? precioMax = null)
        {
            var query = _context.restaurante
                .Include(r => r.Lugar)
                .AsQueryable();

            if (!string.IsNullOrEmpty(provincia))
                query = query.Where(r => r.Lugar.provincia == provincia);

            if (!string.IsNullOrEmpty(tipoCocina))
                query = query.Where(r => r.tipo_cocina_restaurante.Contains(tipoCocina));

            if (precioMin.HasValue)
                query = query.Where(r => r.precio_promedio >= precioMin);

            if (precioMax.HasValue)
                query = query.Where(r => r.precio_promedio <= precioMax);

            var restaurantes = await query
                .OrderBy(r => r.nombre_restaurante)
                .Select(r => new
                {
                    r.id_restaurante,
                    r.nombre_restaurante,
                    r.descripcion_restaurante,
                    r.tipo_cocina_restaurante,
                    r.precio_promedio,
                    r.horario_apertura,
                    r.horario_cierre,
                    Lugar = new
                    {
                        r.Lugar.id_lugar,
                        r.Lugar.nombre,
                        r.Lugar.provincia
                    },
                    ImagenPrincipal = _context.imagen
                        .Where(i => i.id_lugar == r.id_lugar)
                        .Select(i => new { i.url, i.descripcion })
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(new
            {
                Total = restaurantes.Count,
                Restaurantes = restaurantes
            });
        }

        // ==================== GET POR ID ====================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestaurante(int id)
        {
            var restaurante = await _context.restaurante
                .Include(r => r.Lugar)
                .FirstOrDefaultAsync(r => r.id_restaurante == id);

            if (restaurante == null)
                return NotFound("Restaurante no encontrado");

            var imagenes = await _context.imagen
                .Where(i => i.id_lugar == restaurante.id_lugar)
                .Select(i => new
                {
                    i.id_foto,
                    i.url,
                    i.descripcion
                })
                .ToListAsync();

            return Ok(new
            {
                Restaurante = new
                {
                    restaurante.id_restaurante,
                    restaurante.nombre_restaurante,
                    restaurante.descripcion_restaurante,
                    restaurante.tipo_cocina_restaurante,
                    restaurante.precio_promedio,
                    Horario = restaurante.horario_apertura.HasValue && restaurante.horario_cierre.HasValue
                        ? $"{restaurante.horario_apertura:hh\\:mm} - {restaurante.horario_cierre:hh\\:mm}"
                        : "No especificado",
                    Lugar = new
                    {
                        restaurante.Lugar.id_lugar,
                        restaurante.Lugar.nombre,
                        restaurante.Lugar.provincia
                    }
                },
                Imagenes = imagenes
            });
        }

        // ==================== POST ====================
        [HttpPost]
        public async Task<IActionResult> PostRestaurante(Restaurante restaurante)
        {
            _context.restaurante.Add(restaurante);
            await _context.SaveChangesAsync();
            return Ok(restaurante);
        }

        // ==================== PUT ====================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRestaurante(int id, Restaurante restaurante)
        {
            if (id != restaurante.id_restaurante)
                return BadRequest("ID incorrecto");

            _context.Entry(restaurante).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(restaurante);
        }

        // ==================== DELETE ====================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurante(int id)
        {
            var restaurante = await _context.restaurante.FindAsync(id);
            if (restaurante == null)
                return NotFound("Restaurante no encontrado");

            var imagenes = await _context.imagen
                .Where(i => i.id_lugar == restaurante.id_lugar)
                .ToListAsync();

            if (imagenes.Any())
                _context.imagen.RemoveRange(imagenes);

            _context.restaurante.Remove(restaurante);
            await _context.SaveChangesAsync();

            return Ok("Restaurante eliminado correctamente");
        }

        // ==================== POR LUGAR ====================
        [HttpGet("por-lugar/{idLugar}")]
        public async Task<IActionResult> GetRestaurantesPorLugar(int idLugar)
        {
            var restaurantes = await _context.restaurante
                .Where(r => r.id_lugar == idLugar)
                .Select(r => new
                {
                    r.id_restaurante,
                    r.nombre_restaurante,
                    r.tipo_cocina_restaurante,
                    r.precio_promedio
                })
                .OrderBy(r => r.nombre_restaurante)
                .ToListAsync();

            return Ok(restaurantes);
        }
    }
}
