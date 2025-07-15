using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SingularExpress.Models.Models
{
    [Table("PasswordResetTokens")] 
    public class PasswordResetToken
    {
        
        public int Id { get; set; }  

        public string Email { get; set; } = string.Empty;

        public string Otp { get; set; } = string.Empty;  

        public DateTime ExpiresAt { get; set; }  
    }
}