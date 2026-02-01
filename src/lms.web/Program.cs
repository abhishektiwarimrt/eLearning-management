using Yarp.ReverseProxy; // Add this at the top of the file

var builder = WebApplication.CreateBuilder(args);

// React proxy for dev
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapReverseProxy();  // Proxies /api/* to your backend
}

app.MapFallbackToFile("index.html");  // SPA routing
app.Run();
