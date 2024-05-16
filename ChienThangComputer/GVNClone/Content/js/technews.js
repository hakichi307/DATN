let menuToggle = document.querySelector(".header__mobile-toggle");
let menuMobile = document.querySelector(".header__navbar-mobile");
let menuMobileSearch = document.querySelector(".header__navbar-mobile-search");
let menuClose = document.querySelector(".header__navbar-mobile-close");
let btnSearch = document.getElementById("js-btn-search");
let body = document.querySelector("body");
let formSearch = document.querySelector(".header-form-search");
let page = document.querySelector(".page-news");
let btnSearchMobile = document.querySelector(".header__mobile-search-icon");
let menuCloseSearch = document.querySelector(".header__navbar-mobile-search-close");
let lineAnimate = document.querySelector(".line-animate");
var lastPos = 0;

window.onscroll = () => scrollFunction();


function scrollFunction() {
    var header = document.getElementById("js-header-navbar");
    var currentPos = document.documentElement.scrollTop || document.body.scrollTop;
    if (currentPos < lastPos) {
        header.classList.add("header__navbar-fixed");
    } else {
        header.classList.remove("header__navbar-fixed");
    }
    lastPos = currentPos;
    if (currentPos < 100) {
        header.classList.remove("header__navbar-fixed");
    }
}



btnSearch.onclick = () => {
    formSearch.classList.toggle("active");
}

btnSearchMobile.addEventListener("click", () => {
    document.documentElement.scrollTop = 0;
    document.body.scrollTop = 0;
    menuMobileSearch.classList.add("active");
    body.classList.add("disable-scroll", "perspective");
    page.classList.add("perspective");
    setTimeout(() => {
        lineAnimate.classList.add("active");
        document.querySelector(".form-input-search-mobile").focus();
    }, 500);
})

document.addEventListener("click", (e) => {
    if (!e.target.closest(".header-form-search,#js-btn-search")) {
        formSearch.classList.remove("active");
    }
})

document.addEventListener("keydown", (e) => {
    if (e.which === 27) {
        formSearch.classList.remove("active");
    }
})


menuToggle.addEventListener("click", () => {
    document.documentElement.scrollTop = 0;
    document.body.scrollTop = 0;
    menuMobile.classList.add("active");
    body.classList.add("disable-scroll", "perspective");
    page.classList.add("perspective");
})

menuClose.addEventListener("click", () => {
    body.classList.remove("disable-scroll", "perspective");
    page.classList.remove("perspective");
    menuMobile.classList.remove("active");
})

menuCloseSearch.addEventListener("click", () => {
    body.classList.remove("disable-scroll", "perspective");
    page.classList.remove("perspective");
    menuMobileSearch.classList.remove("active");
    lineAnimate.classList.remove("active");
})