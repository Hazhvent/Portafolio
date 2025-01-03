document.addEventListener("DOMContentLoaded", function () {

    //FORMULARIO DINAMICO
    const toggle = document.querySelector(".Form__toggle");
    const forms = document.querySelectorAll(".DynamicForm");
    //VARIABLES GLOBALES
    var state = false;
    var seconds = 0.85;
    var time = seconds * 1000;
    //ASIGNACION DE FUNCION AL EVENTO CLICK
    toggle.onclick = Toggle;
    //DEFINICION DE FUNCION "Toggle""
    function Toggle() {
        if (!state) {
            toggle.onclick = null;
            state = true;
            toggle.style.backgroundImage = "url('/img03/switch_on.png')";
            forms[0].classList.add("DynamicForm__Animation");
            setTimeout(function () {
                forms[0].style.display = "none";
                forms[1].style.display = "flex";
                forms[0].classList.remove("DynamicForm__Animation");
                toggle.onclick = Toggle;
            }, time);
        } else {
            toggle.onclick = null;
            state = false;
            toggle.style.backgroundImage = "url('/img03/switch_off.png')";
            forms[1].classList.add("DynamicForm__Animation");
            setTimeout(function () {
                forms[1].style.display = "none";
                forms[0].style.display = "flex";
                forms[1].classList.remove("DynamicForm__Animation");
                toggle.onclick = Toggle;
            }, time);
        }
    }
    //----------------------RESERVACION-------------------------//

    //FORMULARIO PARA ENVIO DE CODIGO
    const send = document.querySelector("#send");
    const mail = document.querySelector("#mail");
    //FORMULARIO PARA VALIDACION DE CORREO
    const check = document.querySelector("#check");
    const checkCode = document.querySelector("#code");
    const timer = document.querySelector("#timer");
    //FORMULARIO PARA RESERVACION
    const reserve = document.querySelector("#reserv");
    const nombres = document.querySelector("#name");
    const apellidos = document.querySelector("#last");
    const celular = document.querySelector("#tel");
    const hora = document.querySelector("#hour");
    //FORMULARIO PARA BUSCAR RESERVACION
    const search = document.querySelector("#search");
    const eliminate = document.querySelector("#delete");
    const aceptar = document.querySelector("#ok");
    const searchCode = document.querySelector("#searchCode");
    const cliente = document.querySelector("#client");
    const mesa = document.querySelector("#table");
    const horario = document.querySelector("#time");
    //VARIABLES GLOBALES
    var mailCode = null;
    var mailsave = null;
    var reservCode = null;
    //INSTANCIA DE CLASE CONTRARELOJ
    const contador = new CountDown(60, timer);

    Init();

    function Init() {
        GetSchedules(horario);
        GetSchedules(hora);
    }

    //CONFIRMAR DESPUES DE BUSQUEDA
    aceptar.addEventListener('click', function () {
        endConsultation();
    });

    //BUSCAR RESERVACION
    search.addEventListener('click', function () {
        if (searchCode.value.length == 6) {
            const action = "Search?codigo=" + searchCode.value;
            const url = "DesignC/" + action;
            fetch(url, {
                method: "GET"
            }).then(response => {
                    return response.json();                             
            }).then(result => {
                    alert("Reservacion Encontrada!");
                    reservCode = searchCode.value;
                    initConsultation(result);
                    clear();
            }).catch(error => {
                alert("Busqueda fallida: No tenemos ninguna reservacion con el codigo proporcionado!");
            });
        } else {
            alert("Ingrese un codigo valido de 6 caracteres!");
            clear();
        }
    });

    //ELMINAR RESERVACION
    eliminate.addEventListener('click', function () {
        const action = "Delete";
        const url = "DesignC/" + action;
        fetch(url, {
            method: "DELETE",
            headers: new Headers({
                'Content-Type': 'application/x-www-form-urlencoded',
            }),
            body: "codigo=" + reservCode
        }).then(response => {
            return response.json();
        }).then(result => {
            if (result) {
                alert("Eliminacion exitosa: revise su correo para ver detalles!");
                endConsultation();
            } else {
                alert("Eliminacion fallida: Intentelo mas tarde!");
            }
            clear();
        }).catch(error => {
            console.error("Error al procesar solicitud:", error);
            alert("Error al procesar solicitud");
        });
    });

    //SOLICITAR HORARIOS
    function GetSchedules(selector) {
        const action = "Read2";
        const url = "DesignC/" + action;
        fetch(url, {
            method: "GET"
        })
            .then(response => response.json())
            .then(result => {
                SetSchedules(result, selector);
            })
            .catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
    }

    //CARGAR HORARIOS
    function SetSchedules(List, select) {
        List.forEach(item => {
            let option = new Option(item.descripcion, item.id);
            select.appendChild(option);
        });
    }

    //CREA UN OBJETO TIPO RESERVACION
    function Reservation(name, last, cell, hour) {
        const temp = {
            nombre: name,
            apellido: last,
            celular: cell,
            correo: mailsave,
            hora: hour
        };
        return temp;
    }

    //HACER RESERVACION
    reserve.addEventListener('click', function () {
        if (nombres.value.length >= 3 && apellidos.value.length >= 4 && celular.value.length == 9 && hora.value != 0) {
            const action = "Create";
            const url = "DesignC/" + action;
            fetch(url, {
                method: "POST",
                headers: new Headers({
                    'Content-Type': 'application/json',
                }),
                body: JSON.stringify(Reservation(nombres.value.trim(), apellidos.value.trim(), celular.value.trim(), hora.value))
            }).then(response => {
                return response.json();
            }).then(result => {
                if (result) {
                    alert("Reservacion exitosa: revise su correo para ver detalles!");
                    endReservation();
                } else {
                    alert("Reservacion fallida: No tenemos mesas disponibles para ese horario, elija otra. Gracias!");
                }
                clear();
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
        } else {
            alert("Ingrese datos validos!");
            clear();
        }
    });

    //VALIDAR CORREO
    check.addEventListener('click', function () {
        if (mailCode == checkCode.value) {
            alert("correo validado!");
            contador.delete();
            initReservation();
            clear();

        } else {
            alert("codigo invalido");
            contador.delete();
            endVal();
            clear();
        }
    });

    //ENVIAR CODIGO
    send.addEventListener('click', function () {
        if (mail.value.length >= 6 && mail.value.includes("@")) {
            mailsave = mail.value;
            const action = "Check";
            const url = "DesignC/" + action;
            fetch(url, {
                method: "POST",
                headers: new Headers({
                    'Content-Type': 'application/x-www-form-urlencoded',
                }),
                body: "email=" + mailsave
            }).then(response => {
                return response.json();
            }).then(result => {
                console.log(result.valido);
                if (result.valido) {
                    mailCode = result.codigo;
                    initVal();
                    timer.addEventListener('timeOver', function () {
                        endVal();
                    });
                    alert("Codigo enviado!");
                } else {
                    alert("El correo ingresado ya fue utilizado, intente otro!");
                }
                clear();
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
        } else {
            alert("Ingrese un correo valido!");
            clear();
        }
    });

    //FUNCIONES AUXILIARES
    function LoadReservation(obj) {
        cliente.value = obj.cliente;
        mesa.value = obj.mesa;
        horario.value = obj.hora;
    }

    function initConsultation(obj) {
        ConsultationON();
        LoadReservation(obj);
    }

    function endConsultation() {
        ConsultationOFF();
    }

    function initReservation() {//INICIAR INTERFAZ DE RESERVACION
        ReservationON();
    }
    function endReservation() {//TERMINAR INTERFAZ DE RESERVACION
        ReservationOFF();
    }
    function endVal() {//TERMINAR INTERFAZ DE VALIDACION
        mailCode = null;
        CheckCodeOFF();
    }
    function initVal() {//INICIAR INTERFAZ DE VALIDACION
        CheckCodeON();
        contador.init();
    }
    function ConsultationON() {
        forms[1].classList.add("DynamicForm__Animation");
        setTimeout(function () {
            forms[1].style.display = "none";
            forms[4].style.display = "flex";
            forms[1].classList.remove("DynamicForm__Animation");
            toggle.onclick = null;
        }, time);
    }
    function ConsultationOFF() {
        forms[4].classList.add("DynamicForm__Animation");
        setTimeout(function () {
            forms[4].style.display = "none";
            forms[1].style.display = "flex";
            forms[4].classList.remove("DynamicForm__Animation");
            toggle.onclick = Toggle;
        }, time);
    }
    function ReservationOFF() {
        forms[3].classList.add("DynamicForm__Animation");
        setTimeout(function () {
            forms[3].style.display = "none";
            forms[0].style.display = "flex";
            forms[3].classList.remove("DynamicForm__Animation");
            toggle.onclick = Toggle;
        }, time);
    }
    function ReservationON() {
        forms[2].classList.add("DynamicForm__Animation");
        setTimeout(function () {
            forms[2].style.display = "none";
            forms[3].style.display = "flex";
            forms[2].classList.remove("DynamicForm__Animation");
        }, time);
    }
    function CheckCodeON() {
        forms[0].classList.add("DynamicForm__Animation");
        setTimeout(function () {
            forms[0].style.display = "none";
            forms[2].style.display = "flex";
            forms[0].classList.remove("DynamicForm__Animation");
            toggle.onclick = null;
        }, time);
    }
    function CheckCodeOFF() {
        forms[2].classList.add("DynamicForm__Animation");
        setTimeout(function () {
            forms[2].style.display = "none";
            forms[0].style.display = "flex";
            forms[2].classList.remove("DynamicForm__Animation");
            toggle.onclick = Toggle;
        }, time);
    }
    function clear() {
        mail.value = "";
        checkCode.value = "";
        searchCode.value = "";
    }
});