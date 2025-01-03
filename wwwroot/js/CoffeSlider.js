//SLIDER AUTOMATICO TIPO BANNER 
document.addEventListener("DOMContentLoaded", function () {
    let Clasico = document.querySelector(".slider-c");

    //*FUNCION PARA OBTENER IMAGENES
    function getImages() {
        const action = "Pics";
        const url = "DesignC/" + action;
        fetch(url, {
            method: "GET"
        })
            .then(response => {
                return response.json();
            })
            .then(result => {
                Classic(Clasico, result, true);
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });

    }
    //FUNCION DE SLIDER CLASICO
    function Classic(slider, imagenes) {
        var indice = 0;
        var segundos = 0.75;
        var tiempo = segundos * 1000;
        slider.style.backgroundImage = "url(/img03/" + imagenes[indice] + ".jpg)";

        //INTERVALO DE TIEMPO
        setInterval(function () { Loop() }, tiempo);

        //BUCLE DE IMAGENES
        function Loop() {
            if (indice < imagenes.length - 1) {
                slider.style.backgroundImage = "url(/img03/" + imagenes[indice + 1] + ".jpg)";
                indice++;
            } else {
                indice = 0;
                slider.style.backgroundImage = "url(/img03/" + imagenes[indice] + ".jpg)";
            }
        }
    }
    getImages();
});