using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

//PORTAFOLIO VERSION 1.0
namespace Portafolio.Controllers
{
    public class HomeController : Controller
    {   
        public IActionResult Index()
        {
            return View();
        }
        public void AddProject(List<ExpandoObject> list,string modelo, string app, string contexto, int pages, string enlace) {
            dynamic obj = new ExpandoObject();
            obj.modelo = modelo;
            obj.app = app;
            obj.context = contexto;
            obj.pages = pages;
            obj.enlace = enlace;

            list.Add(obj);
        }

        [HttpGet, ActionName("Projects")]
        public List<ExpandoObject> Projects() {
            List<ExpandoObject> temp = new();
            AddProject(temp, "MovieStorage", "Mantenimiento", "Inventario", 2, "DesignA");
            AddProject(temp, "RefugioCitadino", "Reseñas", "Hospedaje", 1, "DesignB");
            AddProject(temp, "CoffeTime", "Reservaciones", "Alimentacion", 1, "DesignC");       
            return temp;
        }

        //VERSION 1.2
        //DISEÑO A: CREAR EL FRONTEND (USAR BOOTSTRAP)

        //VERSION 1.3
        //DISEÑO D: HACER LA PAGINACION CON FILTROS DESDE EL BACKEND

        //VERSION 1.4
        //DISEÑO E: UN SITIO WEB PARA CINE (CARTELERA) USAR LA PAGINACION A NIVEL DE FRONT END
        //CREAR EL BACKEND CON ENTITY FRAMEWORK
    }
}