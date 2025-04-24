using System.ComponentModel.DataAnnotations;

namespace ProjectAurum.Models.User
{
    public class Usuarios
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [EmailAddress]
        [Required]
        public string Correo { get; set; }

        [Required]
        public string Contraseña { get; set; }

        public string Rol { get; set; }
    }
}
