
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

        public DbSet<TokenList> TokenList { get; set; }
        public DbSet<UserList> UserList { get; set; }
        public DbSet<CodesListWithUser> CodesListWithUser { get; set; }
    }
}
