//RESEÑAS
document.addEventListener("DOMContentLoaded", function () {
    const reseñas = document.querySelector(".reviews-list");
    const contador = document.querySelector(".textarea-counter");
    const caja = document.querySelector("#textbox");
    const estrellas = document.querySelectorAll(".set");
    const dni = document.querySelector("#dni");
    const enviar = document.querySelector("#send");
    const help = document.querySelector(".help");
    //VARIABLES GLOBALES
    var rank = 0;
    function clear() {
        dni.value = "";
        caja.value = "";
        rank = 0;
        estrellas.forEach(star => {
            star.classList.remove("ON");
            star.classList.add("OFF");
            star.style.scale = 1;
        })
        contador.innerHTML = "0/320";
        contador.style.color = "#f0f0d4"
    }
    //CREA UN OBJETO TIPO RESEÑA
    function Review(id, text, number) {
        const temp = {
            dni: id,
            reseña: text,
            puntuacion: number
        }
        return temp;
    }


//AUTOGENERAR CLIENTE
    help.addEventListener("click", function () {
        const action = "Autogen";
        const url = "DesignB/" + action;
        fetch(url, {
            method: "POST"
        }).then(response => {
            return response.json();
        }).then(result => {
            dni.value = result;
        }).catch(error => {
            console.error("Error al procesar solicitud:", error);
            alert("Error al procesar solicitud");
        });
    });

//HACER UNA RESEÑA
    enviar.addEventListener("click", function () {
        if (dni.value.length == 8 && caja.value.length >= 10 && caja.value.length <= 320 && rank > 0) {
            const action = "Make";
            const url = "DesignB/" + action;
            fetch(url, {
                method: "POST",
                headers: new Headers({
                    'Content-Type': 'application/json',
                }),
                body: JSON.stringify(Review(dni.value, caja.value.trim(), rank))
            }).then(response => {
                return response.json();
            }).then(result => {
                if (result == 0) {
                    alert("Cliente no registrado!")              
                } else if (result == 1) {
                    alert("Reseña registrada con exito, gracias!")
                    GetReviews();
                } else if (result == 2) {
                    alert("Usted ya registró su reseña, saludos!")
                    GetReviews();
                }
                clear();
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
        } else {
            alert("Ingrese todos los datos correctamente!");
            clear();
        }
    });
    function ranking() {
        estrellas.forEach((star, index) => { //index1 = posicion del elemento selecionado        
            star.addEventListener("click", function () {
                rank = index + 1;
                estrellas.forEach((star, counter) => { //counter = contador de posiciones               
                    if (index >= counter) {
                        star.classList.remove("OFF");
                        star.classList.add("ON");
                        star.style.scale = 1.5;
                    } else {
                        star.classList.remove("ON");
                        star.classList.add("OFF");
                        star.style.scale = 1;
                    }
                })
                //Al elegir una posicion, el contador recorrera todas las posiciones hasta la posicion elegida
                //Acto seguido, se hacen las acciones indicadas
            })
        })
    }
    ranking();
    caja.addEventListener("keyup", function () {
        const limit = 320;
        let current = caja.value.length;
        contador.innerHTML = current + "/320";
        if (current > limit) {
            contador.style.color = "#870F0F";
        } else if (current >= 10) {
            contador.style.color = "blue";
        } else {
            contador.style.color = "#f0f0d4";
        }
    });
    function GetReviews() {
        const action = "Read";
        const url = "DesignB/" + action;
        fetch(url, {
            method: "GET"
        })
            .then(response => {
                return response.json();
            })
            .then(result => {
                GetReviewsList(result);
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
    }
    function BuildClient(nombre) {
        var temp = document.createElement("h2");
        temp.classList.add("client");
        temp.innerText = nombre;
        return temp;
    }
    function BuildDoxa(doxa) {
        var temp = document.createElement("p");
        temp.classList.add("doxa");
        temp.innerText = "\"" + doxa + "\"";      
        return temp;
    }
    function BuildRank(rank) {
        var temp = document.createElement("div");
        temp.classList.add("rank");
        for (var x = 0; x < rank; x++) {
            var star = document.createElement("div");
            star.classList.add("star");
            star.classList.add("ON");
            temp.appendChild(star);
        }
        return temp;
    }
    function GetReviewsList(Lista) {
        reseñas.innerHTML = "";
        Lista.forEach(item => {
            var box = document.createElement("div");
            box.classList.add("box");
            box.appendChild(BuildClient(item.nombres));
            box.appendChild(BuildDoxa(item.reseña));
            box.appendChild(BuildRank(item.puntuacion));
            reseñas.appendChild(box);
        });
    }

    GetReviews();
});