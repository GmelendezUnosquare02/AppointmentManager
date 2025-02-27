using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentDbContext _context;

        public AppointmentsController(AppointmentDbContext context)
        {
            _context = context;
        }

        // GET: api/appointments/user/{username}
        [HttpGet("user/{username}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetUserAppointments(string username)
        {
            return await _context.Appointments
                .Where(a => a.User == username)
                .OrderBy(a => a.Date)
                .ToListAsync();
        }

        // POST: api/appointments

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            if (appointment == null)
            {
                return BadRequest(new { message = "Invalid request: Appointment object is required." });
            }

            if (appointment.Date == default)
            {
                return BadRequest(new { message = "Invalid date format. Please send a valid DateTime format (ISO 8601)." });
            }

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserAppointments), new { username = appointment.User }, appointment);
        }

        // PUT: api/appointments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, Appointment appointment)
        {
            if (id != appointment.Id) return BadRequest();
            var existingAppointment = await _context.Appointments.FindAsync(id);

            if (existingAppointment == null) return NotFound();
            if (existingAppointment.IsApproved || existingAppointment.IsCanceled) return BadRequest("Cannot modify approved or canceled appointment");

            existingAppointment.Title = appointment.Title;
            existingAppointment.Date = appointment.Date;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/appointments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();
            if (appointment.IsApproved || appointment.IsCanceled) return BadRequest("Cannot delete approved or canceled appointment");

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // MANAGER ACTIONS

        // GET: api/appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
        {
            return await _context.Appointments
                .OrderBy(a => a.Date)
                .ToListAsync();
        }

        // PUT: api/appointments/approve/{id}
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();
            if (appointment.IsCanceled) return BadRequest("Cannot approve a canceled appointment");

            appointment.IsApproved = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/appointments/cancel/{id}
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.IsCanceled = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/appointments/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> ManagerDeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();
            if (!appointment.IsCanceled) return BadRequest("Cannot delete an appointment that is not canceled");

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
