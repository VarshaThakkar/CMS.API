using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vaan.CMS.API.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public byte[] HasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Role { get; set; }

    }
}
