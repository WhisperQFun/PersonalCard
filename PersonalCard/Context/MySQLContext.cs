using Microsoft.EntityFrameworkCore;
using PersonalCard.Blockchain;
using PersonalCard.Models;

namespace PersonalCard.Context
{
    public class MySQLContext : DbContext
    {
        public MySQLContext(DbContextOptions<MySQLContext> options)
            : base(options) => Database.EnsureCreated();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => base.OnModelCreating(modelBuilder);

        #region DataBase Sets

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Block> Block { get; set; }
        public DbSet<Access> Acess { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<API> Api { get; set; }

        #endregion
    }
}
