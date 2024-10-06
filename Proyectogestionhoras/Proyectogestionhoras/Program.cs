using Proyectogestionhoras.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Proyectogestionhoras.Services;
using Proyectogestionhoras.Services.Interface;
var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddDbContext<PROYECTO_CONTROL_HORASContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("cadenasql")));
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
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
