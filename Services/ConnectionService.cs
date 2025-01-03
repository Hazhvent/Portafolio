namespace Portafolio.Services
{
    public class ConnectionService
    {
        //OBJETO DE TIPO CONFIGURACION
        private readonly IConfiguration config;
        //ASIGNA EL ARCHIVO DE CONFIGURACION
        public ConnectionService()
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();
        }
        //OBTIENE LA CONEXION
        public string GetConnection(string db)
        {
            try
            {

                string connectionString = config.GetConnectionString(db);
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("La cadena de conexión no está configurada correctamente.");
                }

                return connectionString;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener la cadena de conexión.", ex);
            }
        }
    }
}