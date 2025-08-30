using InventarioAPI.Models;

namespace InventarioAPI.Data.Contrato
{
    public interface IProducto
    {
        List<Producto> Listado();
        Producto ObtenerPorID(int id);
        Producto Registrar(Producto producto);
        Producto Actualizar(Producto producto);
        bool Eliminar(int id);
    }
}
