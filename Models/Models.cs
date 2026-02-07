namespace HomeVideojuegos.Models
{
    public class Usuario
    {
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }    // "Admin" o "Nominal"
        public string Estado { get; set; } // "Activo" o "Baneado"
    }
}