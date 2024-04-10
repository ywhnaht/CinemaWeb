const login = document.querySelector('.js-login-btn');
const modal = document.getElementById('js-modal');
const container = document.getElementById('js-login-container');
const registerBtn = document.getElementById('js-register');
const loginBtn = document.getElementById('js-login');
const signupBtn = document.querySelector('.js-signup');
const signinBtn = document.querySelector('.js-signin');
const inputClicked = document.querySelectorAll('.form-container input');
const error_mess = document.querySelectorAll('.error-message');



login.addEventListener('click', () => {
    modal.classList.add('open');
});

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
});

//signinBtn.addEventListener('click', () => {
//    modal.classList.remove('open');
//});

for (var inputs of inputClicked) {
    inputs.addEventListener('click', () => {
        for (var error of error_mess) {
            error.textContent = "";
        }
    });
}

//signinBtn.addEventListener('click', () => {
//    for (var error of error_mess) {
//        if (error != "") {
//            modal.classList.add('open');
//            var emailInput = document.querySelector('.form-container.sign-in input[name="email"]');
//            emailInput.value = '@ViewBag.Email';
//        }
//    }
//});