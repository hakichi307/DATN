var btnNextStep = $("#js-btn-auth-nextstep");
var errortext = $(".errorMessageAuth");

$("#email").on("keydown", function (e) {
    if (e.which == 13) {
        var email = $(this).val();
        validEmail(email, errortext, e);
    }
})


btnNextStep.on("click", function (e) {
    var email = $("#email").val();
    validEmail(email, errortext, e);
})


function validEmail(email, messageError, eventClick) {
    if (email == "") {
        messageError.text('Vui lòng nhập email của bạn');
        eventClick.preventDefault();
    } else if (!validateEmail(email)) {
        messageError.text('Email không hợp lệ');
        eventClick.preventDefault();
    }
    else {
        btnNextStep.text('');
        messageError.text('');
        var loader = "<div class='loader'></div>";
        btnNextStep.append(loader);
        btnNextStep.css("cursor", "default");
        btnNextStep.attr("disabled", true);
        getOTP(email);
    }
}



function getOTP(email) {
    $.ajax({
        type: 'POST',
        url: '/Auth/VerifyEmail',
        data: { email: email },
        success: function (response) {
            if (response.status === true) {
                window.location.href = "/auth/xac-minh-otp";
            }
            if (response.status === false) {
                $(".loader").remove();
                btnNextStep.text("Tiếp theo");
                btnNextStep.css("cursor", "pointer");
                errortext.text('Email đã được sử dụng');
                btnNextStep.attr("disabled", false);
            }
        },
        error: function (error) {
            console.log(error.status);
        }
    })
}

function validateEmail(email) {
    return email.match(
        /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
    );
}