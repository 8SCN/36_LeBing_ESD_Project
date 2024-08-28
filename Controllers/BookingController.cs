using LeBing_ESD_Project.Data;
using LeBing_ESD_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LeBing_ESD_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public BookingController(BookingDbContext context)
        {
            _context = context;
        }

        // GET: api/Booking
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Only return bookings created by the current user or all if the user is an Admin
            var userId = User.Identity.Name;
            var isAdmin = User.IsInRole("Admin");

            var bookings = isAdmin
                ? await _context.Bookings.ToListAsync()
                : await _context.Bookings.Where(b => b.BookedBy == userId).ToListAsync();

            return Ok(bookings);
        }

        // GET: api/Booking/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.Identity.Name;
            var isAdmin = User.IsInRole("Admin");

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            // Check if the user is allowed to view this booking
            if (!isAdmin && booking.BookedBy != userId)
            {
                return Forbid();
            }

            return Ok(booking);
        }


        // PUT: api/Booking/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Booking updatedBooking)
        {
            var userId = User.Identity.Name;
            var isAdmin = User.IsInRole("Admin");

            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            // Check if the user is allowed to update this booking
            if (!isAdmin && existingBooking.BookedBy != userId)
            {
                return Forbid();
            }

            // Update the fields
            existingBooking.FacilityDescription = updatedBooking.FacilityDescription;
            existingBooking.BookingDateFrom = updatedBooking.BookingDateFrom;
            existingBooking.BookingDateTo = updatedBooking.BookingDateTo;
            existingBooking.BookingStatus = updatedBooking.BookingStatus;

            _context.Bookings.Update(existingBooking);
            await _context.SaveChangesAsync();

            // Audit Log for Update
            var log = new AuditLog
            {
                Action = "Update",
                UserName = userId,
                Time = DateTime.UtcNow.ToString(),
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/Booking/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Identity.Name;
            var isAdmin = User.IsInRole("Admin");

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            // Check if the user is allowed to delete this booking
            if (!isAdmin && booking.BookedBy != userId)
            {
                return Forbid();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            // Audit Log for Delete
            var log = new AuditLog
            {
                Action = "Delete",
                UserName = userId,
                Time = DateTime.UtcNow.ToString(),
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Booking
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.BookedBy = User.Identity.Name;
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Audit Log
                var log = new AuditLog
                {
                    Action = "Create",
                    UserName = User.Identity.Name,
                    Time = DateTime.UtcNow.ToString("o"),
                };
                _context.AuditLogs.Add(log);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = booking.BookingID }, booking);
            }
            return BadRequest(ModelState);
        }

        // GET: api/Booking/AuditLogs
        [Authorize(Roles = "Admin")]
        [HttpGet("AuditLogs")]
        public async Task<IActionResult> GetAuditLogs()
        {
            var logs = await _context.AuditLogs.OrderByDescending(log => log.Time).ToListAsync();
            return Ok(logs);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("AuditLogs")]
        public async Task<IActionResult> ClearAuditLogs()
        {
            _context.AuditLogs.RemoveRange(_context.AuditLogs);
            await _context.SaveChangesAsync();
            return NoContent(); // Return 204 No Content
        }

    }
}
