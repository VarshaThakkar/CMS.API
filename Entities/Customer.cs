using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vaan.CMS.API.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string ObjectId { get; set; }
        public string Email { get; set; }
    }
}
