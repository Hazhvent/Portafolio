//MENU FIJO
document.addEventListener("DOMContentLoaded", function () {
    const menu = document.querySelector(".content");

    window.addEventListener('scroll', () => {
        let scroll = window.scrollY
        if (scroll > 10) {
            menu.style.backgroundColor = '#1c1c1c';
        } else {
            menu.style.backgroundColor = 'transparent';
        }

    })
});