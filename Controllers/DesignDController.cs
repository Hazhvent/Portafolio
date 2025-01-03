using Microsoft.AspNetCore.Mvc;
using Portafolio.Entities;
using Portafolio.Services;

namespace Portafolio.Controllers
{
    public class DesignDController : Controller
    {
        //SERVICIO DE MANTENIMIENTO
        private readonly StorageService _storageService;
        public DesignDController(StorageService storageService)
        {
            _storageService = storageService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Gestionar()
        {
            return View();
        }

        //CONSULTAR PELICULA
        public Pelicula GetMovie(int id)
        {
            return _storageService.BuscarPelicula(id);
        }
       
        //LISTAR PELICULAS
        [HttpGet, ActionName("Read1")]
        public List<Pelicula> Movies()
        {
            return _storageService.ListarPeliculas();
        }
        //LISTAR GENEROS
        [HttpGet, ActionName("Read2")]
        public List<Genero> Genres()
        {
            return _storageService.ListarGeneros();
        }
        //LISTAR CLASIFICACIONES
        [HttpGet, ActionName("Read3")]
        public List<Clasificacion> Class()
        {
            return _storageService.ListarClasificaciones();
        }

        //CAMBIAR ESTADO PELICULA
        [HttpPatch, ActionName("Switch")]
        public bool? Switch(int id)
        {
            return _storageService.CambiarEstado(id);
        }

        //CREAR PELICULA
        [HttpPost, ActionName("Create")]
        public int Crear([FromBody] Pelicula pelicula)
        {
            return _storageService.CrearPelicula(pelicula);
        }

        //ACTUALIZAR PELICULA
        [HttpPut, ActionName("Update")]
        public int Editar([FromBody] Pelicula pelicula)
        {
            return _storageService.EditarPelicula(pelicula);
        }
        //ACTUALIZAR COVER DE PELICULA
        [HttpPost, ActionName("Upload")]
        public bool Upload([FromForm] int id, [FromForm] IFormFile file)
        {
            if (file != null)
            {
                if (_storageService.ActualizarCover(id, file))
                {
                    return true;
                }
                else {
                    return false;
                }              
            }
            return false;
        }
    }
}
