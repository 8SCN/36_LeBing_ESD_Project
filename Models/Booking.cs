using System.ComponentModel.DataAnnotations;

namespace LeBing_ESD_Project.Models
{
    public class UserRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
    public class Response
    {
        public string? Status { get; set; }

        public string? Message { get; set; }
    }
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required.")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }
    public class RegisterAdminModel
    {
        [Required(ErrorMessage = "User Name is required.")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
    public class AuditLog
    {
        public int Id { get; set; }
        public string? Action { get; set; }
        public string? UserName { get; set; }
        public string? Time { get; set; }
    }

    public class Booking
    {
        [Key]

        public int BookingID { get; set; }
        public string? FacilityDescription { get; set; }

        public string? BookingDateFrom { get; set; }

        public string? BookingDateTo { get; set; }

        public string? BookedBy { get; set; }

        public string? BookingStatus { get; set; }
    }
}
