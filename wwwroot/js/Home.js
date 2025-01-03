//RETORNO AL PORTAFOLIO
document.addEventListener("DOMContentLoaded", function () {
    function back() {
        let jump = document.createElement("div");
        jump.classList.add("home");
        jump.addEventListener("click", function () {
            location.href = "Home";
        })
        document.body.appendChild(jump);
    }
    back();
});
