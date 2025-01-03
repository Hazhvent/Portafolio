using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Portafolio.Helpers;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Portafolio.Dto;
using Portafolio.Dto.Responses;

namespace Portafolio.Services
{   //TECNOLOGIA DE CONEXION: DAPPER
    //OBSERVACION: LOS NOMBRES DE LOS ATRIBUTOS DEL MODELO DEBEN COINCIDIR CON LOS CAMPOS EN LA BASE DE DATOS
    public class ReservationService
    {   //SERVICIO DE CONEXION
        private readonly ConnectionService _connectionService;
        //SERVICIO DE CORREO ELECTRONICO
        private readonly EmailService _mail;
        //NOMBRE DE LA LLAVE DE LA CONEXION EN EL ARCHIVO DE CONFIGURACION
        private readonly string database = "DesignC";

        public ReservationService(ConnectionService connectionService, EmailService mail)
        {
            _connectionService = connectionService;
            _mail = mail;
        }

        public List<Horario> ListarHorarios()
        {
            using var conn = new SqlConnection(_connectionService.GetConnection(database));
            var horarios = conn.Query<Horario>("LISTAR_HORARIOS", commandType: CommandType.StoredProcedure).ToList();
            return horarios;
        }

        public Verificacion VerificarCorreo(string email)
        {
            Verificacion temp = new Verificacion();
            string code = "";
            using var conn = new SqlConnection(_connectionService.GetConnection(database));
            var parameters = new { CORREO = email };
            //"QueryFirstOrDefault<>()": Devuelve el primer valor (o columna) de la primera fila
            //*Dentro de <> especifico el tipo de dato a recibir, en este caso: bool. Es decir <bool>
            var result = conn.QueryFirstOrDefault<bool>("EMAIL_VALIDATION", parameters, commandType: CommandType.StoredProcedure);
            if (result)
            {
                code = _mail.SendCode(email, "CoffeTime");          
            }
            temp.Valido = result;
            temp.Codigo = code;
            return temp;
        }

        public bool Reservar(Reservacion reserva)
        {
            string code = RandomGenerator.SetCode(6);
            reserva.Codigo = code;

            using var conn = new SqlConnection(_connectionService.GetConnection(database));
            var parameters = new
            {
                NOMBRE = reserva.Nombre, 
                APELLIDO = reserva.Apellido, 
                CELULAR = reserva.Celular, 
                CORREO = reserva.Correo, 
                HORA = reserva.Hora, 
                CODIGO = reserva.Codigo
            };
            var result = conn.QueryFirstOrDefault<bool>("MAKE_RESERVATION", parameters, commandType: CommandType.StoredProcedure);
            if (result)
            {
                _mail.SendReservation(reserva.Correo, "CoffeTime", code);
            }                      
            return result;        
        }

        public ReservationResponse BuscarReservacion(string code)
        {
            using var conn = new SqlConnection(_connectionService.GetConnection(database));
            var parameters = new { CODIGO = code };
            var result = conn.QueryFirstOrDefault<ReservationResponse>("SEARCH_RESERVATION", parameters, commandType: CommandType.StoredProcedure);
            return result;
        }

        public bool EliminarReservacion(string code)
        {
            using var conn = new SqlConnection(_connectionService.GetConnection(database));
            var parameters = new { CODE = code };
            bool success = false;
            //"QueryFirstOrDefault<>()": Devuelve el primer valor (o columna) de la primera fila
            //"<dynamic>": Indica que puede ser un objeto, es decir, puede haber 2 o mas columnas
            //*Podria considerarse la idea de mapear la respuesta del procedure para tener un codigo mas seguro
            var result = conn.QueryFirstOrDefault<dynamic>("DELETE_RESERVATION", parameters, commandType: CommandType.StoredProcedure);
            if (result != null) {
                success = result.RESPONSE;
                if (success)
                {
                    _mail.SendDeleteReservation(result.CORREO, "CoffeTime");
                }
            }         
            return success;
        }

    }
}
