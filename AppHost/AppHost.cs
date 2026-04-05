using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);
//Micoroservices
IResourceBuilder<ProjectResource> course = builder.AddProject<lms_services_coursemanagement>("Lms-Services-CourseManagement");
IResourceBuilder<ProjectResource> user = builder.AddProject<lms_services_usermanagement>("Lm-Services-UserManagement");

//Web Project
builder.AddProject<lms_web>("Lms-Web")
 .WithExternalHttpEndpoints()
        .WithUrlForEndpoint("https", url => url.DisplayText = "EShop WebApp (HTTPS)")
        .WithUrlForEndpoint("http", url => url.DisplayText = "EShop WebApp (HTTP)")
        .WithReference(user)
        .WithReference(course);


builder.Build().Run();
