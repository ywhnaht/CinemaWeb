const login = document.querySelector('.js-login-btn');
const modal = document.querySelector('.js-modal');
const container = document.getElementById('js-login-container');
const registerBtn = document.getElementById('js-register');
const loginBtn = document.getElementById('js-login');
const movieHover = document.querySelector('.movie-area');
const hideLogin = document.querySelector('.js-hidden-login');
const showLogin = document.querySelector('.js-show-login');
const logoutBtn = document.querySelector('.js-logout');
const signupBtn = document.querySelector('.js-signup');
const signinBtn = document.querySelector('.js-signin');

login.addEventListener('click', () => {
    modal.classList.add('open');
})

modal.addEventListener('click', () => {
    modal.classList.remove('open');
    container.classList.remove("active");
});

registerBtn.addEventListener('click', () => {
    container.classList.add("active");
});

loginBtn.addEventListener('click', () => {
    container.classList.remove("active");
});

container.addEventListener('click', (event) => {
    event.stopPropagation()
})

signinBtn.addEventListener('click', () => {
    hideLogin.classList.add('active');
    showLogin.classList.add('active');
    modal.classList.remove('open');
})

signupBtn.addEventListener('click', () => {
    hideLogin.classList.add('active');
    showLogin.classList.add('active');
    modal.classList.remove('open');
    container.classList.remove("active");
})

logoutBtn.addEventListener('click', () => {
    hideLogin.classList.remove('active');
    showLogin.classList.remove('active');
});