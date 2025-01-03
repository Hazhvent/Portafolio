//GALERIA
document.addEventListener("DOMContentLoaded", function () {
    let grid = document.querySelector(".gallery");

    //*FUNCION PARA OBTENER IMAGENES
    function getImages() {
        const action = "Pics";
        const url = "DesignB/" + action;
        fetch(url, {
            method: "GET"
        })
            .then(response => {
                return response.json();
            })
            .then(result => {
                GridGallery(result);
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });

    }

    // FUNCIÓN PRINCIPAL DE GALERÍA
    function GridGallery(images) {
        // Crear visor
        const viewer = document.createElement("div");
        viewer.classList.add("viewer");

        const content = document.createElement("div");
        content.classList.add("viewer-content");

        const close = document.createElement("i");
        close.classList.add("viewer-close");
        close.classList.add("fa-solid");
        close.classList.add("fa-square-xmark");

        viewer.appendChild(content);
        viewer.appendChild(close);
        document.body.appendChild(viewer);

        // Llenado de imágenes
        images.forEach((image) => {
            const box = document.createElement("div");
            box.style.backgroundImage = `url(./img02/${image}.jpg)`;
            box.classList.add("item");
            grid.appendChild(box);
        });

        // MOSTRAR EL VISOR
        grid.addEventListener("click", function (e) {
            if (e.target.classList.contains("item")) {
                viewer.classList.add("viewer-show");
                content.style.backgroundImage = e.target.style.backgroundImage;
            }
        });

        // CERRAR EL VISOR CON CLIC
        close.addEventListener("click", function () {
            viewer.classList.remove("viewer-show");
        });

        // CERRAR EL VISOR CON ESC
        document.addEventListener("keydown", function (e) {
            if (e.key === "Escape" && viewer.classList.contains("viewer-show")) {
                viewer.classList.remove("viewer-show");
            }
        });
    }
    getImages();
});