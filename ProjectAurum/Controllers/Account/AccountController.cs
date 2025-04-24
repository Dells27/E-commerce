using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAurum.Data;
using ProjectAurum.Models.Category;
using ProjectAurum.Models.User;
using ProjectAurum;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectAurum.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context; //Inyectar el contexto de la base de datos

        private readonly EmailService _emailService; //Inyectar el servicio de correo electrónico. Envia correos

        public AccountController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        // Muestra la lista de usuarios registrados
        public async Task<IActionResult> Index()
        {
            var users = await _context.Usuarios.ToListAsync();

            return View(users);
        }
        // Muestra la vista del perfil del usuario
        public IActionResult Profile()
        {
            return View();
        }
        // Muestra el formulario de login
        public IActionResult Login()
        {
            return View();
        }


        // Procesa el login
        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Correo == Email && u.Contraseña == Password);

            if (user != null)
            {
                // Guardar datos del usuario en la sesión
                HttpContext.Session.SetString("UsuarioNombre", user.Nombre);
                HttpContext.Session.SetString("UsuarioRol", user.Rol);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Usuario o contraseña incorrectos";
                return View();
            }
        }
        // Cierra sesión limpiando la sesión
        public IActionResult Logout()
        {
            // Limpiar la sesión
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }



        // Muestra el formulario de registro
        public IActionResult Register()
        {
            return View();
        }

        // Procesa el registro y envía un correo de bienvenida
        [HttpPost]
        public async Task<IActionResult> Register(string Nombre, string Apellido, string Correo, string Password)
        {
            var usuarioExistente = _context.Usuarios.FirstOrDefault(u => u.Correo == Correo);

            if (usuarioExistente != null)
            {
                ViewBag.ErrorMessage = "Parece que ya tienes una cuenta con este correo. ¡Prueba iniciar sesión!";
                return View();
            }

            string nombreCompleto = $"{Nombre} {Apellido}";

            // Crea el objeto de usuario
            var usuario = new Usuarios
            {
                Nombre = nombreCompleto,
                Correo = Correo,
                Contraseña = Password,
                Rol = "usuario"
            };

            // Agrega y guarda el nuevo usuario en la base de datos
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Prepara el contenido del correo de bienvenida
            string subject = "¡Bienvenido a Aurum Company!";
            string body = $"Hola {nombreCompleto}, gracias por unirte exitosamente con el correo {Correo}.";

            // Envía el correo usando EmailService
            bool emailSent = await _emailService.SendEmailAsync(Correo, subject, body);


            // Verifica si se pudo enviar el correo y muestra mensajes apropiados
            if (emailSent)
            {
                TempData["SuccessMessage"] = "Cuenta creada exitosamente. ¡Revisa tu correo y ahora inicia sesión!";
            }
            else
            {
                TempData["ErrorMessage"] = "Cuenta creada, pero hubo un problema al enviar el correo.";
            }

            TempData["SuccessMessage"] = "Cuenta creada exitosamente. ¡Revisa tu correo y ahora inicia sesión!";
            // Redirigir al Login después del registro exitoso
            return RedirectToAction("Register");


            
        }








        // Muestra la vista de recuperación de contraseña (por implementar)
        public IActionResult ResetPassword()
        {
            return View();
        }
        // Muestra formulario para crear usuario manualmente
        public IActionResult Create()
        {
            return View();
        }

        // Procesa la creación manual de usuario
        [HttpPost]
        public IActionResult Create(string Nombre, string Apellido, string Correo, string Password)
        {
            // Verificar si el correo ya existe en la base de datos
            var usuarioExistente = _context.Usuarios.FirstOrDefault(u => u.Correo == Correo);

            if (usuarioExistente != null)
            {
                ViewBag.ErrorMessage = "Usuario ya existe.";
                return View(); // Retorna a la vista de registro con el mensaje de error
            }

            // Concatenar Nombre y Apellido
            string nombreCompleto = $"{Nombre} {Apellido}";

            // Crear nuevo usuario
            var usuario = new Usuarios
            {
                Nombre = nombreCompleto,
                Correo = Correo,
                Contraseña = Password,
                Rol = "usuario"
            };

            // Guardar en la base de datos
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();


            
            
            // Redirigir al Login después del registro exitoso
            return RedirectToAction("Index");
        }


        // Muestra formulario para editar usuario
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.Roles = new List<string> { "admin", "usuario" };

            return View(usuario);
        }


        // Procesa la edición de usuario
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string Nombre, string Correo, string Rol)
        {
            // Busca el usuario por ID
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Actualiza los datos del usuario
            usuario.Nombre = Nombre;
            usuario.Correo = Correo;
            usuario.Rol = Rol;

            // Guarda los cambios en la base de datos
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            
            

            return RedirectToAction("Index");
        }

        // Muestra confirmación para eliminar usuario
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }


        // Procesa la eliminación de un usuario
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Busca el usuario por ID
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Elimina al usuario de la base de datos
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }

    }
}
    