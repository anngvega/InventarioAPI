using InventarioAPI.Data.Contrato;
using InventarioAPI.Models;
using Microsoft.Data.SqlClient;

namespace InventarioAPI.Data
{
    public class ProductoRepositorio : IProducto
    {
        private readonly IConfiguration _config;
        private readonly string cadenaConexion;

        public ProductoRepositorio(IConfiguration config)
        {
            _config = config;
            cadenaConexion = _config.GetConnectionString("DB");
        }

        public Producto Actualizar(Producto producto)
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("ActualizarProducto", conexion))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", producto.Id);
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);
                    command.Parameters.AddWithValue("@PrecioCosto", producto.PrecioCosto);
                    command.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                    command.Parameters.AddWithValue("@FechaVencimiento", producto.FechaVencimiento);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ConvertirReaderEnObjeto(reader);
                        }
                    }
                }
            }
            return ObtenerPorID(producto.Id);
        }

        public bool Eliminar(int id)
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var command = new SqlCommand("EliminarProducto", conexion))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var filas = reader.GetInt32(reader.GetOrdinal("FilasAfectadas"));
                            return filas > 0;
                        }
                    }
                }
            }
            return false;
        }

        public List<Producto> Listado()
        {
            var lista = new List<Producto>();

            using var conexion = new SqlConnection(cadenaConexion);
            {
                conexion.Open();
                using (var comando = new SqlCommand("ListarProductos", conexion))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(ConvertirReaderEnObjeto(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public Producto ObtenerPorID(int id)
        {
            Producto producto = null;
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("ObtenerProductoPorID", conexion))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@ID", id);
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector != null && lector.HasRows)
                        {
                            lector.Read();
                            producto = ConvertirReaderEnObjeto(lector);

                        }
                    }
                }
            }
            return producto;
        }

        public Producto Registrar(Producto producto)
        {
            Producto nuevoProducto = null;
            int nuevoID = 0;

            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("RegistrarProducto", conexion))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@nombre", producto.Nombre);
                    comando.Parameters.AddWithValue("@Stock", producto.Stock);
                    comando.Parameters.AddWithValue("@PrecioCosto", producto.PrecioCosto);
                    comando.Parameters.AddWithValue("@PrecioVenta", producto.PrecioVenta);
                    comando.Parameters.AddWithValue("@FechaVencimiento", producto.FechaVencimiento);
                    var outputIdParam = new SqlParameter("@NuevoID", System.Data.SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    comando.Parameters.Add(outputIdParam);
                    comando.ExecuteNonQuery();
                    nuevoID = (int)outputIdParam.Value;
                }
            }

            nuevoProducto = ObtenerPorID(nuevoID);
            return nuevoProducto;
        }

        #region . METODOS PRIVADOS .
        private Producto ConvertirReaderEnObjeto(SqlDataReader reader)
        {
            return new Producto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                PrecioCosto = reader.GetDecimal(reader.GetOrdinal("PrecioCosto")),
                PrecioVenta = reader.GetDecimal(reader.GetOrdinal("PrecioVenta")),
                FechaVencimiento = reader.GetDateTime(reader.GetOrdinal("FechaVencimiento")),
                Estado = reader.GetString(reader.GetOrdinal("Estado"))
            };
        }
        #endregion

    }
}




