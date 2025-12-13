using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FitnessApp.Web.Data;
using FitnessApp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<FitnessApp.Web.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<FitnessApp.Web.Data.AppUser, Microsoft.AspNetCore.Identity.IdentityRole>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3; // "sau" is 3 chars
})
    .AddEntityFrameworkStores<FitnessApp.Web.Data.ApplicationDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("Admin") || 
            (context.User.Identity?.Name?.Contains("sakarya.edu.tr") == true)
        ));
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<IAIService, GeminiService>();

// API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Veri tohumlama (Seeding)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await FitnessApp.Web.Data.DbSeeder.SeedRolesAndAdminAsync(services);
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

var supportedCultures = new[] { "tr-TR" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("tr-TR")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// CRITICAL FIX: Add authentication middleware BEFORE authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
