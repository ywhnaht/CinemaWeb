/*const login = document.queryselector('.js-login-btn');*/
//const modal = document.getElementById('js-modal');
const container = document.getElementById('js-login-container');
const registerBtn = document.getElementById('js-register');
const loginBtn = document.getElementById('js-login');
const error_mess = document.querySelectorAll('.error-message');
var modal = document.getElementById('js-modal');
const inputClicked = document.querySelectorAll('.form-container input')
const signinBtn = document.querySelector('.js-signin');
const signupBtn = document.querySelector('.js-signup');

container.classList.add('active');

modal.addEventListener('click', () => {
    var url = modal.getAttribute('data-url');
    window.location.href = url;
});

registerBtn.addEventListener('click', () => {
    container.classList.add("active");
});

loginBtn.addEventListener('click', () => {
    container.classList.remove("active");
});

container.addEventListener('click', (event) => {
    event.stopPropagation()
});

for(var inputs of inputClicked) {
    inputs.addEventListener('click', () => {
        for (var error of error_mess) {
            error.textContent = "";
        }
    });
}

//signinBtn.addEventListener('click', () => {
//    container.classList.add("active");
//})

//signupBtn.addEventListener('click', () => {
//    container.classList.remove("active");
//})

////signupBtn.addEventListener('click', () => {
////    hideLogin.classList.add('active');
////    showLogin.classList.add('active');
////    modal.classList.remove('open');
////    container.classList.remove("active");
////})

////logoutBtn.addEventListener('click', () => {
////    hideLogin.classList.remove('active');
////    showLogin.classList.remove('active');
////});