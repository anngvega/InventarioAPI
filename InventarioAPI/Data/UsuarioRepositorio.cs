using InventarioAPI.Data.Contrato;
using InventarioAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventarioAPI.Data
{
    public class UsuarioRepositorio : IUsuario
    {
        private readonly string cadenaConexion;

        public UsuarioRepositorio(IConfiguration config)
        {
            cadenaConexion = config.GetConnectionString("DB");
        }

        public async Task<Usuario?> LoginAsync(string nombreUsuario, string clave)
        {
            return await Task.Run(() =>
            {
                using var conexion = new SqlConnection(cadenaConexion);
                conexion.Open();

                using var comando = new SqlCommand("LoginUsuario", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                comando.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                comando.Parameters.AddWithValue("@Clave", clave);

                using var lector = comando.ExecuteReader();

                if (lector.Read())
                {
                    return new Usuario
                    {
                        Id = lector.GetInt32(0),
                        Nombres = lector.GetString(1),
                        Apellidos = lector.GetString(2),
                        NombreUsuario = lector.GetString(3),
                        Rol = new Rol { Nombre = lector.GetString(4) }
                    };
                }

                return null;
            });
        }
    }
}
