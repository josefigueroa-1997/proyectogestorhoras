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
using System.Diagnostics;
var builder = WebApplication.CreateBuilder(args);



try
{
    
    var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

    
    if (!Directory.Exists(logDirectory))
    {
        Directory.CreateDirectory(logDirectory);
    }

    var logFilePath = Path.Combine(logDirectory, "stdout-.txt");

    
    Log.Logger = new LoggerConfiguration()
        .WriteTo.File(
            path: logFilePath,
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true)
        .CreateLogger();

    builder.Host.UseSerilog();
}
catch (Exception ex)
{
    
    Console.WriteLine("Error configurando el logger: " + ex.Message);
}


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(12);
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
builder.Services.AddScoped<ReporteService>();
builder.Services.AddScoped<MetaService>();
builder.Services.AddScoped<BonoService>();
builder.Services.AddScoped<PresupuestoService>();
builder.Services.AddScoped<EjecucionService>();
builder.Services.AddScoped<ExcelService>();

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
