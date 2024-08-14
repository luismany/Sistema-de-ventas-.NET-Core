using SistemaVenta.IOC;
using SistemaVenta.AplicacionWeb.Utilidades.Automapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//agregamos el metodo inyeciondependencia de la clase Dependencia de la capa sistemaventa.IOC
builder.Services.InyectarDepependencia(builder.Configuration);

//inyectanto automapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

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
