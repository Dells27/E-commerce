namespace ProjectAurum.Models.Product
{
    public class Productos
    {

        public int Id { get; set; }

        public string? Nombre { get; set; }

        public string? Descripción { get; set; }

        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public string? Imagen { get; set; }

        public int? CategoríaID { get; set; }

        public int? Año { get; set; }



    }
}
