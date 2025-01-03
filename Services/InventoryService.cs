using Microsoft.Data.SqlClient;
using Portafolio.Dto.Requests;
using Portafolio.Dto.Responses;
using Portafolio.Entities;
using Portafolio.Helpers;
using System.Data;

namespace Portafolio.Services
{
    public class InventoryService
    {
        //TECNOLOGIA DE CONEXION: ADO.NET
        private readonly ConnectionService _connectionService;
        //SERVICIO DE GESTION DE ARCHIVOS
        private readonly FileManagerService _fileManagerService;
        // NOMBRE DE LA LLAVE DE LA CONEXION EN EL ARCHIVO DE CONFIGURACION
        private readonly string database = "DesignA";
        //RUTA DE LA CARPETA DE IMAGENES
        private readonly string Images = "img01/";
        //RUTA DE LA CARPETA DE DOCUMENTOS
        private readonly string Documents = "docs01/";

        public InventoryService(ConnectionService connectionService, FileManagerService fileManagerService)
        {
            _connectionService = connectionService;
            _fileManagerService = fileManagerService;
        }

        
        public int CrearGrafica(GraphicRequest grafica)
        {
            int graphicId;

            using (var conn = new SqlConnection(_connectionService.GetConnection(database)))
            {
                using (var cmd = new SqlCommand("InsertarGrafica", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("MarcaId", grafica.MarcaId);
                    cmd.Parameters.AddWithValue("SerieId", grafica.SerieId);
                    cmd.Parameters.AddWithValue("Modelo", grafica.Modelo);
                    cmd.Parameters.AddWithValue("Vram", grafica.Vram);
                    cmd.Parameters.AddWithValue("Precio", grafica.Precio);

                    try
                    {
                        conn.Open();
                        // "ExecuteScalar()": Devuelve el primer valor (o columna) de la primera fila
                        // Este metodo devuelve un objeto (requiere conversion de tipo de dato)
                        // En este caso el valor debe convertirse a un entero (int) para poder usarse
                        graphicId = (int)cmd.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Error al insertar la gráfica", ex);
                    }
                }
            }
           
            return graphicId;
        }

        public bool ActualizarGrafica(int graphicId, GraphicRequest grafica)
        {
            bool resultado;

            using (var conn = new SqlConnection(_connectionService.GetConnection(database)))
            {
                using (var cmd = new SqlCommand("ActualizarGrafica", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GraphicId", graphicId);
                    cmd.Parameters.AddWithValue("@MarcaId", grafica.MarcaId);
                    cmd.Parameters.AddWithValue("@SerieId", grafica.SerieId);
                    cmd.Parameters.AddWithValue("@Modelo", grafica.Modelo);
                    cmd.Parameters.AddWithValue("@Vram", grafica.Vram);
                    cmd.Parameters.AddWithValue("@Precio", grafica.Precio);

                    try
                    {
                        conn.Open();
                        resultado = (bool)cmd.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Error al actualizar la gráfica", ex);
                    }
                }
            }

            return resultado;
        }

        //CAMBIAR ESTADO (INCLUYE SOFT DELETE)
        public bool CambiarEstado(int graphicId)
        {
            bool resultado;

            using (var conn = new SqlConnection(_connectionService.GetConnection(database)))
            {
                using (var cmd = new SqlCommand("CambiarEstado", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GraphicId", graphicId);

                    try
                    {
                        conn.Open();
                        // Ejecutar el procedimiento y obtener el resultado de la operación
                        resultado = (bool)cmd.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Error al eliminar la gráfica", ex);
                    }
                }
            }

            return resultado;
        }


        public bool InsertarAdjuntos(int graphicId, IFormFile imagen, IFormFile manual)
        {
            bool response = false;

            // Procesar la imagen si no es nula
            if (imagen != null)
            {
                if (!BuildFile(imagen, Images, graphicId))
                {
                    throw new Exception("Error al procesar la imagen.");
                }
                response = true; 
            }

            // Procesar el manual si no es nulo
            if (manual != null)
            {
                if (!BuildFile(manual, Documents, graphicId))
                {
                    throw new Exception("Error al procesar el manual.");
                }
                response = true; 
            }

            return response;
        }


        //CONSTRUYE EL ARCHIVO ADJUNTO
        private bool BuildFile(IFormFile archivo, string folder, int graphicId)
        {
            bool response = false;
            byte tipo = (byte)_fileManagerService.CheckFileTypeExtension(archivo);  

            if (tipo == 0)
            {
                throw new Exception("El archivo debe tener un formato válido.");
            }

            //ELIMINAR ARCHIVO PREVIO
            _fileManagerService.RemoveFile(folder, graphicId);

            //OBTIENE EL MODELO DE LA GRAFICA
            string modelo = GetModel(graphicId);

            //DEFINIR LA RUTA PARA GUARDARLA EN LA BASE DE DATOS
            var DBpath = _fileManagerService.GetDBpath(folder, graphicId, archivo);

            //CREACION DEL ARCHIVO ADJUNTO
                var adjunto = new  AdjuntoRequest
                {
                    GraphicId = graphicId,
                    Tipo = tipo,
                    Nombre = modelo,
                    Ruta = DBpath,
                    Peso = archivo.Length
                };
            //GUARDAR ARCHIVO EN LA BASE DE DATOS
            if (MakeAttached(adjunto)) {
                //SI SE LOGRO GUARDAR EN LA BASE DE DATOS ENTONCES SE GUARDA EL ARCHIVO EN EL PROYECTO
                _fileManagerService.SaveFile(_fileManagerService.GetAPIpath(DBpath), archivo);
                response = true;
            }  
            
            return response;
        }

        //OBTIENE EL MODELO DE UNA GRAFICA
        private string GetModel(int graphicId)
        {
            string modelo;

            using (var conn = new SqlConnection(_connectionService.GetConnection(database)))
            {
                using (var cmd = new SqlCommand("GetModel", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GraphicId", graphicId);

                    try
                    {
                        conn.Open();
                        modelo = (string)cmd.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Error al obtener el modelo de la gráfica", ex);
                    }
                }
            }

            return modelo;
        }

        //CREA EL ARCHIVO ADJUNTO EN LA BASE DE DATOS
        private bool MakeAttached(AdjuntoRequest Adjunto)
        {
            bool response;

            using (var conn = new SqlConnection(_connectionService.GetConnection(database)))
            {
                using (var cmd = new SqlCommand("MakeAttached", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Graphic", Adjunto.GraphicId);
                    cmd.Parameters.AddWithValue("@Tipo", Adjunto.Tipo);
                    cmd.Parameters.AddWithValue("@Nombre", Adjunto.Nombre);
                    cmd.Parameters.AddWithValue("@Ruta", Adjunto.Ruta);
                    cmd.Parameters.AddWithValue("@Peso", Adjunto.Peso);

                    try
                    {
                        conn.Open();
                        response = (bool)cmd.ExecuteScalar();

                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Error al insertar la gráfica", ex);
                    }
                }
            }

            return response;
        }

        //LISTAR GRAFICAS (PAGINACION SIN FILTROS)
        public Pagination<GraphicResponse> ListarGraficas(int page, int itemsPerPage)
        {
            // Inicializar el objeto de paginación
            var pagination = new Pagination<GraphicResponse>
            {
                Items = itemsPerPage < PaginationConstants.Maximum ? itemsPerPage : PaginationConstants.Maximum
            };

            using (var conn = new SqlConnection(_connectionService.GetConnection(database)))
            {
                using (var cmd = new SqlCommand("ListarGraficas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@page", page);
                    cmd.Parameters.AddWithValue("@itemsPerPage", pagination.Items);

                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Leer el primer conjunto de resultados (las gráficas)
                            while (reader.Read())
                            {
                                var graphicId = (int)reader["GraphicId"];

                                // Crear un nuevo objeto GraphicResponse y llenar sus propiedades
                                GraphicResponse graficaExistente = pagination.List.FirstOrDefault(g => g.GraphicId == graphicId);

                                if (graficaExistente == null)
                                {
                                    var nuevaGrafica = new GraphicResponse
                                    {
                                        GraphicId = graphicId,
                                        Marca = (string)reader["Marca"],
                                        Serie = (string)reader["Serie"],
                                        Modelo = (string)reader["Modelo"],
                                        Vram = (byte)reader["Vram"],
                                        Precio = (decimal)reader["Precio"],
                                        Estado = (bool)reader["Estado"],
                                        Adjuntos = new List<AdjuntoResponse>()
                                    };

                                    // SE CONVIERTE A TEXTO TODOS LOS ADJUNTOS DE LA GRAFICA
                                    var adjuntos = reader["Adjuntos"].ToString();
                                    if (!string.IsNullOrEmpty(adjuntos))
                                    {
                                        // SE DESCONCATENA A PARTIR DE UNA COMA CADA ADJUNTO 
                                        // PARA ESTE CASO SON 2: IMAGEN Y MANUAL PARA CADA GRAFICA (PODRIA AÑADIRSE GARANTIA POR EJEMPLO)
                                        foreach (var adjunto in adjuntos.Split(','))
                                        {
                                            // SE DESCONCATENA CADA PARTE DEL ADJUNTO A PARTIR DE LOS 2 PUNTOS
                                            var parts = adjunto.Split(':');
                                            // EN ESTE CASO SOLO SON 2 PARTES: "Id" y "Ruta"
                                            if (parts.Length == 2)
                                            {
                                                // SE PODRIA AGREGAR FECHA Y PESO DE MODO QUE LAS PARTES SERIAN 4, ENTOCES "parts.Length == 4"
                                                nuevaGrafica.Adjuntos.Add(new AdjuntoResponse
                                                {
                                                    Id = int.Parse(parts[0]),
                                                    Ruta = parts[1]
                                                    // Fecha = parts[2],
                                                    // Peso = parts[3]
                                                });
                                            }
                                        }
                                    }

                                    pagination.List.Add(nuevaGrafica);
                                }
                            }

                            // Avanzar al siguiente conjunto de resultados (total de registros)
                            if (reader.NextResult() && reader.Read())
                            {
                                // Asignar el total de registros y calcular el número de páginas
                                pagination.Total = (int)reader["TotalRecords"];
                                pagination.Pages = PaginationConstants.Pages(pagination.Total, itemsPerPage);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Error al listar las gráficas", ex);
                    }
                }
            }

            return pagination;
        }


    }
}
