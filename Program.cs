using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Portafolio.Services;

var builder = WebApplication.CreateBuilder(args);

//ARCHIVOS DE CONFIGURACION
//optional : false (EL ARCHIVO NO ES OPCIONAL Y LA API NO CARGARA SI NO SE PUEDE CARGAR EL ARCHIVO) 
//reloadOnChange : true (EL ARCHIVO ADMITE CAMBIOS Y SE RECARGARA CUANDO LOS DETECTE)
builder.Configuration.AddJsonFile("Settings/AttachmentFiles.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("Settings/Credentials.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("Settings/Providers.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("Settings/FileExtensions.json", optional: false, reloadOnChange: true);

//SERVICIOS
builder.Services.AddControllersWithViews();
//AddTransient: PROPORCIONA UNA INSTANCIA AISLADA (INDEPENDIENTE)
builder.Services.AddTransient<EmailService, EmailService>();
builder.Services.AddTransient<FileManagerService, FileManagerService>();
builder.Services.AddTransient<MenuService, MenuService>();
//AddScopped: PROPORCIONA UNA INSTANCIA COMPARTIDA (MISMO CONTEXTO) 
builder.Services.AddScoped<ConnectionService, ConnectionService>();
builder.Services.AddScoped<InventoryService, InventoryService>();
builder.Services.AddScoped<ReviewService, ReviewService>();
builder.Services.AddScoped<ReservationService, ReservationService>();
//PROPORCIONA EL TIPO DE SERVICIO PARA LA CONECTIVIDAD CON ENTITY FRAMEWORK
builder.Services.AddDbContext<StorageService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
