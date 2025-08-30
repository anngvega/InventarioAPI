namespace InventarioAPI.Models
{
    public class MovimientoStock
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int Cantidad { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }
}
