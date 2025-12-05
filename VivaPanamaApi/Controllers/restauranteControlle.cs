using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VivaPanamaApi.Data;
using VivaPanamaApi.Models;

namespace VivaPanamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RestaurantesController(AppDbContext context)
        {
            _context = context;
        }

        // ==================== CRUD BÁSICO ====================

        // GET: api/Restaurantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRestaurantes(
            [FromQuery] string provincia = null,
            [FromQuery] string tipoCocina = null,
            [FromQuery] decimal? precioMin = null,
            [FromQuery] decimal? precioMax = null,
            [FromQuery] decimal? calificacionMin = null)
        {
            var query = _context.restaurante
                .Include(r => r.Lugar)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(provincia))
            {
                query = query.Where(r => r.Lugar.provincia == provincia);
            }

            if (!string.IsNullOrEmpty(tipoCocina))
            {
                query = query.Where(r => r.tipo_cocina_restaurante.Contains(tipoCocina));
            }

            if (precioMin.HasValue)
            {
                query = query.Where(r => r.precio_promedio >= precioMin.Value);
            }

            if (precioMax.HasValue)
            {
                query = query.Where(r => r.precio_promedio <= precioMax.Value);
            }

            if (calificacionMin.HasValue)
            {
                query = query.Where(r => r.calificacion_promedio >= calificacionMin.Value);
            }

            var restaurantes = await query
                .Select(r => new
                {
                    r.id_restaurante,
                    r.nombre_restaurante,
                    r.descripcion_restaurante,
                    r.tipo_cocina_restaurante,
                    r.precio_promedio,
                    r.calificacion_promedio,
                    r.horario_apertura,
                    r.horario_cierre,
                    Lugar = new
                    {
                        r.Lugar.id_Lugar,
                        r.Lugar.mombre,
                        r.Lugar.provincia
                    },
                    ImagenPrincipal = _context.imagen
                        .Where(i => i.tipo_entidad == "restaurante" &&
                                   i.id_entidad == r.id_restaurante &&
                                   i.es_principal)
                        .Select(i => new { i.url_imagen, i.descripcion_imagen })
                        .FirstOrDefault(),
                    TotalCalificaciones = _context.calificacion
                        .Count(c => c.tipo_entidad == "restaurante" && c.id_entidad == r.id_restaurante)
                })
                .OrderBy(r => r.nombre_restaurante)
                .ToListAsync();

            return Ok(new
            {
                Total_Restaurantes = restaurantes.Count,
                Filtros_Aplicados = new { provincia, tipoCocina, precioMin, precioMax, calificacionMin },
                Restaurantes = restaurantes
            });
        }

        // GET: api/Restaurantes/5 - Restaurante completo con todos los detalles
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetRestaurante(int id)
        {
            var restaurante = await _context.restaurante
                .Include(r => r.Lugar)
                .FirstOrDefaultAsync(r => r.id_restaurante == id);

            if (restaurante == null)
            {
                return NotFound($"Restaurante con ID {id} no encontrado");
            }

            // Obtener todas las imágenes
            var imagenes = await _context.imagen
                .Where(i => i.tipo_entidad == "restaurante" && i.id_entidad == id)
                .OrderByDescending(i => i.es_principal)
                .Select(i => new
                {
                    i.id_imagen,
                    i.url_imagen,
                    i.descripcion_imagen,
                    i.es_principal,
                    i.fecha_subida
                })
                .ToListAsync();

            // Obtener calificaciones
            var calificaciones = await _context.calificacion
                .Where(c => c.tipo_entidad == "restaurante" && c.id_entidad == id)
                .Include(c => c.usuario)
                .Select(c => new
                {
                    c.id_calificacion,
                    c.puntuacion,
                    c.comentario,
                    c.fecha_calificacion,
                    Usuario = new { c.usuario.nombre_usuario }
                })
                .OrderByDescending(c => c.fecha_calificacion)
                .Take(10)
                .ToListAsync();

            // Calcular estadísticas
            var promedioCalificaciones = calificaciones.Any()
                ? calificaciones.Average(c => c.puntuacion)
                : 0;

            // Obtener horario formateado
            var horario = restaurante.horario_apertura.HasValue && restaurante.horario_cierre.HasValue
                ? $"{restaurante.horario_apertura:hh\\:mm} - {restaurante.horario_cierre:hh\\:mm}"
                : "Horario no especificado";

            return Ok(new
            {
                Restaurante = new
                {
                    restaurante.id_restaurante,
                    restaurante.nombre_restaurante,
                    restaurante.descripcion_restaurante,
                    restaurante.tipo_cocina_restaurante,
                    restaurante.precio_promedio,
                    restaurante.calificacion_promedio,
                    Horario = horario,
                    Horario_Apertura = restaurante.horario_apertura,
                    Horario_Cierre = restaurante.horario_cierre,
                    Lugar = new
                    {
                        restaurante.Lugar.id_Lugar,
                        restaurante.Lugar.mombre,
                        restaurante.Lugar.descripcion,
                        restaurante.Lugar.provincia
                    }
                },
                Imagenes = imagenes,
                Calificaciones = new
                {
                    Lista = calificaciones,
                    Promedio = Math.Round(promedioCalificaciones, 1),
                    Total = calificaciones.Count,
                    Distribucion = Enumerable.Range(1, 5)
                        .Select(i => new
                        {
                            Estrellas = i,
                            Cantidad = calificaciones.Count(c => c.puntuacion == i)
                        })
                        .ToList()
                }
            });
        }

        // POST: api/Restaurantes
        [HttpPost]
        public async Task<ActionResult<Restaurante>> PostRestaurante(Restaurante restaurante)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(restaurante.nombre_restaurante))
            {
                return BadRequest("El nombre del restaurante es requerido");
            }

            if (string.IsNullOrWhiteSpace(restaurante.tipo_cocina_restaurante))
            {
                return BadRequest("El tipo de cocina es requerido");
            }

            // Validar que el lugar existe
            var lugarExiste = await _context.lugar.AnyAsync(l => l.id_Lugar == restaurante.id_lugar);
            if (!lugarExiste)
            {
                return BadRequest("El lugar especificado no existe");
            }

            // Validar horarios si están presentes
            if (restaurante.horario_apertura.HasValue && restaurante.horario_cierre.HasValue)
            {
                if (restaurante.horario_apertura >= restaurante.horario_cierre)
                {
                    return BadRequest("El horario de apertura debe ser anterior al de cierre");
                }
            }

            // Establecer valores por defecto
            restaurante.calificacion_promedio ??= 0;
            restaurante.precio_promedio ??= 0;

            _context.restaurante.Add(restaurante);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRestaurante",
                new { id = restaurante.id_restaurante },
                new
                {
                    Mensaje = "Restaurante creado exitosamente",
                    Restaurante = restaurante
                });
        }

        // PUT: api/Restaurantes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRestaurante(int id, Restaurante restaurante)
        {
            if (id != restaurante.id_restaurante)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del restaurante");
            }

            var existente = await _context.restaurante.FindAsync(id);
            if (existente == null)
            {
                return NotFound($"Restaurante con ID {id} no encontrado");
            }

            // Validar horarios
            if (restaurante.horario_apertura.HasValue && restaurante.horario_cierre.HasValue)
            {
                if (restaurante.horario_apertura >= restaurante.horario_cierre)
                {
                    return BadRequest("El horario de apertura debe ser anterior al de cierre");
                }
            }

            // Actualizar propiedades
            existente.nombre_restaurante = restaurante.nombre_restaurante;
            existente.descripcion_restaurante = restaurante.descripcion_restaurante;
            existente.tipo_cocina_restaurante = restaurante.tipo_cocina_restaurante;
            existente.precio_promedio = restaurante.precio_promedio;
            existente.horario_apertura = restaurante.horario_apertura;
            existente.horario_cierre = restaurante.horario_cierre;

            // Solo actualizar calificación si viene (podría ser un campo calculado)
            if (restaurante.calificacion_promedio.HasValue)
            {
                existente.calificacion_promedio = restaurante.calificacion_promedio;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    Mensaje = "Restaurante actualizado exitosamente",
                    Restaurante = existente
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestauranteExists(id))
                {
                    return NotFound("El restaurante ya no existe");
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el restaurante: {ex.Message}");
            }
        }

        // DELETE: api/Restaurantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurante(int id)
        {
            var restaurante = await _context.restaurante
                .Include(r => r.Lugar)
                .FirstOrDefaultAsync(r => r.id_restaurante == id);

            if (restaurante == null)
            {
                return NotFound($"Restaurante con ID {id} no encontrado");
            }

            // Verificar si tiene calificaciones asociadas
            var tieneCalificaciones = await _context.calificacion
                .AnyAsync(c => c.tipo_entidad == "restaurante" && c.id_entidad == id);

            if (tieneCalificaciones)
            {
                return BadRequest(new
                {
                    Mensaje = "No se puede eliminar el restaurante porque tiene calificaciones asociadas",
                    Accion_Recomendada = "Considere archivar en lugar de eliminar"
                });
            }

            // Eliminar imágenes asociadas (opcional)
            var imagenes = await _context.imagen
                .Where(i => i.tipo_entidad == "restaurante" && i.id_entidad == id)
                .ToListAsync();

            if (imagenes.Any())
            {
                _context.imagen.RemoveRange(imagenes);
            }

            _context.restaurante.Remove(restaurante);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Restaurante eliminado exitosamente",
                Restaurante_Eliminado = new
                {
                    restaurante.id_restaurante,
                    restaurante.nombre_restaurante,
                    Lugar = restaurante.Lugar.mombre
                },
                Imagenes_Eliminadas = imagenes.Count
            });
        }

        // ==================== ENDPOINTS ESPECÍFICOS ====================

        // GET: api/Restaurantes/por-lugar/5
        [HttpGet("por-lugar/{idLugar}")]
        public async Task<ActionResult<object>> GetRestaurantesPorLugar(int idLugar)
        {
            var lugar = await _context.lugar.FindAsync(idLugar);
            if (lugar == null)
            {
                return NotFound($"Lugar con ID {idLugar} no encontrado");
            }

            var restaurantes = await _context.restaurante
                .Where(r => r.id_lugar == idLugar)
                .Select(r => new
                {
                    r.id_restaurante,
                    r.nombre_restaurante,
                    r.tipo_cocina_restaurante,
                    r.precio_promedio,
                    r.calificacion_promedio,
                    Horario = r.horario_apertura.HasValue && r.horario_cierre.HasValue
                        ? $"{r.horario_apertura:hh\\:mm} - {r.horario_cierre:hh\\:mm}"
                        : "No especificado",
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "restaurante" &&
                                   i.id_entidad == r.id_restaurante &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault(),
                    TotalCalificaciones = _context.calificacion
                        .Count(c => c.tipo_entidad == "restaurante" && c.id_entidad == r.id_restaurante)
                })
                .OrderBy(r => r.nombre_restaurante)
                .ToListAsync();

            return Ok(new
            {
                Lugar = new { lugar.id_Lugar, lugar.mombre, lugar.provincia},
                Total_Restaurantes = restaurantes.Count,
                Restaurantes = restaurantes
            });
        }

        // GET: api/Restaurantes/buscar?q=pizza
        [HttpGet("buscar")]
        public async Task<ActionResult<object>> BuscarRestaurantes([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest("Término de búsqueda requerido");
            }

            var restaurantes = await _context.restaurante
                .Include(r => r.Lugar)
                .Where(r => r.nombre_restaurante.Contains(q) ||
                           r.descripcion_restaurante.Contains(q) ||
                           r.tipo_cocina_restaurante.Contains(q) ||
                           r.Lugar.mombre.Contains(q))
                .Select(r => new
                {
                    r.id_restaurante,
                    r.nombre_restaurante,
                    r.tipo_cocina_restaurante,
                    r.precio_promedio,
                    r.calificacion_promedio,
                    Lugar = r.Lugar.mombre,
                    Provincia = r.Lugar.provincia,
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "restaurante" &&
                                   i.id_entidad == r.id_restaurante &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .Take(15)
                .ToListAsync();

            return Ok(new
            {
                Termino_Busqueda = q,
                Resultados_Encontrados = restaurantes.Count,
                Restaurantes = restaurantes
            });
        }

        // GET: api/Restaurantes/tipo-cocina/italiana
        [HttpGet("tipo-cocina/{tipoCocina}")]
        public async Task<ActionResult<object>> GetRestaurantesPorTipoCocina(string tipoCocina)
        {
            var restaurantes = await _context.restaurante
                .Include(r => r.Lugar)
                .Where(r => r.tipo_cocina_restaurante.Contains(tipoCocina))
                .Select(r => new
                {
                    r.id_restaurante,
                    r.nombre_restaurante,
                    r.tipo_cocina_restaurante,
                    r.precio_promedio,
                    r.calificacion_promedio,
                    Lugar = new
                    {
                        r.Lugar.mombre,
                        r.Lugar.provincia
                    },
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "restaurante" &&
                                   i.id_entidad == r.id_restaurante &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .OrderByDescending(r => r.calificacion_promedio)
                .ToListAsync();

            if (!restaurantes.Any())
            {
                return NotFound($"No se encontraron restaurantes de cocina '{tipoCocina}'");
            }

            return Ok(new
            {
                Tipo_Cocina = tipoCocina,
                Total_Restaurantes = restaurantes.Count,
                Restaurantes = restaurantes
            });
        }

        


        // GET: api/Restaurantes/mejor-calificados?limit=10
        [HttpGet("mejor-calificados")]
        public async Task<ActionResult<object>> GetRestaurantesMejorCalificados([FromQuery] int limit = 10)
        {
            var restaurantes = await _context.restaurante
                .Include(r => r.Lugar)
                .Where(r => r.calificacion_promedio > 0)
                .OrderByDescending(r => r.calificacion_promedio)
                .Take(limit)
                .Select(r => new
                {
                    r.id_restaurante,
                    r.nombre_restaurante,
                    r.tipo_cocina_restaurante,
                    r.precio_promedio,
                    r.calificacion_promedio,
                    Lugar = r.Lugar.mombre,
                    Total_Resenas = _context.calificacion
                        .Count(c => c.tipo_entidad == "restaurante" && c.id_entidad == r.id_restaurante),
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "restaurante" &&
                                   i.id_entidad == r.id_restaurante &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(new
            {
                Total = restaurantes.Count,
                Restaurantes_Mejor_Calificados = restaurantes
            });
        }

        // GET: api/Restaurantes/rango-precio?min=10&max=50
        [HttpGet("rango-precio")]
        public async Task<ActionResult<object>> GetRestaurantesPorRangoPrecio(
            [FromQuery] decimal min = 0,
            [FromQuery] decimal max = 100)
        {
            if (min > max)
            {
                return BadRequest("El precio mínimo no puede ser mayor al máximo");
            }

            var restaurantes = await _context.restaurante
                .Include(r => r.Lugar)
                .Where(r => r.precio_promedio >= min && r.precio_promedio <= max)
                .OrderBy(r => r.precio_promedio)
                .Select(r => new
                {
                    r.id_restaurante,
                    r.nombre_restaurante,
                    r.tipo_cocina_restaurante,
                    r.precio_promedio,
                    r.calificacion_promedio,
                    Nivel_Precio = r.precio_promedio < 20 ? "Económico" :
                                  r.precio_promedio < 50 ? "Moderado" : "Alto",
                    Lugar = r.Lugar.mombre,
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "restaurante" &&
                                   i.id_entidad == r.id_restaurante &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(new
            {
                Rango = $"{min:C} - {max:C}",
                Total_Restaurantes = restaurantes.Count,
                Por_Nivel_Precio = restaurantes
                    .GroupBy(r => r.Nivel_Precio)
                    .Select(g => new
                    {
                        Nivel = g.Key,
                        Cantidad = g.Count(),
                        Precio_Promedio = g.Average(r => r.precio_promedio)
                    })
                    .ToList(),
                Restaurantes = restaurantes
            });
        }

        private bool RestauranteExists(int id)
        {
            return _context.restaurante.Any(e => e.id_restaurante == id);
        }
    }
}