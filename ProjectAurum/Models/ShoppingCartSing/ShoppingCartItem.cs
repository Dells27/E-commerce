using ProjectAurum.Models.Cart.ShoppingCart;
using ProjectAurum.Models.Product;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectAurum.Models.ShoppingCartSing.ShoppingCartItem;

[Table("ShoppingCartItem")] 
public class ShoppingCartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    [ForeignKey("CartId")]
    public virtual ShoppingCart Cart { get; set; }

    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public virtual Productos Product { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
    public decimal Price { get; set; }
}
