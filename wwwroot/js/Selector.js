//SELECTOR DE PROYECTO
document.addEventListener("DOMContentLoaded", function () {
    const modelos = document.querySelector("#list");
    const app = document.querySelector("#app");
    const context = document.querySelector("#context");
    const nro = document.querySelector("#num");
    const enlace = document.querySelector("#go");

    function GetProjects() {
        fetch("Home/Projects")
            .then(response => response.json())
            .then(result => {
                PopulateProjectList(result);
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
    }

    function PopulateProjectList(projects) {
        modelos.innerHTML = ""; // Limpiar la lista antes de agregar opciones
        projects.forEach((project, index) => {
            let option = new Option(project.modelo, index);
            modelos.appendChild(option);
        });

        modelos.addEventListener("change", function () {
            const selectedIndex = modelos.value;
            DisplayProjectDetails(projects[selectedIndex]);
        });

        // Mostrar detalles del primer proyecto al cargar la página
        DisplayProjectDetails(projects[0]);
    }

    function DisplayProjectDetails(project) {
        app.textContent = project.app;
        context.textContent = project.context;
        nro.textContent = project.pages;
        enlace.href = project.enlace;
        //Evento que redirecciona al usuario según el sitio web elegido
        enlace.addEventListener("click", function () {
            window.location.href = project.enlace;
        });
    }

    GetProjects();
});