namespace InventarioAPI.Data.Contrato
{
    public interface IMovimientos
    {
        void RegistrarSalidaVenta(int productoId, int cantidad, int usuarioId, string? motivo = "Venta");
        void RegistrarEntrada(int productoId, int cantidad, int usuarioId, string? motivo = "Compra");
    }
}
