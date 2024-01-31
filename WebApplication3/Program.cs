using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddDataProtection();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configure lockout settings
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 3;

}).AddEntityFrameworkStores<AuthDbContext>();

builder.Services.ConfigureApplicationCookie(config =>
{
    config.LoginPath = "/login";
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    // Set the interval for security stamp validation
    options.ValidationInterval = TimeSpan.Zero; // Set to zero to validate on every request
});

builder.Services.AddReCaptcha(options =>
{
    options.SiteKey = "6LdYhGEpAAAAAGEMY5CVTnp3RxqbzEDXqaPtLW-f";
    options.SecretKey = "6LdYhGEpAAAAALtiEcsCubfpPHlQbCQtVY9e0vEd";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
