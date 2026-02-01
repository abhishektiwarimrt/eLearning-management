using lms.shared.data.entities.usermanagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace lms.shared.data.dbcontexts
{
    public class UserDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<User> ApplicationUsers { get; set; }
        public DbSet<IdentityRole<int>> ApplciationRoles { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
