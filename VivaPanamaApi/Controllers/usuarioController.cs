using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Data;
using VivaPanamaApi.Models;
using BCrypt.Net; // Necesitarás instalar el paquete BCrypt.Net-Next

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
        // POST: api/Usuarios/Login
        // =======================
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel login)
        {
            if (login == null || string.IsNullOrEmpty(login.nombre_usuario) || string.IsNullOrEmpty(login.password))
            {
                return BadRequest("Nombre de usuario y contraseña son requeridos");
            }

            // Buscar usuario por nombre de usuario
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.nombre_usuario == login.nombre_usuario);

            if (usuario == null)
            {
                return Unauthorized("Usuario no encontrado");
            }

            // Verificar contraseña (comparar el hash)
            if (!BCrypt.Net.BCrypt.Verify(login.password, usuario.password))
            {
                return Unauthorized("Contraseña incorrecta");
            }

            // Login exitoso
            return Ok(new
            {
                mensaje = "Login exitoso",
                id = usuario.id_usuario,
                nombre_usuario = usuario.nombre_usuario,
                email_usuario = usuario.email_usuario,
                tipo_usuario = usuario.tipo_usuario,
                cedula_pasaporte = usuario.cedula_pasaporte
            });
        }

        // =======================
        // POST: api/Usuarios/Registrar
        // =======================
        [HttpPost("registrar")]
        public async Task<ActionResult<Usuario>> Registrar([FromBody] Usuario usuario)
        {
            // Validaciones básicas
            if (string.IsNullOrEmpty(usuario.nombre_usuario) || string.IsNullOrEmpty(usuario.password))
            {
                return BadRequest("Nombre de usuario y contraseña son requeridos");
            }

            // Validar unicidad de nombre de usuario
            if (await _context.Usuario.AnyAsync(u => u.nombre_usuario == usuario.nombre_usuario))
            {
                return BadRequest("El nombre de usuario ya está registrado");
            }

            // Validar unicidad de email si se proporciona
            if (!string.IsNullOrEmpty(usuario.email_usuario) &&
                await _context.Usuario.AnyAsync(u => u.email_usuario == usuario.email_usuario))
            {
                return BadRequest("El email ya está registrado");
            }

            // Validar unicidad de cédula/pasaporte si se proporciona
            if (!string.IsNullOrEmpty(usuario.cedula_pasaporte) &&
                await _context.Usuario.AnyAsync(u => u.cedula_pasaporte == usuario.cedula_pasaporte))
            {
                return BadRequest("La cédula/pasaporte ya está registrada");
            }

            // Encriptar la contraseña antes de guardar
            usuario.password = BCrypt.Net.BCrypt.HashPassword(usuario.password);

            // Asignar tipo de usuario por defecto si no se especifica
            if (string.IsNullOrEmpty(usuario.tipo_usuario))
            {
                usuario.tipo_usuario = "cliente";
            }

            // Guardar el usuario
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            // No devolver la contraseña en la respuesta
            usuario.password = null;

            return CreatedAtAction("GetUsuario", new { id = usuario.id_usuario }, usuario);
        }

        // =======================
        // GET: api/Usuarios
        // =======================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _context.Usuario.ToListAsync();

            // No devolver contraseñas en la lista
            usuarios.ForEach(u => u.password = null);

            return usuarios;
        }

        // =======================
        // GET: api/Usuarios/5
        // =======================
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.id_usuario == id);

            if (usuario == null)
                return NotFound();

            // No devolver la contraseña
            usuario.password = null;

            return usuario;
        }

        // =======================
        // POST: api/Usuarios (Mantener para compatibilidad)
        // =======================
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario([FromBody] Usuario usuario)
        {
            // Usar el método Registrar en su lugar
            return await Registrar(usuario);
        }

        // =======================
        // PUT: api/Usuarios/5
        // =======================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.id_usuario)
                return BadRequest("El ID no coincide.");

            // Verificar que el usuario existe
            var usuarioExistente = await _context.Usuario.FindAsync(id);
            if (usuarioExistente == null)
                return NotFound();

            // Validar unicidad al actualizar
            if (!string.IsNullOrEmpty(usuario.email_usuario) &&
                await _context.Usuario.AnyAsync(u => u.email_usuario == usuario.email_usuario && u.id_usuario != id))
                return BadRequest("El email ya está registrado por otro usuario.");

            if (!string.IsNullOrEmpty(usuario.cedula_pasaporte) &&
                await _context.Usuario.AnyAsync(u => u.cedula_pasaporte == usuario.cedula_pasaporte && u.id_usuario != id))
                return BadRequest("La cédula/pasaporte ya está registrada por otro usuario.");

            // Si se actualiza la contraseña, encriptarla
            if (!string.IsNullOrEmpty(usuario.password) && usuario.password != usuarioExistente.password)
            {
                usuario.password = BCrypt.Net.BCrypt.HashPassword(usuario.password);
            }
            else
            {
                // Mantener la contraseña actual si no se cambia
                usuario.password = usuarioExistente.password;
            }

            // Mantener el tipo de usuario si no se especifica
            if (string.IsNullOrEmpty(usuario.tipo_usuario))
            {
                usuario.tipo_usuario = usuarioExistente.tipo_usuario;
            }

            // Actualizar usuario
            _context.Entry(usuarioExistente).CurrentValues.SetValues(usuario);

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
            return _context.Usuario.Any(e => e.id_usuario == id);
        }
    }
}