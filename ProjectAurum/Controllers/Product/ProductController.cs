using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAurum.Data;
using ProjectAurum.Models.Category;
using ProjectAurum.Models.Product;

namespace ProjectAurum.Controllers.Product
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Recupera todos los productos desde la base de datos
            var productos = await _context.Productos.ToListAsync();
            return View(productos);

        }

        public async Task<IActionResult> ShowProducto()
        {
            // Llama al procedimiento almacenado GetProductosPorCategoria1 para obtener productos de la primera categoría
            var productos = await _context.Productos
             .FromSqlRaw("CALL GetProductosPorCategoria1()")
             .ToListAsync();

            return View(productos);

        }

        public async Task<IActionResult> ShowCategory2()
        {
            // Llama al procedimiento almacenado GetProductosPorCategoria2 para obtener productos de la segunda categoría
            var productos = await _context.Productos
             .FromSqlRaw("CALL GetProductosPorCategoria2()")
             .ToListAsync();

            return View(productos);

        }

        public async Task<IActionResult> ShowCategory3()
        {
            // Llama al procedimiento almacenado GetProductosPorCategoria3 para obtener productos de la tercera categoría
            var productos = await _context.Productos
             .FromSqlRaw("CALL GetProductosPorCategoria3()")
             .ToListAsync();

            return View(productos);

        }

        public async Task<IActionResult> ShowCategory4()
        {
            // Llama al procedimiento almacenado GetProductosPorCategoria4 para obtener productos de la cuarta categoría
            var productos = await _context.Productos
             .FromSqlRaw("CALL GetProductosPorCategoria4()")
             .ToListAsync();

            return View(productos);

        }

        // Acción para mostrar el formulario de creación de un nuevo producto
        public IActionResult Create()
        {


            ViewBag.Categorías = _context.Categorías.ToList();
            return View();
        }
        [HttpPost]

        // Acción que maneja el formulario POST para crear un nuevo producto
        public IActionResult Create(string Nombre, int CategoríaId)
        {
            // Verificar si el producto ya existe en la base de datos
            var ProductoExistente = _context.Productos.FirstOrDefault(c => c.Nombre == Nombre);

            if (ProductoExistente != null)
            {
                ViewBag.ErrorMessage = "Producto ya creado.";
                return View(); // Retorna a la vista de registro con el mensaje de error
            }

            var categoriaExistente = _context.Categorías.FirstOrDefault(c => c.Id == CategoríaId);
            if (categoriaExistente == null)
            {
                ViewBag.ErrorMessage = "La categoría seleccionada no existe.";
                return View();
            }

            var producto = new Productos
            {
                Nombre = Nombre,
                CategoríaID = CategoríaId, // Asignar el CategoríaID
                Año = DateTime.Now.Year,// Asignar el año actual como valor por defecto
                Precio = 0,
                Descripción="" ,
            };


            // Guardar en la base de datos
            _context.Productos.Add(producto);
            _context.SaveChanges();

            // Redirigir al Login después del registro exitoso
            return RedirectToAction("Index");

        }

        // Acción para mostrar el formulario de edición de un producto
        public async Task<IActionResult> Edit(int id)
        {
            // Busca el producto en la base de datos por su ID
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // Acción que maneja el formulario POST para editar un producto
        [HttpPost]

        public async Task<IActionResult> Edit(int id, string Nombre)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            producto.Nombre = Nombre;
            _context.Update(producto);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }
        // Acción para mostrar el formulario de eliminación de un producto
        public async Task<IActionResult> Delete(int id)
        {
            // Busca el producto en la base de datos por su ID
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // Acción que maneja la eliminación de un producto (confirmación)
        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            // Elimina el producto de la base de datos
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }

    }
}
