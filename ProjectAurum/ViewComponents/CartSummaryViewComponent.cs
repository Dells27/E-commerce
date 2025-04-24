using Microsoft.AspNetCore.Mvc;
using ProjectAurum.Data;
using ProjectAurum.Models.Cart.ShoppingCart;
using ProjectAurum.Models.ShoppingCartSing.ShoppingCartItem;
using System;
using System.Linq;

public class CartSummaryViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public CartSummaryViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        // Obtener el SessionId desde la cookie
        var sessionId = GetSessionId();

        // Cargar el carrito desde la base de datos
        var cart = _context.ShoppingCart
            .Where(c => c.SessionId == sessionId)
            .Select(c => new ShoppingCart
            {
                Id = c.Id,
                SessionId = c.SessionId,
                Items = c.Items.Select(i => new ShoppingCartItem
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),
                CreatedAt = c.CreatedAt,
                LastModified = c.LastModified
            })
            .FirstOrDefault();

        // Si no hay carrito, crear uno vacío
        if (cart == null)
        {
            cart = new ShoppingCart
            {
                SessionId = sessionId,
                Items = new System.Collections.Generic.List<ShoppingCartItem>()
            };
        }

        return View("_CartSummaryPartial", cart);
    }

    private Guid GetSessionId()
    {
        // Intentar obtener el SessionId desde la cookie
        if (HttpContext.Request.Cookies.TryGetValue("ShoppingCartSession", out string sessionIdStr) &&
            Guid.TryParse(sessionIdStr, out Guid sessionId))
        {
            return sessionId;
        }

        // Si no existe, generar uno nuevo
        var newSessionId = Guid.NewGuid();
        HttpContext.Response.Cookies.Append("ShoppingCartSession", newSessionId.ToString(), new Microsoft.AspNetCore.Http.CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(30),
            HttpOnly = true,
            IsEssential = true
        });

        return newSessionId;
    }
}