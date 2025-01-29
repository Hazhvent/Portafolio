using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Portafolio.Dto.Responses;

namespace Portafolio.Services
{
    //TECNOLOGIA DE CONEXION: DAPPER
    //OBSERVACION: LOS NOMBRES DE LOS ATRIBUTOS DEL MODELO DEBEN COINCIDIR CON LOS CAMPOS EN LA BASE DE DATOS
    public class MenuService
    {   //SERVICIO DE CONEXION
        private readonly ConnectionService _connectionService;
        //NOMBRE DE LA LLAVE DE LA CONEXION EN EL ARCHIVO DE CONFIGURACION
        private readonly string database = "DesignC";

        public MenuService(ConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public List<MenuResponse> ListarMenu()
        {
            using var conn = new SqlConnection(_connectionService.GetConnection(database));
            var menuItems = conn.Query<MenuResponse>("LISTAR_MENU", commandType: CommandType.StoredProcedure).ToList();
            return menuItems;
        }
    }
}
