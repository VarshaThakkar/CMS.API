using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vaan.CMS.API.Models.Users
{
    public class UpdateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public string Password { get; set; }
    }
}
