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
                .FirstOrDefaultAsync(c => c.Id_Calificacion == id);

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
                    new { id = calificacion.Id_Calificacion },
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
            if (id != calificacion.Id_Calificacion)
            {
                return BadRequest("El id de la ruta no coincide con el id del cuerpo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await applicationDbContext.Calificaciones
                .FirstOrDefaultAsync(c => c.Id_Calificacion == id);

            if (existing == null)
            {
                return NotFound();
            }

            // Actualizar campos permitidos
            existing.Puntuacion = calificacion.Puntuacion;
            existing.Comentario = calificacion.Comentario;
            existing.Id_Usuario = calificacion.Id_Usuario;
            existing.Id_Lugar = calificacion.Id_Lugar;
            existing.Fecha = calificacion.Fecha;

            try
            {
                await applicationDbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await applicationDbContext.Calificaciones
                    .AnyAsync(e => e.Id_Calificacion == id))
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
                .FirstOrDefaultAsync(c => c.Id_Calificacion == id);

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
                .Where(c => c.Id_Lugar == idLugar)
                .AverageAsync(c => (double?)c.Puntuacion);

            if (promedio == null)
                return NotFound("No hay calificaciones para este lugar.");

            return Ok(new { Id_Lugar = idLugar, Promedio = promedio });
        }

    }
}
