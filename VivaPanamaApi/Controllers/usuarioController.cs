using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Data;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // =======================
        // GET: api/Usuarios
        // =======================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuario       // MOD: corregido nombre del DbSet
                .ToListAsync();                // MOD: eliminado Include que no existe
        }

        // =======================
        // GET: api/Usuarios/5
        // =======================
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuario     // MOD
                .FirstOrDefaultAsync(u => u.id_usuario == id);   // MOD

            if (usuario == null)
                return NotFound();

            return usuario;
        }

        // =======================
        // POST: api/Usuarios
        // =======================
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // Validaciones de unicidad
            if (await _context.Usuario.AnyAsync(u => u.cedula_pasaporte == usuario.cedula_pasaporte))  // MOD
                return BadRequest("La cédula/pasaporte ya está registrada.");

            if (await _context.Usuario.AnyAsync(u => u.email_usuario == usuario.email_usuario))         // MOD
                return BadRequest("El email ya está registrado.");

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.id_usuario }, usuario); // MOD
        }

        // =======================
        // PUT: api/Usuarios/5
        // =======================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.id_usuario) // MOD
                return BadRequest("El ID no coincide.");

            // Validar unicidad al actualizar
            if (await _context.Usuario.AnyAsync(u => u.email_usuario == usuario.email_usuario && u.id_usuario != id)) // MOD
                return BadRequest("El email ya está registrado por otro usuario.");

            if (await _context.Usuario.AnyAsync(u => u.cedula_pasaporte == usuario.cedula_pasaporte && u.id_usuario != id)) // MOD
                return BadRequest("La cédula/pasaporte ya está registrada por otro usuario.");

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // =======================
        // DELETE: api/Usuarios/5
        // =======================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
                return NotFound();

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.id_usuario == id); // MOD
        }
    }
}
