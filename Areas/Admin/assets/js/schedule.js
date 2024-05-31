const body = document.querySelector("body"),
    sidebar = body.querySelector(".fui-sidbar-navigiation nav"),
    toggle = body.querySelector(".fui-sidbar-navigiation .toggle"),
    searchBtn = body.querySelector(".fui-sidbar-navigiation .search-box"),
    modeSwitch = body.querySelector(".fui-sidbar-navigiation .toggle-switch");

toggle.addEventListener("click", () => {
    sidebar.classList.toggle("close");
});

searchBtn.addEventListener("click", () => {
    sidebar.classList.remove("close");
});