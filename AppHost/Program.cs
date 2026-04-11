IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL Database
//var postgres = builder.AddPostgres("postgres")
//    //.WithPgAdmin()
//    .WithLifetime(ContainerLifetime.Persistent);

//var courseDb = postgres.AddDatabase("coursedb");
//var userDb = postgres.AddDatabase("userdb");

// Redis
var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent);

// Microservices — launchProfileName: "https" picks the https profile from each project's launchSettings.json
// This registers both the https and http endpoints so WithReference injects services__<name>__https__0
var courseManagementService = builder.AddProject<Projects.lms_services_coursemanagement>("coursemanagement", launchProfileName: "https")
    //.WithReference(courseDb)
    .WithReference(redis);

var userManagementService = builder.AddProject<Projects.lms_services_usermanagement>("usermanagement", launchProfileName: "https")
    //.WithReference(userDb)
    .WithReference(redis);

// Web frontend
builder.AddProject<Projects.lms_web>("web", launchProfileName: "https")
    .WithReference(courseManagementService)
    .WithReference(userManagementService);

builder.Build().Run();
