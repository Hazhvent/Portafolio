using Microsoft.AspNetCore.Mvc;
using Portafolio.Dto;
using Portafolio.Dto.Requests;
using Portafolio.Dto.Responses;
using Portafolio.Entities;
using Portafolio.Helpers;
using Portafolio.Services;
using System.Diagnostics;
using System.Text.Json;

namespace Portafolio.Controllers
{
    public class DesignCController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //SERVICIO DE CORREO ELECTRONICO
        private readonly EmailService _mail;
        //SERVICIO DE MENU
        private readonly MenuService _menuService;
        //SERVICIO DE RESERVACION
        private readonly ReservationService _reservacionService;
        public DesignCController(MenuService menuService, EmailService mail, ReservationService reservacionService)
        {
            _menuService = menuService;
            _mail = mail;
            _reservacionService = reservacionService;
        }
        //IMAGENES PARA EL SLIDER
        [HttpGet, ActionName("Pics")]
        public string[] Pics()
        {
            return Gallery.GetDesignCSlider();
        }

        //LISTAR MENU
        [HttpGet, ActionName("Read1")]
        public List<MenuResponse> LeerMenu()
        {
            return _menuService.ListarMenu();
        }

        //ENVIA CODIGO PARA VALIDAR
        [HttpPost, ActionName("Check")]
        public Verificacion Validar(string email)
        {
            return _reservacionService.VerificarCorreo(email);
        }

        //LISTAR HORARIOS
        [HttpGet, ActionName("Read2")]
        public List<Horario> LeerHorarios()
        {
            return _reservacionService.ListarHorarios();
        }

        //HACER RESERVACION
        [HttpPost, ActionName("Create")]
        public bool Crear([FromBody] ReservationRequest reserva) { 
          
            return _reservacionService.Reservar(reserva);
        }

        //BUSCAR RESERVACION
        [HttpGet, ActionName("Search")]
        public ReservationResponse BuscarPorCodigo(string codigo)
        {
            return _reservacionService.BuscarReservacion(codigo);
        }

        //ELIMINAR RESERVACION
        [HttpDelete, ActionName("Delete")]
        public bool Eliminar(string codigo)
        {
            return _reservacionService.EliminarReservacion(codigo);
        }

    }
}
