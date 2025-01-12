using Microsoft.AspNetCore.Mvc;
using Portafolio.Dto.Requests;
using Portafolio.Dto.Responses;
using Portafolio.Entities;
using Portafolio.Services;

namespace Portafolio.Controllers
{
    public class DesignDController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //SERVICIO DE INVENTARIO
        private readonly InventoryService _inventoryService;
        public DesignDController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        //LISTAR GRAFICAS (INCLUYE PAGINACION)
 
        [HttpGet, ActionName("Read")]
        public Pagination<GraphicResponse> Leer([FromForm] int page, [FromForm] int itemsPerPage)
        {
            return _inventoryService.ListarGraficas(page, itemsPerPage);
        }
           
        //CREAR GRAFICA
        [HttpPost, ActionName("Create")]
        public int Crear([FromBody] GraphicRequest grafica)
        {
            return _inventoryService.CrearGrafica(grafica);
        }

        //ACTUALIZAR GRAFICA
        [HttpPut, ActionName("Update")]
        public bool Actualizar(int id, [FromBody] GraphicRequest grafica)
        {
            return _inventoryService.ActualizarGrafica(id, grafica);
        }

        //CAMBIAR ESTADO GRAFICA
        [HttpPatch, ActionName("Switch")]
        public bool Switch(int id)
        {
            return _inventoryService.CambiarEstado(id);
        }

        //INSERTAR ADJUNTOS
        [HttpPost, ActionName("Attached")]
        public bool Adjuntos([FromForm] int graphicId, IFormFile imagen, IFormFile documento)
        {
            return _inventoryService.InsertarAdjuntos(graphicId, imagen, documento);
        }

        //CAMBIAR ESTADO GRAFICA
        [HttpGet, ActionName("AddUnit")]
        public string AddUnit(int id)
        {
            return _inventoryService.InsertarUnidad(id);
        }


    }
}
