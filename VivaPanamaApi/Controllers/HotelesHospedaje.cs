using Microsoft.AspNetCore.Mvc;
using VivaPanamaApi.Contexts;
using VivaPanamaApi.Models;
using Microsoft.EntityFrameworkCore;
namespace VivaPanamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelesHospedaje : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;

        public HotelesHospedaje(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }
        [HttpGet]
        public async Task<ActionResult<List<HotelHospedaje>>> GetHoteles()
        {
            return await applicationDbContext.HotelHospedaje.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<HotelHospedaje>> GetHotel(int id)
        {
            if (id <= 0)
                return BadRequest("El id proporcionado no es válido.");

            var hotel = await applicationDbContext.HotelHospedaje.FindAsync(id);
            if (hotel == null)
                return NotFound("No se encontró el hotel con el id especificado.");

            return Ok(hotel);
        }

        [HttpPost]
        public async Task<ActionResult<HotelHospedaje>> CrearHotel(HotelHospedaje hotel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                applicationDbContext.HotelHospedaje.Add(hotel);
                await applicationDbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetHotel), new { id = hotel.id_hotel }, hotel);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al crear el hotel.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(int id, HotelHospedaje hotel)
        {
            if (id != hotel.id_hotel)
                return BadRequest("El id en la ruta no coincide con el id en el cuerpo.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await applicationDbContext.HotelHospedaje.FindAsync(id);
            if (existing == null)
                return NotFound("No se encontró el hotel con el id especificado.");

            existing.nombre = hotel.nombre;
            existing.id_lugar = hotel.id_lugar;
            existing.precio_noche = hotel.precio_noche;
            existing.servicios = hotel.servicios;

            try
            {
                await applicationDbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await applicationDbContext.HotelHospedaje.AnyAsync(h => h.id_hotel == id))
                    return NotFound("El hotel ya no existe.");
                throw;
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error al actualizar el hotel.");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if (id <= 0)
                return BadRequest("El id proporcionado no es válido.");

            var hotel = await applicationDbContext.HotelHospedaje.FindAsync(id);
            if (hotel == null)
                return NotFound("No se encontró el hotel con el id especificado.");

            try
            {
                applicationDbContext.HotelHospedaje.Remove(hotel);
                await applicationDbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error al eliminar el hotel.");
            }
        }

        [HttpGet("por-lugar/{idLugar}")]
        public async Task<ActionResult<List<HotelHospedaje>>> GetHotelesPorLugar(int idLugar)
        {
            return await applicationDbContext.HotelHospedaje
                .Where(h => h.id_lugar == idLugar)
                .ToListAsync();
        }

    }
}
