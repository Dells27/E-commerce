using Microsoft.EntityFrameworkCore;
using ProjectAurum.Data;

public partial class Program 
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configurar el DbContext con la cadena de conexión
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Agregar servicios al contenedor
        builder.Services.AddControllersWithViews();
        builder.Services.AddTransient<EmailService>();
        builder.Services.AddSession();
        builder.Services.AddHttpContextAccessor();


        // Crea la aplicación web a partir de la configuración anterior
        var app = builder.Build();

        // Configurar el pipeline de solicitudes HTTP
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        //Middlewares
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();
        app.UseRouting();
        app.UseAuthorization();

        //Define el controller y la acción por defecto, por donde la aplicación inicia
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
