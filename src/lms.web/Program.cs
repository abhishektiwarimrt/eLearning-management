using lms.web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Wire up Aspire service defaults: OpenTelemetry, health checks, service discovery
builder.AddServiceDefaults();

// ── Cookie authentication for MVC pages ──────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "LMS.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// ── Session (stores JWT for forwarding to microservices) ──────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── Authorization policies ────────────────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// ── JWT forwarding to microservices ───────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtForwardingHandler>();

string userMgmtBaseUrl =
    builder.Configuration["services__usermanagement__https__0"] ??
    builder.Configuration["services__usermanagement__http__0"] ??
    builder.Configuration["MicroServices:UserManagementUrl"] ??
    "https://localhost:5050/";

if (!userMgmtBaseUrl.EndsWith('/')) userMgmtBaseUrl += '/';

builder.Services.AddHttpClient(UserManagementService.HttpClientName, client =>
{
    client.BaseAddress = new Uri(userMgmtBaseUrl);
})
.AddHttpMessageHandler<JwtForwardingHandler>()
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Trust ASP.NET Core dev certificate in local Aspire runs
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddScoped<UserManagementService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ── HTTP pipeline ─────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();          // must be before Authentication so session is readable
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapDefaultEndpoints();

app.Run();
