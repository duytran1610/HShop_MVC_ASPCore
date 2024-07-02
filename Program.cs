using HShop.Models;
using HShop.Services;
using HShop.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DB
builder.Services.AddDbContext<HshopContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HShop"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// Inject Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Inject AutoMapper
// https://docs.automapper.org/en/stable/Dependency-injection.html
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/KhachHang/Login";
        options.AccessDeniedPath = "/AccessDenied";
    });

// Inject PaypalClient Service
builder.Services.AddSingleton(x => new PaypalClient(
    builder.Configuration["PaypalOptions:ClientId"],
    builder.Configuration["PaypalOptions:ClientSecret"],
    builder.Configuration["PaypalOptions:Mode"]
));

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
