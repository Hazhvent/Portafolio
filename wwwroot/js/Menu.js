//MENU
document.addEventListener("DOMContentLoaded", function () {
    const menu = document.querySelector(".catalog");
    const categorias = document.querySelectorAll(".categ");
    const next = document.querySelector("#next");
    const back = document.querySelector("#back");
    //VARIABLES GLOBALES
    var category = 1; //CATEGORIA SELECCIONADA (POR DEFECTO = 1)
    var collection; //LISTA DE OBJETOS 
    var items = 6; //NRO DE ITEMS POR PAGINA
    var currentPage = 1; // PAGINA ACTUAL

    //INICIALIZA EL MENU
    function Init() {
        GetMenu();
        Controls();
        SetCategory();
    }
    //REINICIA EL MENU DESPUES DE APLICAR UN FILTRO
    function restart() {
        currentPage = 1;
        Builder();
    }
    //SEÑALA LA CATEGORIA SELECCIONADA
    function SetCategory() {
        // Selecciona la primera categoría por defecto
        categorias[0].classList.add("categ-selected");
        categorias.forEach(item => {
            item.addEventListener('click', function () {
                categorias.forEach(otherItem => {
                    if (otherItem !== item) {
                        otherItem.classList.remove("categ-selected");
                    }
                });
                item.classList.add("categ-selected");
                category = item.getAttribute("data-value");
                restart(); 
            });
        });
    }
    //FILTRA EL MENU POR CATEGORIA
    function FilterMenu() {
        return collection.filter(item => { 
            //BUSCA TODOS LOS ITEMS QUE TENGAN LA CATEGORIA SELECCIONADA
            return item.categoria == category;
        }); 
    }
    //FUNCION DE PAGINACION
    function Pagination() {
        let init = items * (currentPage - 1);
        let end = init + items;
        let section = FilterMenu().slice(init, end);
        let pages = Math.ceil(FilterMenu().length / items);
        return {
            'section': section,
            'pages': pages,
        }
    }
    //CONSTRUYE EL MENU SEGUN CATEGORIA SELECCIONADA
    function Builder() {
        Clear();
        //CONSTRUYE NUEVA SECCION SEGUN LA PAGINA ACTUAL
        let Section = Pagination().section;
        Section.forEach(item => {
            var box = document.createElement("div");
            box.classList.add("catalog__item");
            box.appendChild(GetText(item, 1));
            box.appendChild(GetText(item, 2));
            menu.appendChild(box);
        });
    }
    //FUNCION DE CAMBIO DE PAGINA
    function Controls() {
        //ATRAS
        back.addEventListener('click', function () {
            currentPage = currentPage - 1;
            if (currentPage >= 1) {
                Builder()
            } else {
                currentPage = 1;
            }
        });
        //ADELANTE
        next.addEventListener('click', function () {
            currentPage = currentPage + 1;
            if (currentPage <= Pagination().pages) {
                Builder()
            } else {
                currentPage = Pagination().pages;
            }
        });
    }
    //CONSULTA EL MENU A LA BASE DE DATOS
    function GetMenu() {
        const action = "Read1";
        const url = "DesignC/" + action;
        fetch(url, {
            method: "GET"
        })
            .then(response => {
                return response.json();
            })
            .then(result => {
                collection = result;
                Builder();
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
    }

    //FUNCIONES AUXILIARES
    function GetText(obj, number) {
        let temp = document.createElement("p");
        let clase;
        let atributo;    
        switch (number) {
            case 1: {          
                clase = "catalog__name",
                atributo = obj.nombre
            }
                break;
            case 2: {
                clase = "catalog__price",
                atributo = "S/" +obj.precio
            }
                break;
        }
        temp.classList.add(clase);
        temp.innerHTML = atributo;
        return temp;
    }
    function Clear() {
        menu.innerHTML = "";
    }

    Init();
});