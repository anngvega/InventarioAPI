namespace InventarioAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public int RolId { get; set; }
        public Rol? Rol { get; set; }
    }
}
