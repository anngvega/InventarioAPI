using InventarioAPI.Models;

namespace InventarioAPI.Data.Contrato
{
    public interface IUsuario
    {
        Task<Usuario?> LoginAsync(string nombreUsuario, string clave);
    }
}
