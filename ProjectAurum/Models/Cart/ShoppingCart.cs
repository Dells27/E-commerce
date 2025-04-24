using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProjectAurum.Models.ShoppingCartSing.ShoppingCartItem;

namespace ProjectAurum.Models.Cart.ShoppingCart;

[Table("ShoppingCart")] 
public class ShoppingCart
{
    public int Id { get; set; }

    [Required]
    public Guid SessionId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    public virtual ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

    // Propiedad calculada para el total del carrito
    [NotMapped] // No se almacena en la base de datos
    public decimal Total => Items?.Sum(item => item.Quantity * item.Price) ?? 0;
}
