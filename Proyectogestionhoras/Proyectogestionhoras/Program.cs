using Proyectogestionhoras.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Proyectogestionhoras.Services;
using Proyectogestionhoras.Services.Interface;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);



Log.Logger = new LoggerConfiguration()
    .WriteTo.File(@"C:\Users\pepef\Desktop\proyecto gestion\proyectogestorhoras\Proyectogestionhoras\Proyectogestionhoras\logs\myapp-.txt",
                   rollingInterval: RollingInterval.Day,
                   rollOnFileSizeLimit: true) // Permite crear nuevos archivos si el límite de tamaño es alcanzado
    .CreateLogger();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Esto lee la configuración de appsettings.json
    .CreateLogger();
// Reemplaza el logger predeterminado por Serilog
builder.Host.UseSerilog();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1800);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

});
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<Conexion>();
builder.Services.AddScoped<ProyectoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ContactoService>();
builder.Services.AddScoped<SegmentoService>();
builder.Services.AddScoped<FacturaService>();
builder.Services.AddScoped<PlanillaService>();
builder.Services.AddDbContext<PROYECTO_CONTROL_HORASContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("cadenasql")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();
// Configurar la localización (cultura)
var supportedCultures = new[]
{
    new CultureInfo("es-CL")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("es-CL"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseDatabaseErrorPage();
}

else
{
    app.UseDeveloperExceptionPage();
    app.UseDatabaseErrorPage();
    // app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
