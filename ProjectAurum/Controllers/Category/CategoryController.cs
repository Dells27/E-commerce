using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAurum.Data;
using ProjectAurum.Models.Category;
using ProjectAurum.Models.User;

namespace ProjectAurum.Controllers.Categories
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categorías.ToListAsync();

            return View(categories);
        }

        // Acción que muestra el formulario para crear una nueva categoría
        public IActionResult Create()
        {
            return View();
        }

        // Acción que maneja la creación de una nueva categoría (POST)
        [HttpPost]
        public IActionResult Create(string Nombre)
        {
            // Verificar si la categoria ya existe en la base de datos
            var categoriaExistente = _context.Categorías.FirstOrDefault( c=> c.Nombre == Nombre);

            if (categoriaExistente != null)
            {
                ViewBag.ErrorMessage = "Categoria ya creada.";
                return View(); // Retorna a la vista de registro con el mensaje de error
            }


            // Crear nueva categoria
            var categoria = new Categorías
            {
                Nombre = Nombre
            };

            // Guardar en la base de datos
            _context.Categorías.Add(categoria);
            _context.SaveChanges();


            
           
            // Redirigir al Login después del registro exitoso
            return RedirectToAction("Index");          
       
        }

        // Acción para mostrar el formulario de edición de una categoría existente
        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await _context.Categorías.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();// Retorna 404 si no se encuentra la categoría
            }
            return View(categoria);
        }

        // Acción que maneja la edición de una categoría (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string Nombre)
        {
            var categoria = await _context.Categorías.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            categoria.Nombre = Nombre;
            _context.Update(categoria);
            await _context.SaveChangesAsync();// Guarda los cambios en la base de datos


            return RedirectToAction("Index");
        }

        // Acción para mostrar la confirmación de eliminación de una categoría
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorías.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // Acción que elimina definitivamente la categoría (POST)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorías.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            _context.Categorías.Remove(categoria); // Elimina la categoría
            await _context.SaveChangesAsync();// Guarda los cambios


            return RedirectToAction("Index");
        }
    }
}
