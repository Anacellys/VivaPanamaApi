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

        // ==================== CRUD BÁSICO ====================

        // GET: api/Actividades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetActividades(
            [FromQuery] string provincia = null,
            [FromQuery] string dificultad = null,
            [FromQuery] decimal? costoMax = null,
            [FromQuery] int? duracionMax = null)
        {
            var query = _context.actividad_lugar
                .Include(a => a.Lugar)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(provincia))
            {
                query = query.Where(a => a.Lugar.provincia == provincia);
            }

            if (!string.IsNullOrEmpty(dificultad))
            {
                query = query.Where(a => a.dificultad_actividad == dificultad);
            }

            if (costoMax.HasValue)
            {
                query = query.Where(a => a.costo_actividad <= costoMax.Value);
            }

            if (duracionMax.HasValue)
            {
                query = query.Where(a => a.duracion_estimada <= duracionMax.Value);
            }

            var actividades = await query
                .Select(a => new
                {
                    a.id_actividad,
                    a.nombre_actividad,
                    a.descripcion_actividad,
                    a.costo_actividad,
                    a.duracion_estimada,
                    a.horario_apertura,
                    a.horario_cierre,
                    a.dificultad_actividad,
                    Lugar = new
                    {
                        a.Lugar.id_Lugar,
                        a.Lugar.mombre,
                        a.Lugar.provincia
                    },
                    ImagenPrincipal = _context.imagen
                        .Where(i => i.tipo_entidad == "actividad" &&
                                   i.id_entidad == a.id_actividad &&
                                   i.es_principal)
                        .Select(i => new { i.url_imagen, i.descripcion_imagen })
                        .FirstOrDefault(),
                    Horario_Formateado = a.horario_apertura.HasValue && a.horario_cierre.HasValue
                        ? $"{a.horario_apertura.Value:hh\\:mm} - {a.horario_cierre.Value:hh\\:mm}"
                        : "Horario no especificado"
                })
                .OrderBy(a => a.nombre_actividad)
                .ToListAsync();

            return Ok(new
            {
                Total_Actividades = actividades.Count,
                Filtros_Aplicados = new { provincia, dificultad, costoMax, duracionMax },
                Actividades = actividades
            });
        }

        // GET: api/Actividades/5 - Actividad completa con todos los detalles
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetActividad(int id)
        {
            var actividad = await _context.actividad_lugar
                .Include(a => a.Lugar)
                .FirstOrDefaultAsync(a => a.id_actividad == id);

            if (actividad == null)
            {
                return NotFound($"Actividad con ID {id} no encontrada");
            }

            // Obtener todas las imágenes
            var imagenes = await _context.imagen
                .Where(i => i.tipo_entidad == "actividad" && i.id_entidad == id)
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
                .Where(c => c.tipo_entidad == "actividad" && c.id_entidad == id)
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
            var horario = actividad.horario_apertura.HasValue && actividad.horario_cierre.HasValue
                ? $"{actividad.horario_apertura.Value:hh\\:mm} - {actividad.horario_cierre.Value:hh\\:mm}"
                : "Horario no especificado";

            // Verificar si está disponible ahora
            var ahora = DateTime.Now.TimeOfDay;
            var estaDisponibleAhora = actividad.horario_apertura.HasValue &&
                                     actividad.horario_cierre.HasValue &&
                                     actividad.horario_apertura <= ahora &&
                                     actividad.horario_cierre >= ahora;

            return Ok(new
            {
                Actividad = new
                {
                    actividad.id_actividad,
                    actividad.nombre_actividad,
                    actividad.descripcion_actividad,
                    actividad.costo_actividad,
                    actividad.duracion_estimada,
                    actividad.horario_apertura,
                    actividad.horario_cierre,
                    actividad.dificultad_actividad,
                    actividad.equipo_requerido,
                    actividad.restricciones,
                    Horario = horario,
                    Esta_Disponible_Ahora = estaDisponibleAhora,
                    Lugar = new
                    {
                        actividad.Lugar.id_Lugar,
                        actividad.Lugar.mombre,
                        actividad.Lugar.descripcion,
                        actividad.Lugar.provincia,
                        actividad.Lugar.tipo_lugar
                    }
                },
                Imagenes = imagenes,
                Calificaciones = new
                {
                    Lista = calificaciones,
                    Promedio = Math.Round(promedioCalificaciones, 1),
                    Total = calificaciones.Count
                },
                Informacion_Practica = new
                {
                    Duracion_Horas = actividad.duracion_estimada.HasValue
                        ? $"{actividad.duracion_estimada / 60}h {actividad.duracion_estimada % 60}min"
                        : "No especificada",
                    Recomendaciones = !string.IsNullOrEmpty(actividad.equipo_requerido)
                        ? actividad.equipo_requerido.Split(',').Select(e => e.Trim()).ToList()
                        : new List<string>(),
                    Restricciones_Importantes = !string.IsNullOrEmpty(actividad.restricciones)
                        ? actividad.restricciones.Split(',').Select(r => r.Trim()).ToList()
                        : new List<string>()
                }
            });
        }

        // POST: api/Actividades
        [HttpPost]
        public async Task<ActionResult<ActividadLugar>> PostActividad(ActividadLugar actividad)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(actividad.nombre_actividad))
            {
                return BadRequest("El nombre de la actividad es requerido");
            }

            // Validar que el lugar existe
            var lugarExiste = await _context.lugar.AnyAsync(l => l.id_Lugar == actividad.id_lugar);
            if (!lugarExiste)
            {
                return BadRequest("El lugar especificado no existe");
            }

            // Validar horarios si están presentes
            if (actividad.horario_apertura.HasValue && actividad.horario_cierre.HasValue)
            {
                if (actividad.horario_apertura >= actividad.horario_cierre)
                {
                    return BadRequest("El horario de apertura debe ser anterior al de cierre");
                }
            }

            // Validar duración
            if (actividad.duracion_estimada.HasValue && actividad.duracion_estimada <= 0)
            {
                return BadRequest("La duración estimada debe ser mayor a 0");
            }

            // Validar costo
            if (actividad.costo_actividad.HasValue && actividad.costo_actividad < 0)
            {
                return BadRequest("El costo no puede ser negativo");
            }

            _context.actividad_lugar.Add(actividad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActividad",
                new { id = actividad.id_actividad },
                new
                {
                    Mensaje = "Actividad creada exitosamente",
                    Actividad = actividad
                });
        }

        // PUT: api/Actividades/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActividad(int id, ActividadLugar actividad)
        {
            if (id != actividad.id_actividad)
            {
                return BadRequest("El ID de la ruta no coincide con el ID de la actividad");
            }

            var existente = await _context.actividad_lugar.FindAsync(id);
            if (existente == null)
            {
                return NotFound($"Actividad con ID {id} no encontrada");
            }

            // Validar horarios
            if (actividad.horario_apertura.HasValue && actividad.horario_cierre.HasValue)
            {
                if (actividad.horario_apertura >= actividad.horario_cierre)
                {
                    return BadRequest("El horario de apertura debe ser anterior al de cierre");
                }
            }

            // Validar duración
            if (actividad.duracion_estimada.HasValue && actividad.duracion_estimada <= 0)
            {
                return BadRequest("La duración estimada debe ser mayor a 0");
            }

            // Validar costo
            if (actividad.costo_actividad.HasValue && actividad.costo_actividad < 0)
            {
                return BadRequest("El costo no puede ser negativo");
            }

            // Actualizar propiedades
            existente.nombre_actividad = actividad.nombre_actividad;
            existente.descripcion_actividad = actividad.descripcion_actividad;
            existente.costo_actividad = actividad.costo_actividad;
            existente.duracion_estimada = actividad.duracion_estimada;
            existente.horario_apertura = actividad.horario_apertura;
            existente.horario_cierre = actividad.horario_cierre;
            existente.dificultad_actividad = actividad.dificultad_actividad;
            existente.equipo_requerido = actividad.equipo_requerido;
            existente.restricciones = actividad.restricciones;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    Mensaje = "Actividad actualizada exitosamente",
                    Actividad = existente
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActividadExists(id))
                {
                    return NotFound("La actividad ya no existe");
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la actividad: {ex.Message}");
            }
        }

        // DELETE: api/Actividades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActividad(int id)
        {
            var actividad = await _context.actividad_lugar
                .Include(a => a.Lugar)
                .FirstOrDefaultAsync(a => a.id_actividad == id);

            if (actividad == null)
            {
                return NotFound($"Actividad con ID {id} no encontrada");
            }

            // Verificar si está en itinerarios
            var enItinerarios = await _context.actividad_itinerario
                .AnyAsync(ai => ai.id_actividad == id);

            if (enItinerarios)
            {
                return BadRequest(new
                {
                    Mensaje = "No se puede eliminar la actividad porque está incluida en itinerarios",
                    Accion_Recomendada = "Elimine primero las referencias en itinerarios"
                });
            }

            // Eliminar imágenes asociadas
            var imagenes = await _context.imagen
                .Where(i => i.tipo_entidad == "actividad" && i.id_entidad == id)
                .ToListAsync();

            if (imagenes.Any())
            {
                _context.imagen.RemoveRange(imagenes);
            }

            // Eliminar calificaciones asociadas
            var calificaciones = await _context.calificacion
                .Where(c => c.tipo_entidad == "actividad" && c.id_entidad == id)
                .ToListAsync();

            if (calificaciones.Any())
            {
                _context.calificacion.RemoveRange(calificaciones);
            }

            _context.actividad_lugar.Remove(actividad);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Actividad eliminada exitosamente",
                Actividad_Eliminada = new
                {
                    actividad.id_actividad,
                    actividad.nombre_actividad,
                    Lugar = actividad.Lugar.mombre
                },
                Imagenes_Eliminadas = imagenes.Count,
                Calificaciones_Eliminadas = calificaciones.Count
            });
        }

        // ==================== ENDPOINTS ESPECÍFICOS ====================

        // GET: api/Actividades/por-lugar/5
        [HttpGet("por-lugar/{idLugar}")]
        public async Task<ActionResult<object>> GetActividadesPorLugar(int idLugar)
        {
            var lugar = await _context.lugar.FindAsync(idLugar);
            if (lugar == null)
            {
                return NotFound($"Lugar con ID {idLugar} no encontrado");
            }

            var actividades = await _context.actividad_lugar
                .Where(a => a.id_lugar == idLugar)
                .Select(a => new
                {
                    a.id_actividad,
                    a.nombre_actividad,
                    a.descripcion_actividad,
                    a.costo_actividad,
                    a.duracion_estimada,
                    a.dificultad_actividad,
                    Horario = a.horario_apertura.HasValue && a.horario_cierre.HasValue
                        ? $"{a.horario_apertura.Value:hh\\:mm} - {a.horario_cierre.Value:hh\\:mm}"
                        : "No especificado",
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "actividad" &&
                                   i.id_entidad == a.id_actividad &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .OrderBy(a => a.costo_actividad)
                .ToListAsync();

            return Ok(new
            {
                Lugar = new { lugar.id_Lugar, lugar.mombre, lugar.provincia },
                Total_Actividades = actividades.Count,
                Actividades = actividades
            });
        }

        // GET: api/Actividades/por-dificultad/facil
        [HttpGet("por-dificultad/{dificultad}")]
        public async Task<ActionResult<object>> GetActividadesPorDificultad(string dificultad)
        {
            var actividades = await _context.actividad_lugar
                .Include(a => a.Lugar)
                .Where(a => a.dificultad_actividad == dificultad)
                .Select(a => new
                {
                    a.id_actividad,
                    a.nombre_actividad,
                    a.costo_actividad,
                    a.duracion_estimada,
                    a.dificultad_actividad,
                    Lugar = a.Lugar.mombre,
                    Provincia = a.Lugar.provincia,
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "actividad" &&
                                   i.id_entidad == a.id_actividad &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .OrderBy(a => a.costo_actividad)
                .ToListAsync();

            if (!actividades.Any())
            {
                return NotFound($"No se encontraron actividades con dificultad '{dificultad}'");
            }

            return Ok(new
            {
                Dificultad = dificultad,
                Total_Actividades = actividades.Count,
                Actividades = actividades
            });
        }

        // GET: api/Actividades/por-costo?max=50
        [HttpGet("por-costo")]
        public async Task<ActionResult<object>> GetActividadesPorCosto([FromQuery] decimal max = 100)
        {
            var actividades = await _context.actividad_lugar
                .Include(a => a.Lugar)
                .Where(a => a.costo_actividad <= max)
                .Select(a => new
                {
                    a.id_actividad,
                    a.nombre_actividad,
                    a.costo_actividad,
                    a.duracion_estimada,
                    a.dificultad_actividad,
                    Lugar = a.Lugar.mombre,
                    Nivel_Precio = a.costo_actividad < 20 ? "Económico" :
                                 a.costo_actividad < 50 ? "Moderado" : "Premium",
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "actividad" &&
                                   i.id_entidad == a.id_actividad &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .OrderBy(a => a.costo_actividad)
                .ToListAsync();

            return Ok(new
            {
                Costo_Maximo = max,
                Total_Actividades = actividades.Count,
                Por_Nivel_Precio = actividades
                    .GroupBy(a => a.Nivel_Precio)
                    .Select(g => new
                    {
                        Nivel = g.Key,
                        Cantidad = g.Count(),
                        Costo_Promedio = g.Average(a => a.costo_actividad)
                    })
                    .ToList(),
                Actividades = actividades
            });
        }

        // GET: api/Actividades/abiertas-ahora
        [HttpGet("abiertas-ahora")]
        public async Task<ActionResult<object>> GetActividadesAbiertasAhora()
        {
            var ahora = DateTime.Now.TimeOfDay;

            var actividades = await _context.actividad_lugar
                .Include(a => a.Lugar)
                .Where(a => a.horario_apertura.HasValue && a.horario_cierre.HasValue &&
                           a.horario_apertura <= ahora && a.horario_cierre >= ahora)
                .Select(a => new
                {
                    a.id_actividad,
                    a.nombre_actividad,
                    a.costo_actividad,
                    a.duracion_estimada,
                    Horario = $"{a.horario_apertura.Value:hh\\:mm} - {a.horario_cierre.Value:hh\\:mm}",
                    Tiempo_Restante = Math.Round((a.horario_cierre.Value - ahora).TotalHours, 1),
                    Lugar = a.Lugar.mombre,
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "actividad" &&
                                   i.id_entidad == a.id_actividad &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .OrderBy(a => a.Tiempo_Restante)
                .Take(10)
                .ToListAsync();

            return Ok(new
            {
                Hora_Actual = ahora.ToString(@"hh\:mm"),
                Total_Abiertas = actividades.Count,
                Actividades_Abiertas = actividades
            });
        }

        // GET: api/Actividades/duracion?max=120
        [HttpGet("duracion")]
        public async Task<ActionResult<object>> GetActividadesPorDuracion([FromQuery] int max = 240)
        {
            var actividades = await _context.actividad_lugar
                .Include(a => a.Lugar)
                .Where(a => a.duracion_estimada <= max)
                .Select(a => new
                {
                    a.id_actividad,
                    a.nombre_actividad,
                    a.costo_actividad,
                    a.duracion_estimada,
                    Duracion_Horas = a.duracion_estimada.HasValue
                        ? $"{a.duracion_estimada / 60}h {a.duracion_estimada % 60}min"
                        : "No especificada",
                    a.dificultad_actividad,
                    Lugar = a.Lugar.mombre,
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "actividad" &&
                                   i.id_entidad == a.id_actividad &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .OrderBy(a => a.duracion_estimada)
                .ToListAsync();

            return Ok(new
            {
                Duracion_Maxima_Minutos = max,
                Duracion_Maxima_Horas = $"{max / 60}h {max % 60}min",
                Total_Actividades = actividades.Count,
                Actividades = actividades
            });
        }

        // GET: api/Actividades/mejor-calificadas?limit=10
        [HttpGet("mejor-calificadas")]
        public async Task<ActionResult<object>> GetActividadesMejorCalificadas([FromQuery] int limit = 10)
        {
            var actividades = await _context.actividad_lugar
                .Include(a => a.Lugar)
                .Select(a => new
                {
                    a.id_actividad,
                    a.nombre_actividad,
                    a.costo_actividad,
                    a.duracion_estimada,
                    a.dificultad_actividad,
                    Lugar = a.Lugar.mombre,
                    Calificacion_Promedio = _context.calificacion
                        .Where(c => c.tipo_entidad == "actividad" && c.id_entidad == a.id_actividad)
                        .Average(c => (double?)c.puntuacion) ?? 0,
                    Total_Calificaciones = _context.calificacion
                        .Count(c => c.tipo_entidad == "actividad" && c.id_entidad == a.id_actividad),
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "actividad" &&
                                   i.id_entidad == a.id_actividad &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .Where(a => a.Calificacion_Promedio > 0)
                .OrderByDescending(a => a.Calificacion_Promedio)
                .ThenByDescending(a => a.Total_Calificaciones)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                Total = actividades.Count,
                Actividades_Mejor_Calificadas = actividades
            });
        }

        // GET: api/Actividades/buscar?q=senderismo
        [HttpGet("buscar")]
        public async Task<ActionResult<object>> BuscarActividades([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest("Término de búsqueda requerido");
            }

            var actividades = await _context.actividad_lugar
                .Include(a => a.Lugar)
                .Where(a => a.nombre_actividad.Contains(q) ||
                           a.descripcion_actividad.Contains(q) ||
                           a.dificultad_actividad.Contains(q) ||
                           a.equipo_requerido.Contains(q) ||
                           a.Lugar.mombre.Contains(q))
                .Select(a => new
                {
                    a.id_actividad,
                    a.nombre_actividad,
                    a.costo_actividad,
                    a.duracion_estimada,
                    a.dificultad_actividad,
                    Lugar = a.Lugar.mombre,
                    Provincia = a.Lugar.provincia,
                    Imagen = _context.imagen
                        .Where(i => i.tipo_entidad == "actividad" &&
                                   i.id_entidad == a.id_actividad &&
                                   i.es_principal)
                        .Select(i => i.url_imagen)
                        .FirstOrDefault()
                })
                .Take(15)
                .ToListAsync();

            return Ok(new
            {
                Termino_Busqueda = q,
                Resultados_Encontrados = actividades.Count,
                Actividades = actividades
            });
        }

        private bool ActividadExists(int id)
        {
            return _context.actividad_lugar.Any(e => e.id_actividad == id);
        }
    }
}