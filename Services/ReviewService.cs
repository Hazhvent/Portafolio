using Microsoft.Data.SqlClient;
using Portafolio.Dto.Requests;
using Portafolio.Dto.Responses;
using System.Data;
using System.Text.Json;

namespace Portafolio.Services
{
    //TECNOLOGIA DE CONEXION: ADO.NET
    public class ReviewService
    {   //SERVICIO DE CONEXION
        private readonly ConnectionService _connectionService;
        //NOMBRE DE LA LLAVE DE LA CONEXION EN EL ARCHIVO DE CONFIGURACION
        private readonly string database = "DesignB";

        public ReviewService(ConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        public List<ReviewResponse> ListarReseñas()
        {
            var temp = new List<ReviewResponse>();
            using (var conn = new SqlConnection(_connectionService.GetConnection(database)))
            using (var cmd = new SqlCommand("LISTAR_RESEÑAS", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    conn.Open();
                    using var puntero = cmd.ExecuteReader();
                    while (puntero.Read())
                    {
                        var review = new ReviewResponse
                        {
                            Nombres = puntero.GetString(0),
                            Reseña = puntero.GetString(1),
                            Puntuacion = puntero.GetByte(2)
                        };
                        temp.Add(review);
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al ejecutar la consulta SQL", ex);
                }
            }
            return temp;
        }
        public int GenerarReseña(ReviewRequest reseña)
        {
            using var conn = new SqlConnection(_connectionService.GetConnection(database));
            using var cmd = new SqlCommand("MAKE_REVIEW", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("DNI", reseña.Dni);
            cmd.Parameters.AddWithValue("RESEÑA", reseña.Reseña);
            cmd.Parameters.AddWithValue("PUNTOS", reseña.Puntuacion);

            try
            {
                conn.Open();
                //"ExecuteScalar()": Devuelve el primer valor (o columna) de la primera fila
                //Este metodo devuelve un objeto (requiere conversion de tipo de dato)
                //*En este caso el valor debe convertirse a un entero (int) para poder usarse
                return (int)cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al generar reseña", ex);
            }
        }

        public string AutoGenerarCliente()
        {
            using var conn = new SqlConnection(_connectionService.GetConnection(database));
            using var cmd = new SqlCommand("AUTOCLIENT", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();
                return (string)cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al generar cliente autogenerado", ex);
            }
        }

    }
}
