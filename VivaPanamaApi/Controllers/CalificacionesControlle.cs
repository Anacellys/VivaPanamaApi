using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Data;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalificacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CalificacionesController(AppDbContext context) // O ApplicationDbContext
        {
            _context = context;
        }

        // GET: api/Calificaciones
        [HttpGet]
        public async Task<ActionResult<List<Calificacion>>> GetCalificaciones()
        {
            return await _context.calificacion.ToListAsync();
        }

        // GET: api/Calificaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Calificacion>> GetCalificacion(int id)
        {
            var calificacion = await _context.calificacion
                .FirstOrDefaultAsync(c => c.id_calificacion == id);

            if (calificacion == null)
            {
                return NotFound();
            }

            return Ok(calificacion);
        }

        // POST: api/Calificaciones
        [HttpPost]
        public async Task<ActionResult<Calificacion>> CrearCalificacion(Calificacion calificacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Asignar fecha actual si no viene
            if (calificacion.fecha_calificacion == default)
            {
                calificacion.fecha_calificacion = DateTime.UtcNow;
            }

            // Validar que el usuario existe
            var usuarioExiste = await _context.Usuario
                .AnyAsync(u => u.id_usuario == calificacion.id_usuario);

            if (!usuarioExiste)
            {
                return BadRequest("El usuario no existe");
            }

            // Validar que la entidad calificada existe según su tipo
            bool entidadExiste = calificacion.tipo_entidad switch
            {
                "lugar" => await _context.lugar.AnyAsync(l => l.id_Lugar == calificacion.id_entidad),
                "hotel" => await _context.hotel.AnyAsync(h => h.id_hotel == calificacion.id_entidad),
                "restaurante" => await _context.restaurante.AnyAsync(r => r.id_restaurante == calificacion.id_entidad),
                "actividad" => await _context.actividad_lugar.AnyAsync(a => a.id_actividad == calificacion.id_entidad),
                _ => false
            };

            if (!entidadExiste)
            {
                return BadRequest($"La entidad de tipo '{calificacion.tipo_entidad}' con ID {calificacion.id_entidad} no existe");
            }

            // Validar que la puntuación esté entre 1 y 5
            if (calificacion.puntuacion < 1 || calificacion.puntuacion > 5)
            {
                return BadRequest("La puntuación debe estar entre 1 y 5");
            }

            try
            {
                _context.calificacion.Add(calificacion);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCalificacion),
                    new { id = calificacion.id_calificacion },
                    calificacion);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Ocurrió un error al crear la Calificación: {ex.Message}");
            }
        }

        // PUT: api/Calificaciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCalificacion(int id, Calificacion calificacion)
        {
            if (id != calificacion.id_calificacion)
            {
                return BadRequest("El id de la ruta no coincide con el id del cuerpo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await _context.calificacion
                .FirstOrDefaultAsync(c => c.id_calificacion == id);

            if (existing == null)
            {
                return NotFound();
            }

            // Validar que la puntuación esté entre 1 y 5
            if (calificacion.puntuacion < 1 || calificacion.puntuacion > 5)
            {
                return BadRequest("La puntuación debe estar entre 1 y 5");
            }

            // Actualizar campos permitidos
            existing.puntuacion = calificacion.puntuacion;
            existing.comentario = calificacion.comentario;
            existing.fecha_calificacion = calificacion.fecha_calificacion;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.calificacion.AnyAsync(e => e.id_calificacion == id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Ocurrió un error al actualizar la Calificación: {ex.Message}");
            }
        }

        // DELETE: api/Calificaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCalificacion(int id)
        {
            var calificacion = await _context.calificacion
                .FirstOrDefaultAsync(c => c.id_calificacion == id);

            if (calificacion == null)
            {
                return NotFound();
            }

            try
            {
                _context.calificacion.Remove(calificacion);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Ocurrió un error al eliminar la Calificación: {ex.Message}");
            }
        }

        // GET: api/Calificaciones/usuario/5
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<List<Calificacion>>> GetCalificacionesPorUsuario(int idUsuario)
        {
            var calificaciones = await _context.calificacion
                .Where(c => c.id_usuario == idUsuario)
                .ToListAsync();

            if (!calificaciones.Any())
            {
                return NotFound($"No hay calificaciones para el usuario con ID {idUsuario}");
            }

            return Ok(calificaciones);
        }

        // GET: api/Calificaciones/entidad/lugar/5
        [HttpGet("entidad/{tipoEntidad}/{idEntidad}")]
        public async Task<ActionResult<List<Calificacion>>> GetCalificacionesPorEntidad(string tipoEntidad, int idEntidad)
        {
            // Validar tipo de entidad
            string[] tiposValidos = { "lugar", "hotel", "restaurante", "actividad" };
            if (!tiposValidos.Contains(tipoEntidad.ToLower()))
            {
                return BadRequest($"Tipo de entidad '{tipoEntidad}' no válido. Tipos válidos: {string.Join(", ", tiposValidos)}");
            }

            var calificaciones = await _context.calificacion
                .Where(c => c.tipo_entidad == tipoEntidad && c.id_entidad == idEntidad)
                .ToListAsync();

            if (!calificaciones.Any())
            {
                return NotFound($"No hay calificaciones para la entidad {tipoEntidad} con ID {idEntidad}");
            }

            return Ok(calificaciones);
        }

        // GET: api/Calificaciones/promedio/entidad/lugar/5
        [HttpGet("promedio/entidad/{tipoEntidad}/{idEntidad}")]
        public async Task<ActionResult<object>> GetPromedioPorEntidad(string tipoEntidad, int idEntidad)
        {
            // Validar tipo de entidad
            string[] tiposValidos = { "lugar", "hotel", "restaurante", "actividad" };
            if (!tiposValidos.Contains(tipoEntidad.ToLower()))
            {
                return BadRequest($"Tipo de entidad '{tipoEntidad}' no válido. Tipos válidos: {string.Join(", ", tiposValidos)}");
            }

            var promedio = await _context.calificacion
                .Where(c => c.tipo_entidad == tipoEntidad && c.id_entidad == idEntidad)
                .AverageAsync(c => (double?)c.puntuacion);

            if (promedio == null)
            {
                return NotFound($"No hay calificaciones para la entidad {tipoEntidad} con ID {idEntidad}");
            }

            // Contar cuántas calificaciones hay
            var cantidad = await _context.calificacion
                .Where(c => c.tipo_entidad == tipoEntidad && c.id_entidad == idEntidad)
                .CountAsync();

            return Ok(new
            {
                Tipo_Entidad = tipoEntidad,
                Id_Entidad = idEntidad,
                Promedio = Math.Round(promedio.Value, 2),
                Cantidad_Calificaciones = cantidad
            });
        }

        // GET: api/Calificaciones/resumen/usuario/5
        [HttpGet("resumen/usuario/{idUsuario}")]
        public async Task<ActionResult<object>> GetResumenCalificacionesUsuario(int idUsuario)
        {
            // Verificar que el usuario existe
            var usuarioExiste = await _context.Usuario.AnyAsync(u => u.id_usuario == idUsuario);
            if (!usuarioExiste)
            {
                return NotFound($"Usuario con ID {idUsuario} no encontrado");
            }

            var calificaciones = await _context.calificacion
                .Where(c => c.id_usuario == idUsuario)
                .ToListAsync();

            var resumen = new
            {
                Usuario_Id = idUsuario,
                Total_Calificaciones = calificaciones.Count,
                Por_Tipo_Entidad = calificaciones
                    .GroupBy(c => c.tipo_entidad)
                    .Select(g => new
                    {
                        Tipo_Entidad = g.Key,
                        Cantidad = g.Count(),
                        Promedio = Math.Round(g.Average(c => c.puntuacion), 2)
                    })
                    .ToList(),
                Ultimas_Calificaciones = calificaciones
                    .OrderByDescending(c => c.fecha_calificacion)
                    .Take(5)
                    .Select(c => new
                    {
                        c.id_calificacion,
                        c.tipo_entidad,
                        c.id_entidad,
                        c.puntuacion,
                        c.fecha_calificacion
                    })
                    .ToList()
            };

            return Ok(resumen);
        }
    }
}