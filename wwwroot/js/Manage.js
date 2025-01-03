document.addEventListener("DOMContentLoaded", function () {
    const save = document.querySelector("#save");
    const state = document.querySelector("#switch");
    const stateIcon = document.querySelector("#state");
    const cover = document.querySelector("#cover");
    const upload = document.querySelector("#upload");
    const toggle = document.querySelector("#toggle");
    const name = document.querySelector("#name");
    const image = document.querySelector(".manage__img");
    const here = document.querySelector(".here");
    const gen = document.querySelector("#gen");
    const classf = document.querySelector("#class");
    const sub = document.querySelector("#sub");
    const dob = document.querySelector("#dob");

    //VARIABLES GLOABALES
    var movie;
    var file;
    var change;
    var lock = "fa-file-circle-check"; 
    var unlock = "fa-file-circle-xmark"; 

    Init();

    function Init() {
        GetGenres();
        GetClassif();   
        //SearchId() es llamado despues de que GetClassif() y GetGenres(); han finalizado su consulta
        //SearchId() esta ubicado despues de procesar la respuesta de GetClassif()
        //SearchId() pudo haber estado despues de GetGenres(), es indiferente eso
    }
    //CAMBIA ENTRE EDITAR Y ELIMINAR PELICULA
    toggle.addEventListener('change', function () {
        if (this.checked) {
            SetNew();
            change = true;
        } else {
            SetUpdate();
            change = false;
        }
    });
    //SUBE LA IMAGEN DE LA PELICULA
    upload.addEventListener("click", function () {
        cover.click();
    });
    //CONTIENE Y MUESTRA LA IMAGEN SUBIDA
    cover.addEventListener("change", function () {
        file = this.files[0];
        const reader = new FileReader();
        reader.onload = function (e) {
            var pic = e.target.result;
            image.style.backgroundImage = `url(${pic})`;   
            here.style.display = "none";
        };
        reader.readAsDataURL(file);
    });

    //CREA UN OBJETO TIPO PELICULA
    function Movie(nro, name, genre, classf, ver) {
        const temp = {
            id: nro,
            nombre: name,
            genero: genre,
            clasificacion: classf,
            version: ver
        };
        return temp;
    }

    //VALIDACION DE CAMPOS 
    function Validations() {
        if (name.value.length >= 3 && parseInt(gen.value) > 0 && parseInt(classf.value) > 0 && (sub.checked == true || dob.checked == true))  {
            return true;
        }
        return false;
    }

    // EDITAR PELICULA
    save.addEventListener("click", function () {

        if (Validations()) {
            let msg = "";
            let error = "";
            let action;
            let method;

            if (change) {
                method = "POST";
                action = "Create";
                msg = "Pelicula creada correctamente!";
                error = "Error al crear la pelicula!";
            }
            else {
                method = "PUT";
                action = "Update";
                msg = "Pelicula actualizada correctamente!";
                error = "Error al actualizar la pelicula!";
            }
            const url = action;
            fetch(url, {
                method: method,
                headers: new Headers({
                    'Content-Type': 'application/json',
                }),
                body: JSON.stringify(Movie(movie, name.value, parseInt(gen.value), parseInt(classf.value), sub.checked ? 0 : 1))
            })
                .then(response => {
                    return response.json();
                })
                .then(result => {
                    //result = 0: SE ACTUALIZO UNA PELICULA
                   //result >= 1: SE CREO UNA PELICULA
                    if (result >= 0) {
                        alert(msg);//SE CONFIRMA LO QUE SE HAYA REALIZADO
                        if (result >= 1) {
                            UpdatePic(result);
                        } else {
                            UpdatePic(movie);
                        }                   
                    } else {
                        //result = -1: NO SE ACTUALIZO LA PELICULA
                        alert(error);
                    }
                })
                .catch(error => {
                    console.error("Error al procesar solicitud:", error);
                    alert("Error al procesar solicitud");
                });
        } else {
            alert("Ingrese todos los datos requeridos!");
        }

        
    });

    //ACTUALIZAR IMAGEN
    function UpdatePic(id) {
        const action = "Upload";
        const url = action;
        const formData = new FormData();
        formData.append('id', id); 
        formData.append('file', file); 
        fetch(url, {
            method: "POST",
            body: formData           
        })
            .then(response => {
                return response.json();
            })
            .then(result => {
                if (result) {
                    alert("Caratula actualizada!");
                } else {
                    alert("Caratula no actualizada!");
                    GetMovie(id);
                }
            })
            .catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
    }

    //CAMBIAR ESTADO DE PELICULA
    state.addEventListener("click", function () {
        const action = "Switch/" + movie;
        const url = action;
        fetch(url, {
            method: "PATCH"
        })
            .then(response => {
                return response.json();
            })
            .then(result => {
                if (result != null) {
                    UpdateState(result);
                } else {
                    alert("Error: Pelicula no encontrada");
                }
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
    });

    //IDENTIFICA EL ID DE LA PELICULA RECIBIDA
    function SearchId() {
        const urlParams = new URLSearchParams(window.location.search);
        const Id = urlParams.get('id');
        if (Id !== null) {
            GetMovie(Id);
        }
    }

    //BUSCAR PELICULA
    function GetMovie(movieId) {
        const action = "GetMovie/" + movieId;
        const url = action;
        fetch(url, {
            method: "GET"
        })
            .then(response => {
                return response.json();
            })
            .then(result => {
                LoadMovie(result);
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
    }

    //CONSULTA LA LISTA DE GENEROS
    function GetGenres() {
        const action = "Read2";
        const url = action;
        fetch(url, {
            method: "GET"
        })
            .then(response => {
                return response.json();
            })
            .then(result => {
                LoadGenres(result);
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
    }

    //CONSULTA LA LISTA DE CLASIFICACIONES
    function GetClassif() {
        const action = "Read3";
        const url = action;
        fetch(url, {
            method: "GET"
        })
            .then(response => {
                return response.json();
            })
            .then(result => {
                LoadClassf(result);
                SearchId();
            }).catch(error => {
                console.error("Error al procesar solicitud:", error);
                alert("Error al procesar solicitud");
            });
    }

    //FUNCIONES AUXILIARES

    //CARGAR PELICULA
    function LoadMovie(obj) {
        movie = obj.id;//ID DE PELICULA
        name.value = obj.nombre;
        image.style.backgroundImage = "url('" + (obj.path) + "')";
        gen.value = obj.genero;
        classf.value = obj.clasificacion;     
        LoadVersion(obj);
        LoadState(obj);
        
    }

    function UpdateState(value) {
        if (!value) {
            stateIcon.classList.add(lock);
            stateIcon.classList.remove(unlock);
        } else {
            stateIcon.classList.add(unlock);
            stateIcon.classList.remove(lock);
        }
    }

    function LoadState(obj) {
        var temp = "";
        if (obj.estado) {
            temp = unlock;
        } else {
            temp = lock;
        }
        stateIcon.classList.add(temp);
    }

    function LoadVersion(obj) {
        switch (obj.version) {
            case 0:
                sub.checked = true; 
                dob.checked = false; 
                break;
            case 1:
                sub.checked = false; 
                dob.checked = true; 
                break;
            default:
                sub.checked = false; 
                dob.checked = false;
                break;
        }
    }

    function LoadClassf(List) {
        List.forEach((item) => {
            let option = new Option(item.descripcion, item.id);
            classf.appendChild(option);
        });
    }
    function LoadGenres(List) {
        List.forEach((item) => {
            let option = new Option(item.descripcion, item.id);
            gen.appendChild(option);
        });

    }
        function SetNew() {
            state.disabled = true;
            state.style.backgroundColor = "#454141";
            name.value = "";
            gen.selectedIndex = 0;
            classf.selectedIndex = 0;
            sub.checked = false;
            dob.checked = false;
            here.style.display = "flex";
        }

        function SetUpdate() {
            state.disabled = false;
            state.style.backgroundColor = "";
            here.style.display = "none";
            SearchId();
        }

});    
