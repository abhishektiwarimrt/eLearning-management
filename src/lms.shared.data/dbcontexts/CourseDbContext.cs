using lms.shared.data.entities.coursemanagement;
using lms.shared.data.entities.coursemanagement.Content;
using Microsoft.EntityFrameworkCore;

namespace lms.shared.data.dbcontexts
{
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSection> CourseSections { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }
        public DbSet<FileUploadQueueItem> FileUploadQueueItems { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Course configuration
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Sections)
                .WithOne(s => s.Course)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // CourseSection configuration
            modelBuilder.Entity<CourseSection>()
                .HasMany(s => s.CourseModules)
                .WithOne(m => m.CourseSection)
                .HasForeignKey(m => m.CourseSectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // CourseModule configuration
            modelBuilder.Entity<CourseModule>()
                .HasMany(m => m.Quizes)
                .WithOne(m => m.CourseModule)
                .HasForeignKey(m => m.CourseModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quiz configuration
            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question configuration
            modelBuilder.Entity<Question>()
                .HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // FileUploadQueueItem: configure composite key (CourseModuleId + FileName)
            modelBuilder.Entity<FileUploadQueueItem>()
                .HasKey(f => new { f.CourseModuleId, f.FileName });

            // FileUploadQueueItem -> CourseModule (one-to-many)
            modelBuilder.Entity<FileUploadQueueItem>()
                .HasOne(f => f.CourseModule)
                .WithMany(m => m.FileUploadQueueItems)
                .HasForeignKey(f => f.CourseModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


