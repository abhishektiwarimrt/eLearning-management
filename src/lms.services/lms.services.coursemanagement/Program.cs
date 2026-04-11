using Amazon.S3;
using Amazon.SQS;
using Asp.Versioning.ApiExplorer;
using lms.buildingblocks.middleware;
using lms.services.aws.S3;
using lms.services.aws.SQS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Reflection;
using System.Text;

namespace lms.services.coursemanagement
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
                    // JWT Bearer authentication
                    var jwtSection = builder.Configuration.GetSection("Jwt");
                    var jwtKey = jwtSection["Key"];
                    if (!string.IsNullOrWhiteSpace(jwtKey))
                    {
                        builder.Services
                            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

                    TypeAdapterConfig.GlobalSettings.Default.MapToConstructor(true);
                    builder.Services.AddMediatR(config =>
                    {
                        config.RegisterServicesFromAssemblies(assembly);
                        config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                        config.AddOpenBehavior(typeof(LogBehavior<,>));
                    });
                    builder.Services.AddValidatorsFromAssembly(assembly);
                    builder.Services.AddDbContext<CourseDbContext>(options =>
                        options.UseNpgsql(builder.Configuration.GetConnectionString("CourseDatabase"),
                        x => x.MigrationsAssembly("lms.shared.data")));

                    builder.Services.AddAWSService<IAmazonS3>();
                    builder.Services.AddAWSService<IAmazonSQS>();
                    builder.Services.AddScoped<ICourseService, CourseService>();
                    builder.Services.AddScoped<ICourseSectionService, CourseSectionService>();
                    builder.Services.AddScoped<ICourseModuleService, CourseModuleService>();
                    builder.Services.AddScoped<ICourseEnrollmentService, CourseEnrollmentService>();
                    builder.Services.AddScoped<ICourseRepository, CourseRepository>();
                    builder.Services.AddScoped<ICourseSectionRepository, CourseSectionRepository>();
                    builder.Services.AddScoped<ICourseModuleRepository, CourseModuleRepository>();
                    builder.Services.AddScoped<IFileUploadQueueItemRepository, FileUploadQueueItemRepository>();
                    builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
                    builder.Services.AddScoped<IUnitOfWork<CourseDbContext>, UnitOfWork<CourseDbContext>>();
                    builder.Services.AddScoped<IS3ServiceEvent>(sp =>
                    {
                        var s3BucketName = builder.Configuration.GetSection("AWS:S3Bucket:Name").Value ?? throw new ArgumentNullException(typeof(S3ServiceEvent).Name, "S3Bucket:Name not found");
                        IAmazonS3 s3Client = sp.GetRequiredService<IAmazonS3>();
                        return new S3ServiceEvent(s3Client, s3BucketName);
                    });

                    builder.Services.AddScoped<ISqsServiceEvent>(sp =>
                    {
                        IAmazonSQS sqsClient = sp.GetRequiredService<IAmazonSQS>();
                        IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
                        return new SqsServiceEvent(sqsClient, configuration);
                    });

                    builder.Services.AddExceptionHandler<CustomExceptionHandler>();
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();
                    builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

                    IConfigurationRoot configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();

                    builder.Services.AddLogging(loggingBuilder =>
                        loggingBuilder.AddSerilog(dispose: true));
                },
                configureApp: app =>
                {
                    if (app.Environment.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
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
