using Microsoft.EntityFrameworkCore.Design;

namespace lms.services.usermanagement.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var dbContextBuilder = new DbContextOptionsBuilder<UserDbContext>();

            var connectionString = configuration.GetConnectionString("UserDatabase");

            dbContextBuilder.UseNpgsql(connectionString);

            return new UserDbContext(dbContextBuilder.Options);
        }
    }
}