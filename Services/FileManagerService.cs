using System.Diagnostics;
using System.IO;
using System.Net.Mail;

namespace Portafolio.Services
{
    public class FileManagerService
    {
        //OBJETO DE TIPO CONFIGURACION
        private readonly IConfiguration _configuration;
        //RUTA DE LA CARPETA DEL FRONT DEL PROYECTO
        private readonly string project = "wwwroot";
        //RUTA DE LA CARPETA DE ADJUNTOS
        private readonly string attached = "/attached/";
        public FileManagerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //GUARDAR ARCHIVO
        public void SaveFile(string path, IFormFile file)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }
        //ELMINAR ARCHIVO
        private void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        //OBTIENE LA EXTENSION DEL ARCHIVO
        private string GetFileExtension(IFormFile file) {
            return System.IO.Path.GetExtension(file.FileName);
        }

        //RETORNA LA RUTA PARA GUARDAR EL ARCHIVO EN LA BASE DE DATOS
        public string GetDBpath(string folder, int id, IFormFile file)
        {
            return attached + folder + id + GetFileExtension(file);
        }

        //RETORNA LA RUTA PARA GUARDAR EL ARCHIVO EN EL PROYECTO
        public string GetAPIpath(string path)
        {
            return project + path;
        }

        //VERIFICA EL TIPO DE EXTENSIÓN DEL ARCHIVO
        public int CheckFileTypeExtension(IFormFile file)
        {
            string fileExtension = GetFileExtension(file);

            Debug.WriteLine($"Verificando la extensión del archivo: {fileExtension}");

            if (!string.IsNullOrEmpty(fileExtension))
            {
                try
                {
                    //OBTIENE TODAS LAS CATEGORIAS DE EXTENSIONES PERMITIDAS
                    var allowedExtensionsSection = _configuration.GetSection("AllowedExtensions").GetChildren().ToArray();

                    //BUSCA EN CADA CATEGORIA
                    foreach (var section in allowedExtensionsSection)
                    {
                        string categoryName = section.Key; //NOMBRE DE LA CATEGORIA (Images, Documents, etc.)
                        string[] extensions = section.Get<string[]>(); //OBTIENE LAS EXTENSIONES CONTENIDAS EN LA CATEGORIA

                        Debug.WriteLine($"Verificando en la categoría: {categoryName}");

                        //COMPARA LA EXTENSION DEL ARCHIVO CON CADA EXTENSION DE CADA CATEGORIA: extensions
                        if (extensions != null && extensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                        {
                            Debug.WriteLine($"Extensión encontrada en la categoría: {categoryName}");

                            return categoryName switch
                            {
                                "Images" => 1,    
                                "Documents" => 2,  
                                "Videos" => 3,     
                                //"Otro" => 4, 
                                _ => 0 //CATEGORIA NO VALIDA
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error al verificar la extensión del archivo: {ex.Message}");
                }
            }

            Debug.WriteLine("Extensión no válida o no encontrada en ninguna categoría.");
            return 0;
        }

        // OBTIENE UNA LISTA DE TODAS LAS EXTENSIONES PERMITIDAS
        private string[] GetAllExtensions()
        {
            var allowedExtensions = _configuration.GetSection("AllowedExtensions");
            //LISTA TOTAL DE EXTENSIONES
            var allExtensions = new List<string>();

            //BUSCA TODAS LAS EXTENSIONES POR CATEGORIA
            foreach (var category in allowedExtensions.GetChildren())
            {
                var extensions = category.Get<string[]>();
                if (extensions != null)
                {
                    //SI LA CATEGORIA CONTIENE EXTENSIONES LA AÑADE A LA LISTA TOTAL DE EXTENSIONES
                    allExtensions.AddRange(extensions);
                }
            }
            return allExtensions.ToArray();
        }

        // VERIFICA LA EXISTENCIA DEL ARCHIVO
        private string SearchFile(string path)
        {
            foreach (string extension in GetAllExtensions())
            {
                var temp = path + extension;
                if (File.Exists(temp))
                {
                    return temp;
                }
            }

            return null;
        }

        //RETORNA LA RUTA ABSOLUTA DEL ARCHIVO EN EL PROYECTO
        private string AbsolutePath(string folder, int id) {

            //RUTA RELATIVA: wwwroot/attached/folder/id
            string RelativePath = project + attached + folder + id;
            //RUTA ABSOLUTA: wwwroot/attached/folder/id.extension
            return SearchFile(RelativePath);
        }


        //ELIMINA ARCHIVOS DEL PROYECTO
        public void RemoveFile(string folder, int id)
        {
            string path = AbsolutePath(folder, id);

            if (path != null)
            {
                DeleteFile(path);
            }
        }
    }
}
