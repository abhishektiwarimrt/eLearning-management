using Amazon.S3;
using Amazon.SQS;
using lms.buildingblocks.middleware;
using lms.services.aws.S3;
using lms.services.aws.SQS;
using lms.services.coursemanagement.Services;
using Serilog;

namespace lms.services.coursemanagement
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
                    TypeAdapterConfig.GlobalSettings.Default.MapToConstructor(true);
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
                        //var acessKey = builder.Configuration.GetSection("AWS:AccessKey").Value ?? throw new ArgumentNullException(typeof(S3ServiceEvent).Name, "AccessKey not found");
                        //var secretKey = builder.Configuration.GetSection("AWS:SecretKey").Value ?? throw new ArgumentNullException(typeof(S3ServiceEvent).Name, "SecretKey not found");
                        var s3BucketName = builder.Configuration.GetSection("AWS:S3Bucket:Name").Value ?? throw new ArgumentNullException(typeof(S3ServiceEvent).Name, "S3Bucket:Name not found");
                        //var s3BucketRegion = builder.Configuration.GetSection("AWS:S3Bucket:Region").Value ?? throw new ArgumentNullException(typeof(S3ServiceEvent).Name, "S3Bucket:Region not found"); ;
                        var s3Client = sp.GetRequiredService<IAmazonS3>();
                        return new S3ServiceEvent(s3Client, s3BucketName);
                    });

                    builder.Services.AddScoped<ISqsServiceEvent>(sp =>
                    {
                        var sqsClient = sp.GetRequiredService<IAmazonSQS>();
                        var configuration = sp.GetRequiredService<IConfiguration>();
                        return new SqsServiceEvent(sqsClient, configuration);
                    });
                    //builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
                    builder.Services.AddHostedService<FileUploadWorkerService>();
                    


                    builder.Services.AddExceptionHandler<CustomExceptionHandler>();
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();
                    builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();


                    // Read Serilog configuration from appsettings.json
                    var configuration = new ConfigurationBuilder()
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
                    //app.UseAntiforgery();
                    // Add custom middlewareu
                    if (app.Environment.IsDevelopment())
                    {
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
                    }
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
