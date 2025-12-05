using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using VivaPanamaApi.Contexts;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroVisitasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegistroVisitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RegistroVisitas
        [HttpGet]
        public async Task<ActionResult<List<registro>>> GetVisitas()
        {
            return await _context.Registro
                .Include(r => r.Usuario)
                .Include(r => r.Lugar)
                .ToListAsync();
        }

        // GET: api/RegistroVisitas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<registro>> GetVisita(int id)
        {
            var visita = await _context.Registro
                .Include(r => r.Usuario)
                .Include(r => r.Lugar)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (visita == null)
                return NotFound();

            return Ok(visita);
        }

        // POST: api/RegistroVisitas
        [HttpPost]
        public async Task<ActionResult<registro>> CrearVisita(registro visita)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Registro.Add(visita);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetVisita),
                    new { id = visita.Id },
                    visita);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al crear el registro de visita.");
            }
        }

        // PUT: api/RegistroVisitas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVisita(int id, registro visita)
        {
            if (id != visita.Id)
                return BadRequest("El id de la ruta no coincide con el id del cuerpo.");

            var existing = await _context.Registro.FirstOrDefaultAsync(r => r.Id == id);
            if (existing == null)
                return NotFound();

            existing.Id_Usuario = visita.Id_Usuario;
            existing.Id_Lugar = visita.Id_Lugar;
            existing.Fecha_Entrada = visita.Fecha_Entrada;
            existing.Fecha_Salida = visita.Fecha_Salida;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Registro.AnyAsync(r => r.Id == id))
                    return NotFound();

                throw;
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al actualizar el registro de visita.");
            }
        }

        // DELETE: api/RegistroVisitas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisita(int id)
        {
            var visita = await _context.Registro.FirstOrDefaultAsync(r => r.Id == id);
            if (visita == null)
                return NotFound();

            try
            {
                _context.Registro.Remove(visita);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al eliminar el registro de visita.");
            }
        }

        // GET: api/RegistroVisitas/historial/usuario/1
        [HttpGet("historial/usuario/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<registro>>> HistorialPorUsuario(int idUsuario)
        {
            var historial = await _context.Registro
                .Include(r => r.Lugar)
                .Where(r => r.Id_Usuario == idUsuario)
                .OrderBy(r => r.Fecha_Entrada)
                .ToListAsync();

            return Ok(historial);
        }

        // GET: api/RegistroVisitas/reportes/mas-visitados
        [HttpGet("reportes/mas-visitados")]
        public async Task<ActionResult<IEnumerable<object>>> LugaresMasVisitados()
        {
            var lugares = await _context.Registro
                .GroupBy(r => r.Id_Lugar)
                .Select(g => new
                {
                    Id_Lugar = g.Key,
                    CantidadVisitas = g.Count()
                })
                .OrderByDescending(g => g.CantidadVisitas)
                .ToListAsync();

            return Ok(lugares);
        }

        // GET: api/RegistroVisitas/reportes/mejor-calificados
        [HttpGet("reportes/mejor-calificados")]
        public async Task<ActionResult<IEnumerable<object>>> LugaresMejorCalificados()
        {
            var lugares = await _context.Calificaciones
                .GroupBy(c => c.id_lugar)
                .Select(g => new
                {
                    Id_Lugar = g.Key,
                    Promedio = g.Average(c => c.puntuacion)
                })
                .OrderByDescending(g => g.Promedio)
                .ToListAsync();

            return Ok(lugares);
        }

       
    }
}
