using InventarioAPI.Data.Contrato;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventarioAPI.Data
{
    public class MovimientoRepositorio : IMovimientos
    {
        private readonly string cs;
        public MovimientoRepositorio(IConfiguration cfg)
        {
            cs = cfg.GetConnectionString("DB")
                 ?? throw new InvalidOperationException("Falta ConnectionStrings:DB");
        }

        public void RegistrarSalidaVenta(int productoId, int cantidad, int usuarioId, string? motivo = "Venta")
        {
            using var cn = new SqlConnection(cs);
            cn.Open();
            using var cmd = new SqlCommand("RegistrarSalidaVenta", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@ProductoId", productoId);
            cmd.Parameters.AddWithValue("@Cantidad", cantidad);
            cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
            cmd.Parameters.AddWithValue("@Motivo", (object?)motivo ?? "Venta");
            cmd.ExecuteNonQuery();
        }

        public void RegistrarEntrada(int productoId, int cantidad, int usuarioId, string? motivo = "Compra")
        {
            using var cn = new SqlConnection(cs);
            cn.Open();
            using var cmd = new SqlCommand("RegistrarEntrada", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@ProductoId", productoId);
            cmd.Parameters.AddWithValue("@Cantidad", cantidad);
            cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
            cmd.Parameters.AddWithValue("@Motivo", (object?)motivo ?? "Compra");
            cmd.ExecuteNonQuery();
        }
    }
}
