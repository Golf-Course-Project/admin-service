
using AdminService.ViewModels.Identity;

using Microsoft.EntityFrameworkCore;

namespace AdminService.Data
{
    public class IdentityDataContextForSp : DbContext
    {
        public IdentityDataContextForSp(DbContextOptions<IdentityDataContextForSp> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {         

        }

        public DbSet<Tokens> Tokens { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<DeletedUsers> DeletedUsers { get; set; }
        public DbSet<CodesListWithUser> CodesListWithUser { get; set; }
    }
}
