
function redirectToSignIn(returnUrl) {
    $('.signin-modal').addClass('open');
    sessionStorage.setItem('returnUrl', returnUrl);
}

function SignIn(email, pass, returnUrl) {
    var form = $('.signup-form');
    form.off('submit').on('submit', function (e) {
        e.preventDefault();
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
                if (!data.success) {
                    $('.error-message').text(data.message);
                } else {
                    var returnUrl = sessionStorage.getItem('returnUrl');
                    sessionStorage.removeItem('returnUrl');
                    window.location.href = returnUrl || data.url;
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    })
}

function CheckExistAccount(email, name, pass, confirmpass, returnUrl) {
    var form = $('.signup-form');
    form.off('submit').on('submit', function (e) {
        e.preventDefault();
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
                    SendVerifyCode(email, name);
                } else {
                    $('.error-message').text(data.message);
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    });
}

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
            console.log(err);
        }
    });
}

function SignUp(name, email, dateofbirth, pass, returnUrl, verifyCode) {
    var form = $('.verifyCode-form');
    form.off('submit').on('submit', function (e) {
        e.preventDefault();
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
                if (!data.success) {
                    $('.response-mess').text(data.message);
                } else {
                    var returnUrl = sessionStorage.getItem('returnUrl');
                    sessionStorage.removeItem('returnUrl');
                    window.location.href = returnUrl || data.url;
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    });
}

function ForgetPassword(registerEmail) {
    var form = $('.forgetPass-form');
    form.off('submit').on('submit', function (e) {
        e.preventDefault();
        $.ajax({
            url: '/Home/ForgetPassword',
            type: 'POST',
            data: { registerEmail: registerEmail },
            success: function (data) {
                $('.response-mess').text(data.message);
            },
            error: function (err) {
                console.log(err);
            }
        });
    });
    $('input').click(() => {
        $('.response-mess').text("");
    });
}
