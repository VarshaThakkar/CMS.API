using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vaan.CMS.API.Models.Google
{
    public class GoogleRegister
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccessToken { get; set; }
    }
}
