using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vaan.CMS.API.Models
{
    public class UserDto
    { 
        [Required(ErrorMessage = "This field is required!")]
        [MaxLength(50)]
        public string Password { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        [MaxLength(50)]
        public string Email { get; set; }
    }
}
