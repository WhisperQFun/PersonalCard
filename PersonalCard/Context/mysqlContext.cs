using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalCard.Blockchain;
using PersonalCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PersonalCard.Context
{
    public class mysqlContext : IdentityDbContext<ApplicationUser>
    {
        public mysqlContext(DbContextOptions<mysqlContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        public mysqlContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public virtual DbSet<User> User { get; set; }

        public virtual DbSet<Block> Block { get; set; }
    }
}
