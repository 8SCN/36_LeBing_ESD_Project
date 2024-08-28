using LeBing_ESD_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace LeBing_ESD_Project.Data
{
    public class BookingDbContext : IdentityDbContext<IdentityUser>
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options)
        : base(options)
        {
        }
        public DbSet<Booking>? Bookings { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
