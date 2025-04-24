using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectAurum.Models;

namespace ProjectAurum.Controllers.Home
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // Acción que devuelve el index
        public IActionResult Index()
        {
            return View();
        }

        // Acción que devuelve la vista de privacidad
        public IActionResult Privacy()
        {
            return View();
        }
        // Acción que devuelve la vista de contacto
        public IActionResult Contact()
        {
            return View();
        }
        // Acción que devuelve la vista de preguntas frecuentes (FAQ)
        public IActionResult FAQ()
        {
            return View();
        }


        // Acción que se muestra en caso de error del sistema
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Crea un modelo de error con el ID de la solicitud para mostrarlo al usuario
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
