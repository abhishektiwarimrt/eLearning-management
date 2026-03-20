using lms.shared.data.entities.instructormanagement;
using Microsoft.EntityFrameworkCore;


namespace lms.shared.data.dbcontexts
{
    public class InstructorDBContext : DbContext
    {
        public InstructorDBContext(DbContextOptions<InstructorDBContext> options) : base(options)
        {
        }

        public DbSet<InstructorProfile> InstructorProfile { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<InstructorProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ApplicationUserId).IsUnique(); // One profile per user
                entity.Property(e => e.ApplicationUserId).IsRequired();
            });
        }
    }
}
