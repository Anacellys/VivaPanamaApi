using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Data;
using VivaPanamaApi.Models;
using BCrypt.Net;

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

        // ==========================================================
        // POST: api/Usuarios/login
        // ==========================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if (string.IsNullOrWhiteSpace(login.nombre_usuario) || string.IsNullOrWhiteSpace(login.password))
                return BadRequest("Debe ingresar usuario y contraseña.");

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.nombre == login.nombre_usuario);

            if (usuario == null)
                return Unauthorized("Usuario no encontrado.");

            if (!BCrypt.Net.BCrypt.Verify(login.password, usuario.password))
                return Unauthorized("Contraseña incorrecta.");

            return Ok(new
            {
                mensaje = "Login exitoso",
                id_usuario = usuario.id_usuario,
                nombre = usuario.nombre,
                email = usuario.email,
                tipo_usuario = usuario.tipo_usuario,
                cedula_pasaporte = usuario.cedula_pasaporte
            });
        }

        // ==========================================================
        // POST: api/Usuarios/registrar
        // ==========================================================
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.nombre) || string.IsNullOrWhiteSpace(usuario.password))
                return BadRequest("Debe ingresar nombre y contraseña.");

            if (await _context.Usuario.AnyAsync(u => u.nombre == usuario.nombre))
                return BadRequest("El nombre de usuario ya está registrado.");

            if (!string.IsNullOrWhiteSpace(usuario.email) &&
                await _context.Usuario.AnyAsync(u => u.email == usuario.email))
                return BadRequest("El email ya está registrado.");

            usuario.password = BCrypt.Net.BCrypt.HashPassword(usuario.password);

            if (string.IsNullOrWhiteSpace(usuario.tipo_usuario))
                usuario.tipo_usuario = "cliente";

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            usuario.password = null; // No devolver contraseña

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.id_usuario }, usuario);
        }

        // ==========================================================
        // GET: api/Usuarios
        // ==========================================================
        [HttpGet]
        public async Task<IEnumerable<Usuario>> GetUsuarios()
        {
            var lista = await _context.Usuario.ToListAsync();
            lista.ForEach(u => u.password = null);
            return lista;
        }

        // ==========================================================
        // GET: api/Usuarios/{id}
        // ==========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            usuario.password = null;
            return Ok(usuario);
        }

        // ==========================================================
        // PUT: api/Usuarios/{id}
        // ==========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario datos)
        {
            var usuario = await _context.Usuario.FindAsync(id);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            // Validar email único SOLO si lo envían
            if (!string.IsNullOrWhiteSpace(datos.email) &&
                await _context.Usuario.AnyAsync(u => u.email == datos.email && u.id_usuario != id))
                return BadRequest("El email ya está en uso.");

            // 🔹 ACTUALIZAR SOLO LO QUE VIENE (NO SOBREESCRIBIR NULL)
            usuario.nombre = string.IsNullOrWhiteSpace(datos.nombre) ? usuario.nombre : datos.nombre;
            usuario.email = string.IsNullOrWhiteSpace(datos.email) ? usuario.email : datos.email;
            usuario.cedula_pasaporte = string.IsNullOrWhiteSpace(datos.cedula_pasaporte)
                                        ? usuario.cedula_pasaporte
                                        : datos.cedula_pasaporte;

            // Si no envía edad → mantener
            usuario.edad = datos.edad == 0 ? usuario.edad : datos.edad;

            // 🔹 CONTRASEÑA SOLO SI ENVÍAN UNA NUEVA
            if (!string.IsNullOrWhiteSpace(datos.password))
                usuario.password = BCrypt.Net.BCrypt.HashPassword(datos.password);

            // 🔹 MANTENER tipo_usuario SIEMPRE
            usuario.tipo_usuario = usuario.tipo_usuario;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // ==========================================================
        // DELETE: api/Usuarios/{id}
        // ==========================================================
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
    }
}
