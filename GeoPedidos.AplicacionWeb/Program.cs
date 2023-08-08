using GeoPedidos.AplicacionWeb.Utilidades.AutoMapper;
using GeoPedidos.IOC;

using GeoPedidos.AplicacionWeb.Utilidades.Extensiones;
using DinkToPdf;
using DinkToPdf.Contracts;

using Microsoft.AspNetCore.Authentication.Cookies; // LOGIN

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option => {
    option.LoginPath = "/Acceso/Login";
    option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
}); // SERVICIO DE LOGUEO

builder.Services.InyectarDependencia(builder.Configuration); // Llamamos al metodo de inyectar dependencia que se encuentra en la capa SistemaVentas.IOC

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));// Ejecutar AUTOMAPPER

var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "Utilidades/LibreriaPDF/libwkhtmltox.dll"));
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools())); // AGREGA LIBRERIA PDF

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

app.UseAuthentication(); // PARA USAR EL LOGUEO

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();
