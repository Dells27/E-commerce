using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAurum.Data;
using ProjectAurum.Models.Cart.ShoppingCart;
using ProjectAurum.Models.ShoppingCartSing.ShoppingCartItem;

namespace ProjectAurum.Controllers.Carrito
{
    
    /// Controlador que gestiona todas las operaciones relacionadas con el carrito de compras.
    /// Utiliza cookies para identificar la sesión del usuario y Entity Framework para persistencia de datos.

    public class ShoppingCartController : Controller
    {
        private readonly AppDbContext _context;
        private const string SessionCookieName = "ShoppingCartSession";

        public ShoppingCartController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Muestra el contenido actual del carrito del usuario.
        /// </summary>
        /// <returns>Vista con el carrito asociado a la sesión.</returns>
        public async Task<IActionResult> Index()
        {
            var sessionId = GetOrCreateSessionId();
            var cart = await GetOrCreateCartAsync(sessionId);
            return View(cart);
        }

        /// <summary>
        /// Agrega un producto al carrito. Si ya existe, se incrementa la cantidad.
        /// </summary>
        /// <param name="productId">ID del producto a agregar.</param>
        /// <param name="quantity">Cantidad deseada (por defecto 1).</param>
        /// <returns>Redirecciona a la vista del carrito.</returns>
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            if (quantity <= 0) quantity = 1;

            var sessionId = GetOrCreateSessionId();
            await AddItemToCartAsync(sessionId, productId, quantity);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Actualiza la cantidad de un ítem del carrito. Si la cantidad es menor o igual a cero, se elimina el ítem.
        /// </summary>
        /// <param name="cartItemId">ID del ítem del carrito.</param>
        /// <param name="quantity">Nueva cantidad.</param>
        /// <returns>Redirecciona a la vista del carrito.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, int quantity)
        {
            if (quantity <= 0)
                return RedirectToAction(nameof(RemoveFromCart), new { cartItemId });

            var sessionId = GetOrCreateSessionId();
            await UpdateCartItemAsync(sessionId, cartItemId, quantity);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Elimina un ítem específico del carrito.
        /// </summary>
        /// <param name="cartItemId">ID del ítem a eliminar.</param>
        /// <returns>Redirecciona a la vista del carrito.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var sessionId = GetOrCreateSessionId();
            await RemoveCartItemAsync(sessionId, cartItemId);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Elimina todos los productos del carrito actual.
        /// </summary>
        /// <returns>Redirecciona a la vista del carrito.</returns>
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var sessionId = GetOrCreateSessionId();
            await ClearCartAsync(sessionId);

            return RedirectToAction(nameof(Index));
        }

        #region Métodos Privados para Manejo del Carrito

        /// <summary>
        /// Obtiene el ID de sesión del carrito actual a través de cookies.
        /// Si no existe, se crea uno nuevo.
        /// </summary>
        /// <returns>GUID único de sesión.</returns>
        private Guid GetOrCreateSessionId()
        {
            if (Request.Cookies.TryGetValue(SessionCookieName, out string sessionIdStr) &&
                Guid.TryParse(sessionIdStr, out Guid sessionId))
            {
                return sessionId;
            }

            var newSessionId = Guid.NewGuid();
            Response.Cookies.Append(SessionCookieName, newSessionId.ToString(), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps
            });

            return newSessionId;
        }

        /// <summary>
        /// Obtiene el carrito asociado a una sesión. Si no existe, se crea uno nuevo.
        /// </summary>
        /// <param name="sessionId">ID de sesión.</param>
        /// <returns>Instancia del carrito.</returns>
        private async Task<ShoppingCart> GetOrCreateCartAsync(Guid sessionId)
        {
            var cart = await _context.ShoppingCart
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    SessionId = sessionId,
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

                _context.ShoppingCart.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        /// <summary>
        /// Agrega un producto al carrito. Si ya existe en el carrito, aumenta su cantidad.
        /// </summary>
        private async Task AddItemToCartAsync(Guid sessionId, int productId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(sessionId);
            var product = await _context.Productos.FindAsync(productId);

            if (product == null)
                throw new ArgumentException("Producto no encontrado");

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _context.ShoppingCartItem.Update(existingItem);
            }
            else
            {
                var newItem = new ShoppingCartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Precio
                };

                cart.Items.Add(newItem);
            }

            cart.LastModified = DateTime.UtcNow;
            _context.ShoppingCart.Update(cart);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza la cantidad de un ítem en el carrito.
        /// </summary>
        private async Task UpdateCartItemAsync(Guid sessionId, int cartItemId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(sessionId);
            var cartItem = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (cartItem == null)
                throw new ArgumentException("Item de carrito no encontrado");

            cartItem.Quantity = quantity;
            _context.ShoppingCartItem.Update(cartItem);

            cart.LastModified = DateTime.UtcNow;
            _context.ShoppingCart.Update(cart);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina un ítem del carrito según su ID.
        /// </summary>
        private async Task RemoveCartItemAsync(Guid sessionId, int cartItemId)
        {
            var cart = await GetOrCreateCartAsync(sessionId);
            var cartItem = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (cartItem == null)
                throw new ArgumentException("Item de carrito no encontrado");

            _context.ShoppingCartItem.Remove(cartItem);

            cart.LastModified = DateTime.UtcNow;
            _context.ShoppingCart.Update(cart);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina todos los ítems del carrito.
        /// </summary>
        private async Task ClearCartAsync(Guid sessionId)
        {
            var cart = await GetOrCreateCartAsync(sessionId);

            foreach (var item in cart.Items.ToList())
            {
                _context.ShoppingCartItem.Remove(item);
            }

            cart.LastModified = DateTime.UtcNow;
            _context.ShoppingCart.Update(cart);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
