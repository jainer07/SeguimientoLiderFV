using Microsoft.EntityFrameworkCore;
using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;
using SeguimientoApp.Application.UseCases.Catalogos;
using SeguimientoApp.Application.UseCases.Eventos;
using SeguimientoApp.Application.UseCases.Notificacion;
using SeguimientoApp.Application.UseCases.Personas;
using SeguimientoApp.Infrastructure.Notificacion.Onurix;
using SeguimientoApp.Infrastructure.Persistence.MySql;
using SeguimientoApp.Infrastructure.Persistence.MySql.Repositories;
using SeguimientoApp.Web.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(cs, ServerVersion.AutoDetect(cs)));

builder.Services.AddSession();

builder.Services.AddScoped<IPersonaRepositoryPort, PersonaRepository>();
builder.Services.AddScoped<GetPersonas>();
builder.Services.AddScoped<GetPersonaById>();
builder.Services.AddScoped<CreatePersona>();
builder.Services.AddScoped<UpdatePersona>();
builder.Services.AddScoped<ImportVotantes>();
builder.Services.AddScoped<ICatalogoRepositoryPort, CatalogoRepository>();
builder.Services.AddScoped<GetLsCatalogo>();
builder.Services.AddScoped<IEventoRepositoryPort, EventoRepository>();
builder.Services.AddScoped<GetEventos>();
builder.Services.AddScoped<GetEventoById>();
builder.Services.AddScoped<CreateEvento>();
builder.Services.AddScoped<UpdateEvento>();
builder.Services.AddScoped<IEventoActividadRepositoryPort, EventoActividadRepository>();
builder.Services.AddScoped<IActividadRegistroRepositoryPort, ActividadRegistroRepository>();
builder.Services.AddScoped<ISmsOutboxRepositoryPort, SmsOutboxRepository>();
builder.Services.AddScoped<ScheduleSmsBulk>();
builder.Services.AddScoped<GetSmsJob>();

builder.Services.Configure<SmsSendingOptions>(builder.Configuration.GetSection("SmsSending"));
builder.Services.AddHostedService<SmsOutboxWorker>();

// ===== Onurix SMS =====
var onurixOpt = builder.Configuration.GetSection("Onurix").Get<OnurixOptions>() ?? new OnurixOptions();
builder.Services.AddSingleton(onurixOpt);
builder.Services.AddHttpClient("onurix", client =>
{
    client.BaseAddress = new Uri(onurixOpt.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<INotificacionRepositoryPort>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var http = factory.CreateClient("onurix");
    var opt = sp.GetRequiredService<OnurixOptions>();
    return new OnurixSmsSender(http, opt);
});

builder.Services.AddScoped<SendSms>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Personas}/{action=Index}/{id?}");

app.Run();
