using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Contexts;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalificacionesController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;

        public CalificacionesController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        // GET: api/Calificaciones
        [HttpGet]
        public async Task<ActionResult<List<Calificacion>>> GetCalificaciones()
        {
            return await applicationDbContext.Calificaciones.ToListAsync();
        }

        // GET: api/Calificaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Calificacion>> GetCalificacion(int id)
        {
            var calificacion = await applicationDbContext.Calificaciones
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

            try
            {
                applicationDbContext.Calificaciones.Add(calificacion);
                await applicationDbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCalificacion),
                    new { id = calificacion.id_calificacion },
                    calificacion);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al crear la Calificación.");
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

            var existing = await applicationDbContext.Calificaciones
                .FirstOrDefaultAsync(c => c.id_calificacion == id);

            if (existing == null)
            {
                return NotFound();
            }

            // Actualizar campos permitidos
            existing.puntuacion = calificacion.puntuacion;
            existing.comentario = calificacion.comentario;
            existing.id_usuario = calificacion.id_usuario;
            existing.id_lugar = calificacion.id_lugar;
            existing.fecha = calificacion.fecha;

            try
            {
                await applicationDbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await applicationDbContext.Calificaciones
                    .AnyAsync(e => e.id_calificacion == id))
                {
                    return NotFound();
                }

                throw;
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al actualizar la Calificación.");
            }
        }

        // DELETE: api/Calificaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCalificacion(int id)
        {
            var calificacion = await applicationDbContext.Calificaciones
                .FirstOrDefaultAsync(c => c.id_calificacion == id);

            if (calificacion == null)
            {
                return NotFound();
            }

            try
            {
                applicationDbContext.Calificaciones.Remove(calificacion);
                await applicationDbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al eliminar la Calificación.");
            }
        }

        [HttpGet("promedio/lugar/{idLugar}")]
        public async Task<ActionResult<object>> PromedioPorLugar(int idLugar)
        {
            // Usar el contexto correcto inyectado: applicationDbContext
            var promedio = await applicationDbContext.Calificaciones
                .Where(c => c.id_lugar == idLugar)
                .AverageAsync(c => (double?)c.puntuacion);

            if (promedio == null)
                return NotFound("No hay calificaciones para este lugar.");

            return Ok(new { Id_Lugar = idLugar, Promedio = promedio });
        }

    }
}
