using Asp.Versioning.ApiExplorer;
using lms.buildingblocks.middleware;
using lms.buildingblocks.OpenAPI;
using lms.shared.data.repositories.instructormanagement;
using System.Reflection;

namespace lms.services.usermanagement
{
    public partial class Program : VersionedApiProgram
    {
        public static void Main(string[] args)
        {
            Assembly assembly = typeof(Program).Assembly;
            WebApplication app = ConfigureApi(
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
                    builder.Services.AddDbContext<UserDbContext>(options =>
                        options.UseNpgsql(builder.Configuration.GetConnectionString("UserDatabase"),
                        x => x.MigrationsAssembly("lms.shared.data")));

                    builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
                    {
                        //password rules
                        options.Password.RequireDigit = true;
                        options.Password.RequiredLength = 10;
                        options.Password.RequireNonAlphanumeric = true;

                        //locaout (prevent brute force)
                        options.Lockout.MaxFailedAccessAttempts = 5;
                    })
                    .AddEntityFrameworkStores<UserDbContext>()
                    .AddDefaultTokenProviders();

                    builder.Services.AddScoped<IUserService, UserService>();
                    builder.Services.AddScoped<IRoleService, RoleService>();
                    builder.Services.AddScoped<IUserRepository, UserRepository>();
                    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
                    builder.Services.AddScoped<IOnboardingRepository, OnboardingRepository>();
                    builder.Services.AddScoped<IUnitOfWork<UserDbContext>, UnitOfWork<UserDbContext>>();

                    builder.Services.AddExceptionHandler<CustomExceptionHandler>();
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();
                    builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

                    // Read Serilog configuration from appsettings.json
                    IConfigurationRoot configuration = new ConfigurationBuilder()
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
                        IReadOnlyList<ApiVersionDescription> descriptons = app.DescribeApiVersions();
                        app.UseSwaggerUI(c =>
                        {
                            foreach (ApiVersionDescription description in descriptons)
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