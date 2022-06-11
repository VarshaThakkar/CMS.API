using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vaan.CMS.API.Entities;

namespace Vaan.CMS.API.Data
{
    public class CMSDbContext : DbContext
    {
        public CMSDbContext(DbContextOptions<CMSDbContext> options)
            : base(options)
        {

        }
       // public DbSet<User> Users { get; set; }
        public DbSet<UserEntity> CMSUsers { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
