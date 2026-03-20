using lms.shared.data.entities.instructormanagement;
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
        public DbSet<InstructorOnboardingStatus> InstructorOnboardingStatuses { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InstructorOnboardingStatus>(e =>
            {
                e.ToTable("InstructorOnboardingStatuses");

                e.HasKey(x => x.Id);

                // One user → exactly one onboarding record
                e.HasIndex(x => x.UserId).IsUnique();

                // FK → AspNetUsers.Id — cascade so orphan rows never exist
                e.HasOne(x => x.User)
                 .WithOne()
                 .HasForeignKey<InstructorOnboardingStatus>(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.Status)
                 .HasMaxLength(20)
                 .HasDefaultValue(OnboardingStatusValue.InProgress)
                 .IsRequired();

                e.Property(x => x.Headline).HasMaxLength(60);
                e.Property(x => x.Language).HasMaxLength(100);
                e.Property(x => x.PayoutMethod).HasMaxLength(50);
            });
        }
    }
}