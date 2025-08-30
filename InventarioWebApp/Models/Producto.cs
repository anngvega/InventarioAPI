namespace InventarioWebApp.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }
        public DateTime FechaVencimiento { get; set; } = DateTime.Now;
        public int EstadoProductoId { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
