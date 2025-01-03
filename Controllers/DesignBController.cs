using Microsoft.AspNetCore.Mvc;
using Portafolio.Services;
using Portafolio.Helpers;
using System.Text.Json;
using Portafolio.Dto.Responses;
using Portafolio.Dto.Requests;

namespace Portafolio.Controllers
{
    public class DesignBController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //SERVICIO DE RESEÑAS
        private readonly ReviewService _reviewService;
        public DesignBController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        //IMAGENES PARA LA GALERIA
        [HttpGet, ActionName("Pics")]
        public string[] Pics()
        {
            return Gallery.GetDesignBGallery();
        }
        //LISTAR RESEÑAS
        [HttpGet, ActionName("Read")]
        public List<ReviewResponse> Leer()
        {
            return _reviewService.ListarReseñas();
        }
        //HACER RESEÑA
        [HttpPost, ActionName("Make")]
        public int Hacer([FromBody] ReviewRequest reseña)
        {
            return _reviewService.GenerarReseña(reseña);
        }

        //AUTOGENERAR CLIENTE
        [HttpPost, ActionName("Autogen")]
        public string GenerarCliente()
        {           
          return _reviewService.AutoGenerarCliente();        
        }
    }
}
