using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SingularExpress.Dto
{
    public class UserReadDto
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}