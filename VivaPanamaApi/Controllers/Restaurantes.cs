using Microsoft.AspNetCore.Mvc;
using VivaPanamaApi.Contexts;
using VivaPanamaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace VivaPanamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Restaurantes : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;
         public Restaurantes(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }
        [HttpGet]
        public async Task<ActionResult<List<Restaurante>>> GetRestaurantes()
        {
            return await applicationDbContext.Restaurantes.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurante>> GetRestaurante(int id)
        {
            var restaurante = await applicationDbContext.Restaurantes.FindAsync(id);
            if (restaurante == null) return NotFound();
            return Ok(restaurante);
        }
        [HttpPost]
        public async Task<ActionResult<Restaurante>> CrearRestaurante(Restaurante restaurante)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                applicationDbContext.Restaurantes.Add(restaurante);
                await applicationDbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetRestaurante), new { id = restaurante.id_restaurante }, restaurante);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al crear el Restaurante");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurante(int id, Restaurante restaurante)
        {
            if (restaurante == null)
            {
                return BadRequest("El restaurante enviado es nulo.");
            }

            if (id != restaurante.id_restaurante)
            {
                return BadRequest("El id en la ruta no coincide con el id en el cuerpo.");
            }

            if (string.IsNullOrWhiteSpace(restaurante.horarios_disponibles))
            {
                return BadRequest("Debe especificar al menos un horario disponible.");
            }

            var existing = await applicationDbContext.Restaurantes.FindAsync(id);
            if (existing == null)
            {
                return NotFound("No se encontró el restaurante con el id especificado.");
            }

            // Solo se actualizan los horarios disponibles
            existing.horarios_disponibles = restaurante.horarios_disponibles;

            try
            {
                await applicationDbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await applicationDbContext.Restaurantes.AnyAsync(r => r.id_restaurante == id))
                {
                    return NotFound("El restaurante ya no existe.");
                }
                throw;
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error al actualizar el restaurante.");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurante(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El id proporcionado no es válido.");
            }

            var restaurante = await applicationDbContext.Restaurantes.FindAsync(id);
            if (restaurante == null)
            {
                return NotFound("No se encontró el restaurante con el id especificado.");
            }

            try
            {
                applicationDbContext.Restaurantes.Remove(restaurante);
                await applicationDbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error al eliminar el restaurante.");
            }
        }
        [HttpGet("por-lugar/{idLugar}")]
        public async Task<ActionResult<List<Restaurante>>> GetRestaurantesPorLugar(int idLugar)
        {
            return await applicationDbContext.Restaurantes
                .Where(r => r.id_lugar == idLugar)
                .ToListAsync();
        }


    }
}
