
using Microsoft.EntityFrameworkCore;

using AdminService.Entities.Identity;

namespace AdminService.Data
{
    public class IdentityDataContext: DbContext
    {
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(builder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Token>().ToTable("Tokens");
            modelBuilder.Entity<Code>().ToTable("Codes");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens {  get; set; }
        public DbSet<Code> Codes { get; set; }
    }
}
