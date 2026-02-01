

namespace lms.services.usermanagement.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<CourseDbContext>
    {
        public CourseDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var dbContextBuilder = new DbContextOptionsBuilder<CourseDbContext>();

            var connectionString = configuration.GetConnectionString("CourseDatabase");

            dbContextBuilder.UseNpgsql(connectionString);

            return new CourseDbContext(dbContextBuilder.Options);
        }
    }
}