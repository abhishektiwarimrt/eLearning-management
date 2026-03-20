using FluentValidation;
using lms.buildingblocks.apiversioning;
using lms.buildingblocks.behaviours;
using lms.buildingblocks.exceptions.Handler;
using lms.buildingblocks.middleware;
using lms.buildingblocks.OpenAPI;
using lms.shared.data.dbcontexts;
using lms.shared.data.repositories.usermanagement;
using lms.shared.data.unitofwork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace lms.services.instructor
{
    public partial class Program : VersionedApiProgram
    {
        public static void Main(string[] args)
        {
            var assembly = typeof(Program).Assembly;
            var app = ConfigureApi(
                args,
                configureServices: builder =>
                {
                    // Add custom services
                    // Add services to the container
                    builder.Services.AddMediatR(config =>
                    {
                        config.RegisterServicesFromAssemblies(assembly);
                        config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                        config.AddOpenBehavior(typeof(LogBehavior<,>));
                    });
                    builder.Services.AddValidatorsFromAssembly(assembly);
                    // Add services to the container
                    builder.Services.AddMediatR(config =>
                    {
                        config.RegisterServicesFromAssemblies(assembly);
                        config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                        config.AddOpenBehavior(typeof(LogBehavior<,>));
                    });
                    builder.Services.AddValidatorsFromAssembly(assembly);
                    builder.Services.AddDbContext<InstructorDBContext>(options =>
                        options.UseNpgsql(builder.Configuration.GetConnectionString("InstructorDatabase"),
                        x => x.MigrationsAssembly("lms.shared.data")));

                   

                    //builder.Services.AddScoped<IInstructorService, InstructorService>();
                    //builder.Services.AddScoped<IRoleService, RoleService>();
                    //builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
                    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
                    builder.Services.AddScoped<IUnitOfWork<InstructorDBContext>, UnitOfWork<InstructorDBContext>>();

                    builder.Services.AddExceptionHandler<CustomExceptionHandler>();
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();
                    builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

                    // Read Serilog configuration from appsettings.json
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    //Log.Logger = new LoggerConfiguration()
                    //    .ReadFrom.Configuration(configuration)
                    //    .CreateLogger();

                    //builder.Services.AddLogging(loggingBuilder =>
                    //    loggingBuilder.AddSerilog(dispose: true));
                },
                configureApp: app =>
                {
                    // Add custom middleware
                    //if (app.Environment.IsDevelopment())
                    //{
                    app.UseDeveloperExceptionPage();
                    try
                    {
                        app.UseSwagger();
                        var descriptons = app.DescribeApiVersions();
                        app.UseSwaggerUI(c =>
                        {
                            foreach (var description in descriptons)
                            {
                                string url = $"/swagger/{description.GroupName}/swagger.json";
                                string name = description.GroupName.ToUpperInvariant();
                                c.SwaggerEndpoint(url, name);
                            }

                            c.RoutePrefix = string.Empty; // Serve the Swagger UI at the app's root
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error configuring Swagger: {ex.Message}");
                    }
                    //}
                    app.UseExceptionHandler(options => { });
                    app.UseMiddleware<RateLimitingMiddleware>();
                    // You can also add custom endpoints here if needed
                    app.MapGet("/health", () => "Healthy");
                }
            );

            app.Run();
        }
    }
}