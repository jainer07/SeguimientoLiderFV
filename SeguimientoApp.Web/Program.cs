using Microsoft.EntityFrameworkCore;
using SeguimientoApp.Application.Ports.Persistence;
using SeguimientoApp.Application.UseCases.Catalogos;
using SeguimientoApp.Application.UseCases.Eventos;
using SeguimientoApp.Application.UseCases.Personas;
using SeguimientoApp.Infrastructure.Persistence.MySql;
using SeguimientoApp.Infrastructure.Persistence.MySql.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(cs, ServerVersion.AutoDetect(cs)));

builder.Services.AddScoped<IPersonaRepositoryPort, PersonaRepository>();
builder.Services.AddScoped<GetPersonas>();
builder.Services.AddScoped<GetPersonaById>();
builder.Services.AddScoped<CreatePersona>();
builder.Services.AddScoped<UpdatePersona>();
builder.Services.AddScoped<ICatalogoRepositoryPort, CatalogoRepository>();
builder.Services.AddScoped<GetLsCatalogo>();
builder.Services.AddScoped<IEventoRepositoryPort, EventoRepository>();
builder.Services.AddScoped<GetEventos>();
builder.Services.AddScoped<GetEventoById>();
builder.Services.AddScoped<CreateEvento>();
builder.Services.AddScoped<UpdateEvento>();
builder.Services.AddScoped<IEventoActividadRepositoryPort, EventoActividadRepository>();
builder.Services.AddScoped<IActividadRegistroRepositoryPort, ActividadRegistroRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
