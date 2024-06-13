function redirectToSignIn(returnUrl) {
    $('.signin-modal').addClass('open')
    sessionStorage.setItem('returnUrl', returnUrl);
}

$('.signin-form').submit(function (e) {
    e.preventDefault();
    var email = $('.signin-form input[name="email"]').val();
    var pass = $('.signin-form input[name="pass"]').val();
    var returnUrl = $('.signin-form input[name="returnUrl"]').val();
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: '/Home/SignIn',
        type: 'POST',
        data: {
            email: email,
            pass: pass,
            returnUrl: returnUrl,
            __RequestVerificationToken: token
        },
        success: function (data) {
            if (data.success === false) {
                $('.error-message').text(data.message)
            }
            else {
                sessionStorage.setItem('signinSuccess', true);
                var returnUrl = sessionStorage.getItem('returnUrl');
                sessionStorage.removeItem('returnUrl');
                if (returnUrl) {
                    window.location.href = returnUrl;
                }
                else
                    window.location.href = data.url;
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
})

$('.signup-form').submit(function (e) {
    e.preventDefault();
    var email = $('input[name="email"]').val();
    var name = $('input[name="name"]').val();
    var pass = $('input[name="pass"]').val();
    var confirmpass = $('input[name="confirmpass"]').val();
    var returnUrl = $('input[name="returnUrl"]').val();
    $.ajax({
        url: '/Home/CheckExistAccount',
        type: 'POST',
        data: {
            email: email,
            name: name,
            pass: pass,
            confirmpass: confirmpass,
            returnUrl: returnUrl
        },
        success: function (data) {
            if (data.success) {
                SendVerifyCode(email, name)
            }
            else {
                $('.error-message').text(data.message)
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
})

function SendVerifyCode(email, name) {
    $.ajax({
        url: '/Home/VerifyEmail',
        type: 'POST',
        data: { email: email, name: name },
        success: function (data) {
            if (data.success) {
                $('#verifyCode').modal('show');
            } else {
                $('.error-message').text(data.message);
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
}

$('.verifyCode-form').submit(function (e) {
    e.preventDefault();
    var verifyCode = $('input[name="verifyCode"]').val();
    var name = $('.signup-form input[name="name"]').val();
    var email = $('.signup-form input[name="email"]').val();
    var pass = $('.signup-form input[name="pass"]').val();
    var dateofbirth = $('.signup-form input[name="dateofbirth"]').val();
    var returnUrl = $('.signup-form input[name="returnUrl"]').val();
    $.ajax({
        url: '/Home/SignUp',
        type: 'POST',
        data: {
            name: name,
            email: email,
            dateofbirth: dateofbirth,
            pass: pass,
            returnUrl: returnUrl,
            verifyCode: verifyCode
        },
        success: function (data) {
            if (data.success === false)
                $('.response-mess').text(data.message)
            else {
                sessionStorage.setItem('signupSuccess', true);
                var returnUrl = sessionStorage.getItem('returnUrl');
                sessionStorage.removeItem('returnUrl');
                if (returnUrl) {
                    window.location.href = returnUrl;
                }
                else
                    window.location.href = data.url;
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
})

$('.forgetPass-form').submit(function (e) {
    e.preventDefault();
    var registerEmail = $('input[name="registerEmail"]').val();
    $.ajax({
        url: '/Home/ForgetPassword',
        type: 'POST',
        data: { registerEmail: registerEmail },
        success: function (data) {
            $('.response-mess').text(data.message)
        },
        error: function (err) {
            console.log(err)
        }
    })
})
$('input').click(() => {
    $('.response-mess').text("")
})