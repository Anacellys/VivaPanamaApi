using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Data;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActividadesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActividadesController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET: api/Actividades
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<object>> GetActividades()
        {
            var actividades = await _context.actividad
                .Include(a => a.Lugar)
                .Select(a => new
                {
                    a.id_actividad,
                    a.nombre,
                    a.descripcion,
                    a.costo,
                    a.horario,
                    a.disponibilidad,
                    Lugar = new
                    {
                        a.Lugar.id_lugar,
                        a.Lugar.nombre,
                        a.Lugar.provincia
                    }
                })
                .ToListAsync();

            return Ok(actividades);
        }

        // ============================================================
        // GET: api/Actividades/{id}
        // ============================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetActividad(int id)
        {
            var actividad = await _context.actividad
                .Include(a => a.Lugar)
                .FirstOrDefaultAsync(a => a.id_actividad == id);

            if (actividad == null)
                return NotFound("Actividad no encontrada");

            return Ok(new
            {
                actividad.id_actividad,
                actividad.nombre,
                actividad.descripcion,
                actividad.costo,
                actividad.horario,
                actividad.disponibilidad,
                Lugar = actividad.Lugar
            });
        }

        // ============================================================
        // POST: api/Actividades
        // ============================================================
        [HttpPost]
        public async Task<ActionResult> PostActividad(Actividad actividad)
        {
            if (string.IsNullOrWhiteSpace(actividad.nombre))
                return BadRequest("El nombre es obligatorio.");

            var lugar = await _context.lugar.FindAsync(actividad.id_lugar);
            if (lugar == null)
                return BadRequest("El lugar no existe.");

            _context.actividad.Add(actividad);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Actividad creada exitosamente",
                Actividad = actividad
            });
        }

        // ============================================================
        // PUT: api/Actividades/{id}
        // ============================================================
        [HttpPut("{id}")]
        public async Task<ActionResult> PutActividad(int id, Actividad actividad)
        {
            if (id != actividad.id_actividad)
                return BadRequest("ID no coincide.");

            var existente = await _context.actividad.FindAsync(id);
            if (existente == null)
                return NotFound("Actividad no encontrada.");

            existente.nombre = actividad.nombre;
            existente.descripcion = actividad.descripcion;
            existente.costo = actividad.costo;
            existente.horario = actividad.horario;
            existente.disponibilidad = actividad.disponibilidad;
            existente.id_lugar = actividad.id_lugar;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Actividad actualizada",
                Actividad = existente
            });
        }

        // ============================================================
        // DELETE: api/Actividades/{id}
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActividad(int id)
        {
            var actividad = await _context.actividad.FindAsync(id);
            if (actividad == null)
                return NotFound("Actividad no existe.");

            _context.actividad.Remove(actividad);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Actividad eliminada",
                Actividad = actividad
            });
        }
    }
}
