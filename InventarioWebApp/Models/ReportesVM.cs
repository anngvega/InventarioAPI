namespace InventarioWebApp.Models
{
    public class VentaDetalleVM
    {
        public DateTime Fecha { get; set; }
        public string Codigo { get; set; } = "";
        public string Producto { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal Importe { get; set; }
        public decimal Costo { get; set; }
        public decimal Ganancia { get; set; }
        public string Usuario { get; set; } = "";
    }

    public class VentaResumenDiaVM
    {
        public DateTime Dia { get; set; }
        public int CantidadTotal { get; set; }
        public decimal ImporteTotal { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal GananciaTotal { get; set; }
    }
}