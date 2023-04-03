using IdentityExample.Web.Models;
using Microsoft.EntityFrameworkCore;
using IdentityExample.Web.Extensions;
using IdentityExample.Web.Services;
using IdentityExample.Web.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using IdentityExample.Web.ClaimProvides;
using IdentityExample.Web.Requirements;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// identity
builder.Services.AddIdentityWithExtension();

// cookie options
builder.Services.ConfigureApplicationCookie(options =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie";
    options.LoginPath = new PathString("/Home/SignIn");
    options.LogoutPath = new PathString("/Member/Logout");
    options.Cookie = cookieBuilder;
    options.ExpireTimeSpan = TimeSpan.FromDays(60);
    options.SlidingExpiration = true;
});


// Password reset için token süresi ayarlamasý.
builder.Services.Configure<DataProtectionTokenProviderOptions>(config =>
{
    config.TokenLifespan = TimeSpan.FromHours(2);
});

// AppSettings.json dosyasýndan class ile veri alma ayarý.
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();


// security stamp ayarlarýnýn customize edilmesi.
builder.Services.Configure<SecurityStampValidatorOptions>(config =>
{
    config.ValidationInterval = TimeSpan.FromHours(1);
});

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

// cookie'ye özel claim ekleme.
builder.Services.AddScoped<IClaimsTransformation, ClaimProvider>();

// Policy bazlý yetkilendirme için.
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();

// Policy ekleme.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AnkaraPolicy", policy =>
    {
        policy.RequireClaim("city", "ankara", "manisa");
        policy.RequireAuthenticatedUser();
        policy.RequireRole("admin");
    });

    options.AddPolicy("ExchangeExpirePolicy", policy => { policy.AddRequirements(new ExchangeExpireRequired()); });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();