document.addEventListener("DOMContentLoaded", function () {
//ELEMENTOS
const reset = document.querySelector("#reset");
const results = document.querySelector("#results");
const message = document.querySelector("#message");
const filters = [...document.querySelectorAll('.filter')];
const generos = [...document.querySelectorAll('[name="genre"]')];
const versiones = [...document.querySelectorAll('[name="version"]')];
const clases = [...document.querySelectorAll('[name="class"]')];
const block = document.querySelector(".block");
const front = document.querySelector("#front");
const back = document.querySelector("#back");
const nro = document.querySelector("#page");
const find = document.querySelector("#find");
//VARIABLES GLOBALES
var collection; //LISTA DE OBJETOS
var items = 6; //NRO DE ITEMS POR PAGINA
var currentPage = 1; // PAGINA ACTUAL

Init();

    //CONSULTA LA LISTA DE PELICULAS
    function GetMovies() {
        const action = "Read1";
        const url = "DesignD/" + action;
        fetch(url, {
            method: "GET"
        })
            .then(response => {
                return response.json();
            })
            .then(result => {
                collection = result;
                Builder();
                Controls();
                Search();
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
    }

//INICIALIZA LA TIENDA
function Init(){
  GetMovies();
}

//REINICIA LA TIENDA DESPUES DE APLICAR UN FILTRO
    function restart() {
  
      currentPage = 1;
      nro.value = 1;
      Builder();
      nro.max = Pagination().pages;
      Results(ApplyFilters(), nro.max); 
}

    // EVENTO QUE APLICA LOS FILTROS
    filters.forEach(filter => {
        filter.addEventListener('change', function () {
            restart();
        });
    });

//EVENTO QUE DESHACE LOS FILTROS Y REESTABLECE LA LISTA ORIGINAL
    reset.addEventListener('click', function () {
        filters.forEach(filter => filter.checked = false);
        restart();
});
//MUESTRA LOS RESULTADOS DE LA BUSQUEDA
function Results(elements,total){
  message.innerHTML = "";
  let temp = document.createElement("ul");
          temp.classList.add("object__list"); 
                let item1 = document.createElement("li");
                item1.classList.add("object__item");
                item1.innerHTML = "Se encontraron: " + elements.length +" elementos.";
                let item2 = document.createElement("li");
                item2.classList.add("object__item");
                item2.innerHTML = "Hay " + total + " página(s) disponible(s).";
            temp.appendChild(item1);
            temp.appendChild(item2);
  message.appendChild(temp);
  results.appendChild(message);
  results.style.display = "block";
  results.scrollIntoView({ behavior: "smooth", block: "start" });
}

//ESTABLECE LOS FILTROS
function ApplyFilters(){
  //TIPOS DE FILTROS
  let genres = GetFilters(generos);
  let versions = GetFilters(versiones);
  let clasification = GetFilters(clases);

//INTERSECCION DE LOS FILTROS SELECCIONADOS
    return collection.filter(item => {
        //DE NO HABER FILTRO SELECCIONADO ESTO RETORNARA UNA LONGITUD CON VALOR 0 DE MODO QUE SE INCLUYEN TODOS LOS JUEGOS (filtro.length == 0)
        return ((genres.length == 0 || genres.includes(item.genero)) &&
            (versions.length == 0 || (versions.includes(item.version))) &&
                (clasification.length == 0 || clasification.includes(item.clasificacion)))
    });       

}

//RECOJE TODOS LOS VALORES DEL TIPO DE FILTRO SELECCIONADO
function GetFilters(Type){
  let Selected = [];
  Type.forEach(function(ref) {
    if(ref.checked){
      Selected.push(Number(ref.value));
    }   
  });
  return Selected;
}

//FUNCION DE PAGINACION
function Pagination(){ 
  let init = items*(currentPage - 1);
  let end = init + items;
  let section = ApplyFilters().slice(init, end);
  let pages = Math.ceil(ApplyFilters().length / items);
  return {
    'section': section,
    'pages': pages,
  }
}
    
//CONSTRUYE LA TIENDA SEGUN FILTROS APLICADOS
function Builder(){
    //ELIMINA LA SECCION ANTERIOR
    Clear();
    //CONSTRUYE NUEVA SECCION SEGUN LA PAGINA ACTUAL
    let Section = Pagination().section;
    Section.forEach(item => {
      let obj = document.createElement("div");
        obj.classList.add("object");
        obj.classList.add("fake-margin");
        obj.appendChild(GetName(item));
        obj.appendChild(GetImage(item));
        obj.appendChild(GetLink(item));
        block.appendChild(obj);
    });    
}

//FUNCION DE BUSQUEDA DE PAGINA
function Search(){
  nro.max = Pagination().pages;
  find.addEventListener('click', function(){
    if(nro.value > 0 && nro.value <= nro.max){
      currentPage = Number(nro.value); 
      Builder(); 
    }else{
      alert("ingrese un nro valido!");
    }        
	});	
}

//FUNCION DE CAMBIO DE PAGINA
function Controls(){
    //ATRAS
    back.addEventListener('click', function(){
        currentPage = currentPage - 1; 
        if(currentPage >= 1){
            Builder();
        }else{
          currentPage = 1;
        }
        nro.value = currentPage;
      });
    //ADELANTE
    front.addEventListener('click', function(){
      currentPage = currentPage + 1; 
      if(currentPage <= Pagination().pages){
          Builder();
      }else{
        currentPage = Pagination().pages;
      } 
      nro.value = currentPage;
    });      		
}

//FUNCIONES AUXILIARES
    function GetImage(obj) {
        let temp = document.createElement("div");
        temp.classList.add("object__img");
        temp.style.backgroundImage = "url('" + (obj.path) + "')";
        return temp;
    }

    function GetName(obj){
      let temp = document.createElement("h2");
          temp.classList.add("object__name");
          temp.innerHTML = obj.nombre;
      return temp;
    }

    function GetLink(obj){
      let temp = document.createElement("a");
          temp.classList.add("object__link");
          temp.href = "DesignD/Gestionar?id=" + obj.id;
          temp.innerHTML = "Gestionar";
      return temp; 
    }

    function Clear(){
      block.innerHTML = "";
    }
});