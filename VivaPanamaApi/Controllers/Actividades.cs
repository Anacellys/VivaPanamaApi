using Microsoft.AspNetCore.Mvc;
using VivaPanamaApi.Contexts;
using VivaPanamaApi.Models;
using Microsoft.EntityFrameworkCore;
namespace VivaPanamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Actividades : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;

        public Actividades(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }
        [HttpGet]
        public async Task<ActionResult<List<Actividad>>> GetActividades()
        {
            return await applicationDbContext.Actividades.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Actividad>> GetActividad(int id)
        {
            var actividad = await applicationDbContext.Actividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound("No se encontró la actividad con el id especificado.");
            }
            return Ok(actividad);
        }

        [HttpPost]
        public async Task<ActionResult<Actividad>> CrearActividad(Actividad actividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                applicationDbContext.Actividades.Add(actividad);
                await applicationDbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetActividad), new { id = actividad.id_actividad }, actividad);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al crear la actividad.");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActividad(int id, Actividad actividad)
        {
            if (id != actividad.id_actividad)
            {
                return BadRequest("El id en la ruta no coincide con el id en el cuerpo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await applicationDbContext.Actividades.FindAsync(id);
            if (existing == null)
            {
                return NotFound("No se encontró la actividad con el id especificado.");
            }

            existing.nombre = actividad.nombre;
            existing.horario = actividad.horario;
            existing.costo = actividad.costo;
            existing.disponibilidad = actividad.disponibilidad;
            existing.id_lugar = actividad.id_lugar;

            try
            {
                await applicationDbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await applicationDbContext.Actividades.AnyAsync(a => a.id_actividad == id))
                {
                    return NotFound("La actividad ya no existe.");
                }
                throw;
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error al actualizar la actividad.");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActividad(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El id proporcionado no es válido.");
            }

            var actividad = await applicationDbContext.Actividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound("No se encontró la actividad con el id especificado.");
            }

            try
            {
                applicationDbContext.Actividades.Remove(actividad);
                await applicationDbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error al eliminar la actividad.");
            }
        }
        [HttpGet("por-disponibilidad/{disponible}")]
        public async Task<ActionResult<List<Actividad>>> GetPorDisponibilidad(string disponible)
        {
            return await applicationDbContext.Actividades
                .Where(a => a.disponibilidad == disponible)
                .ToListAsync();
        }

        [HttpGet("por-horario/{horario}")]
        public async Task<ActionResult<List<Actividad>>> GetPorHorario(string horario)
        {
            return await applicationDbContext.Actividades
                .Where(a => a.horario == horario)
                .ToListAsync();
        }

        [HttpGet("por-costo/{costo}")]
        public async Task<ActionResult<List<Actividad>>> GetPorCosto(decimal costo)
        {
            return await applicationDbContext.Actividades
                .Where(a => a.costo <= costo)
                .ToListAsync();
        }

    }
}
