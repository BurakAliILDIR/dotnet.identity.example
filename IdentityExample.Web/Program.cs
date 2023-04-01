using IdentityExample.Web.Models;
using Microsoft.EntityFrameworkCore;
using IdentityExample.Web.Extensions;
using IdentityExample.Web.Services;
using IdentityExample.Web.Settings;
using Microsoft.AspNetCore.Identity;

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


// Password reset i�in token s�resi ayarlamas�.
builder.Services.Configure<DataProtectionTokenProviderOptions>(config =>
{
    config.TokenLifespan = TimeSpan.FromHours(2);
});

// AppSettings.json dosyas�ndan class ile veri alma ayar�.
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();


// security stamp ayarlar�n�n customize edilmesi.
builder.Services.Configure<SecurityStampValidatorOptions>(config =>
{
    config.ValidationInterval = TimeSpan.FromHours(1);
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