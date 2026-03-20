using lms.shared.data.dbcontexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace lms.services.instructor.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<InstructorDBContext>
    {
        public InstructorDBContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var dbContextBuilder = new DbContextOptionsBuilder<InstructorDBContext>();

            var connectionString = configuration.GetConnectionString("InstructorDatabase");

            dbContextBuilder.UseNpgsql(connectionString);

            return new InstructorDBContext(dbContextBuilder.Options);
        }
    }
}