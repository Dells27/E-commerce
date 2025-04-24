using Microsoft.EntityFrameworkCore;
using ProjectAurum.Models.Cart.ShoppingCart;
using ProjectAurum.Models.Category;
using ProjectAurum.Models.Product;
using ProjectAurum.Models.ShoppingCartSing.ShoppingCartItem;
using ProjectAurum.Models.User;

namespace ProjectAurum.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        // DbSet representa una tabla en la db
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Categorías> Categorías { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItem { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para ShoppingCart
            // Se crea un índice único sobre la columna SessionId (cada carrito se identifica por sesión)
            modelBuilder.Entity<ShoppingCart>()
                .HasIndex(c => c.SessionId)
                .IsUnique();


            // Configuración para ShoppingCartItem
            // Relación uno a muchos: un carrito puede tener muchos ítems
            modelBuilder.Entity<ShoppingCartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina un carrito, se eliminan sus ítems

            // Relación entre ítems y productos:
            // Cada ítem está relacionado con un producto
            modelBuilder.Entity<ShoppingCartItem>()
                .HasOne(ci => ci.Product)
                .WithMany() // No se define una colección de ítems en la entidad Producto
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // No se permite eliminar un producto si tiene ítems relacionados
        }
    }
}
