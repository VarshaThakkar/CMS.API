using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vaan.CMS.API.Models.Users
{
    public class UserRegister
    {
        [Required(ErrorMessage = "This field is required!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        public bool Status { get; set; }
       // [Required]
        public string Password { get; set; }
    }
}
