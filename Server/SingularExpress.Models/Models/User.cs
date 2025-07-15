using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SingularExpress.Models.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "User name is required")]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password hash is required")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedOn { get; set; }

        // 🔥 Required for login tracking
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }

        // ✅ Optional for roles or admin control
        public string Role { get; set; } = "User";

        // ✅ Optional for password reset
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }
}
