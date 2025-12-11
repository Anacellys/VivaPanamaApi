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
                    },

                    // 👇 NUEVA IMAGEN
                    Imagen = a.imagen_url
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

            // Imagen opcional → si viene vacía, asignamos un placeholder
            if (string.IsNullOrWhiteSpace(actividad.imagen_url))
            {
                actividad.imagen_url = "https://i.imgur.com/NT2F1gr.png"; // 🔹 placeholder default
            }

            _context.actividad.Add(actividad);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Actividad creada exitosamente",
                Actividad = new
                {
                    actividad.id_actividad,
                    actividad.nombre,
                    actividad.descripcion,
                    actividad.costo,
                    actividad.horario,
                    actividad.disponibilidad,
                    actividad.id_lugar,
                    actividad.imagen_url
                }
            });
        }


        // ============================================================
        // PUT: api/Actividades/{id}
        // ============================================================
        [HttpPut("{id}")]
        public async Task<ActionResult> PutActividad(int id, Actividad actividad)
        {
            var existente = await _context.actividad.FindAsync(id);
            if (existente == null)
                return NotFound("Actividad no encontrada.");

            // Actualizar valores
            existente.nombre = actividad.nombre;
            existente.descripcion = actividad.descripcion;
            existente.costo = actividad.costo;
            existente.horario = actividad.horario;
            existente.disponibilidad = actividad.disponibilidad;
            existente.id_lugar = actividad.id_lugar;

            // Imagen opcional
            if (!string.IsNullOrWhiteSpace(actividad.imagen_url))
                existente.imagen_url = actividad.imagen_url;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Actividad actualizada",
                Actividad = new
                {
                    existente.id_actividad,
                    existente.nombre,
                    existente.descripcion,
                    existente.costo,
                    existente.horario,
                    existente.disponibilidad,
                    existente.id_lugar,
                    existente.imagen_url
                }
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
