using System.ComponentModel.DataAnnotations;

namespace SingularExpress.Dto
{
    public class UpdatePasswordDto
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}