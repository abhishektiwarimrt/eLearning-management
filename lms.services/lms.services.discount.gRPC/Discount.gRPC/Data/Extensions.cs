using Microsoft.EntityFrameworkCore;

namespace Discount.gRPC.Data
{
    public static class Extensions
    {
        public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetService<DiscountContext>();
            context.Database.MigrateAsync();

            return app;
        }
    }
}
