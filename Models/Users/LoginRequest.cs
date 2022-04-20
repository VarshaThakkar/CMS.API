using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vaan.CMS.API.Models.Users
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; }
       // [Required]
        public string Password { get; set; }
    }
}
