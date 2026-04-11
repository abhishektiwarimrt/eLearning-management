using Asp.Versioning.ApiExplorer;
using lms.buildingblocks.middleware;
using lms.buildingblocks.OpenAPI;
using lms.shared.data.repositories.instructormanagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

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

                    // AddIdentity registers cookie auth internally — must come BEFORE we
                    // override the default scheme to JWT, otherwise Identity wins.
                    builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
                    {
                        options.Password.RequireDigit = true;
                        options.Password.RequiredLength = 10;
                        options.Password.RequireNonAlphanumeric = true;
                        options.Lockout.MaxFailedAccessAttempts = 5;
                    })
                    .AddEntityFrameworkStores<UserDbContext>()
                    .AddDefaultTokenProviders();

                    // Override the default scheme that AddIdentity set back to cookies.
                    // JWT Bearer must be the default so API endpoints return 401, not redirect.
                    var jwtSection = builder.Configuration.GetSection("Jwt");
                    var jwtKey = jwtSection["Key"];
                    if (!string.IsNullOrWhiteSpace(jwtKey))
                    {
                        builder.Services
                            .AddAuthentication(options =>
                            {
                                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                            })
                            .AddJwtBearer(options =>
                            {
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    ValidIssuer = jwtSection["Issuer"],
                                    ValidAudience = jwtSection["Audience"],
                                    IssuerSigningKey = new SymmetricSecurityKey(
                                        Encoding.UTF8.GetBytes(jwtKey)),
                                    ClockSkew = TimeSpan.FromMinutes(1)
                                };
                            });
                        builder.Services.AddAuthorization();
                    }

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
                },
                configureApp: app =>
                {
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
                            c.RoutePrefix = string.Empty;
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error configuring Swagger: {ex.Message}");
                    }
                    app.UseExceptionHandler(options => { });
                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.UseMiddleware<RateLimitingMiddleware>();
                    app.MapGet("/health", () => "Healthy");
                }
            );

            app.Run();
        }
    }
}
