$(document).ready(function () {
    localStorage.removeItem('address_1_saved');
    localStorage.removeItem('district');
    localStorage.removeItem('address_2_saved');
})
$(".btn-checkout").click(function (e) {
    if (!checkPhoneNumber()) {
        //$("#MessageError_PhoneNumber").text("Không hợp lệ").css("color", "red").css("padding", "5px").css("display", "block");
        $(".MessageError_PhoneNumber").text("Không hợp lệ").css({
            color: 'red',
            padding: '5px',
            display: 'block'
        });
        $("#SoDienThoai").css("box-shadow", "0 0 0 2px #ff6d6d");
        e.preventDefault();
    } else {
        $("#SoDienThoai").css("box-shadow", "0 0 0 1px #d9d9d9");
        $(".MessageError_PhoneNumber").text("");
    }
})

if (address_2 = localStorage.getItem('address_2_saved')) {
    $('select[name="calc_shipping_district"] option').each(function () {
        if ($(this).text() == address_2) {
            $(this).attr('selected', '')
        }
    })
    $('input.billing_address_2').attr('value', address_2)
}
if (district = localStorage.getItem('district')) {
    $('select[name="calc_shipping_district"]').html(district)
    $('select[name="calc_shipping_district"]').on('change', function () {
        var target = $(this).children('option:selected')
        target.attr('selected', '')
        $('select[name="calc_shipping_district"] option').not(target).removeAttr('selected')
        address_2 = target.text()
        $('input.billing_address_2').attr('value', address_2)
        district = $('select[name="calc_shipping_district"]').html()
        localStorage.setItem('district', district)
        localStorage.setItem('address_2_saved', address_2)
    })
}
$('select[name="calc_shipping_provinces"]').each(function () {
    var $this = $(this),
        stc = ''
    c.forEach(function (i, e) {
        e += +1
        stc += '<option value=' + e + '>' + i + '</option>'
        $this.html('<option value="">Tỉnh / Thành phố</option>' + stc)
        if (address_1 = localStorage.getItem('address_1_saved')) {
            $('select[name="calc_shipping_provinces"] option').each(function () {
                if ($(this).text() == address_1) {
                    $(this).attr('selected', '')
                }
            })
            $('input.billing_address_1').attr('value', address_1)
        }
        $this.on('change', function (i) {
            i = $this.children('option:selected').index() - 1
            var str = '',
                r = $this.val()
            if (r != '') {
                arr[i].forEach(function (el) {
                    str += '<option value="' + el + '">' + el + '</option>'
                    $('select[name="calc_shipping_district"]').html('<option value="">Quận / Huyện</option>' + str)
                })
                var address_1 = $this.children('option:selected').text()
                var district = $('select[name="calc_shipping_district"]').html()
                localStorage.setItem('address_1_saved', address_1)
                localStorage.setItem('district', district)
                $('select[name="calc_shipping_district"]').on('change', function () {
                    var target = $(this).children('option:selected')
                    target.attr('selected', '')
                    $('select[name="calc_shipping_district"] option').not(target).removeAttr('selected')
                    var address_2 = target.text()
                    $('input.billing_address_2').attr('value', address_2)
                    district = $('select[name="calc_shipping_district"]').html()
                    localStorage.setItem('district', district)
                    localStorage.setItem('address_2_saved', address_2)
                })
            } else {
                $('select[name="calc_shipping_district"]').html('<option value="">Quận / Huyện</option>')
                district = $('select[name="calc_shipping_district"]').html()
                localStorage.setItem('district', district)
                localStorage.removeItem('address_1_saved', address_1)
            }
        })
    })
})
$("#calc_shipping_provinces").change(function () {
    $(".billing_address_1").val($(this).find("option:selected").text());
})
$("#calc_shipping_district").change(function () {
    $(".billing_address_2").val($(this).find("option:selected").text());
})

function checkPhoneNumber() {
    var flag = false;
    var phone = $('#SoDienThoai').val().trim();
    phone = phone.replace('(+84)', '0');
    phone = phone.replace('+84', '0');
    phone = phone.replace('0084', '0');
    phone = phone.replace(/ /g, '');
    if (phone != '') {
        var firstNumber = phone.substring(0, 2);
        if ((firstNumber == '09' || firstNumber == '08' || firstNumber == '07' || firstNumber == '06' || firstNumber == '05' || firstNumber == '04' || firstNumber == '03') && phone.length == 10) {
            if (phone.match(/^\d{10}/)) {
                flag = true;
            }
        } else if (firstNumber == '01' || firstNumber == '02' && phone.length == 11) {
            if (phone.match(/^\d{11}/)) {
                flag = true;
            }
        }
    }
    return flag;
}


var isShow = false;

$("#js-handle-cart-list").click(function () {
    if (!isShow) {
        $(".info-cart-text--animate").html("Ẩn");
        isShow = true;
    } else {
        $(".info-cart-text--animate").html("Hiển thị");
        isShow = false;
    }
    $(".item-cart").toggleClass("hide-on-mobile-tablet");
    $(".total-price-table").toggleClass("hide-on-mobile-tablet");
})


let isFetchingActivateDiscountCode = false;

var delay = 2000;
var res = {
    loader: $("<div>", {
        class: 'loader',
    })
};

window.onload = () => {
    let totalPayment;
    if (localStorage.getItem("discountPrice") && localStorage.getItem("totalPayment")) {
        $(".discount-code").css("display", "none");
        totalPayment = localStorage.getItem("totalPayment");
        $(".js-total-payment").html(parseFloat(totalPayment).toLocaleString() + "đ");
    }
}




$("#btnUseDiscountCode").click(function () {
    var dcCode = $("#txtDiscountCode");
    if (isFetchingActivateDiscountCode) return;
    isFetchingActivateDiscountCode = true;
    $.ajax({
        type: 'POST',
        url: '/Cart/ActivateDiscountCode',
        data: { code: dcCode.val().trim() },
        beforeSend: function () {
            $("#btnUseDiscountCode").html('');
            $("#btnUseDiscountCode").append(res.loader);
        },
        success: function (response) {
            setTimeout(() => {
                isFetchingActivateDiscountCode = false;
                $(".loader").remove();
                $("#btnUseDiscountCode").html('Sử dụng');
                //Not found discount code
                //if (response.status === 404 && response.message !== "") {
                //    handleEventInput(response.message);
                //}
                //// Not Acceptable
                //if (response.status === 406) {
                //    handleEventInput(response.message);
                //}
                //// OK
                //if (response.status === 200) {

                //    $(".js-total-payment").html(response.totalFinal.toLocaleString() + "đ");
                //    localStorage.setItem("discountPrice", response.discount);
                //    localStorage.setItem("totalPayment", response.totalFinal);
                //    localStorage.setItem("discountCode", dcCode.val().trim());
                //    alert(response.message);
                //}
                if (response.status === 200) {
                    $(".js-total-payment").html(response.totalFinal.toLocaleString() + "đ");
                    localStorage.setItem("discountPrice", response.discount);
                    localStorage.setItem("totalPayment", response.totalFinal);
                    localStorage.setItem("discountCode", dcCode.val().trim());

                    // Hiển thị thông báo thành công
                    showNotification(response.message, 'success');
                } else {
                    handleEventInput(response.message);
                }
            }, delay);
        },
        error: function (request) {
            console.log(request);
        }
    })
    // Function to handle success message

    // Error message
    let handleEventInput = (message) => {
        $("#js-error-disc-code").html(message);
        $("#js-error-disc-code").css("color", "red");
        dcCode.addClass("error-dis-code");
        $(document).on("input", dcCode, function () {
            if (dcCode.hasClass("error-dis-code")) {
                dcCode.removeClass("error-dis-code");
                $("#js-error-disc-code").html("");
            }
        })
    }
    function showNotification(message, type) {
        // Tạo thông báo tùy theo loại (success hoặc error)
        var notification = $("<div>").addClass("notification " + type).text(message);
        $("body").append(notification);
        // Tự động ẩn thông báo sau 3 giây
        setTimeout(function () {
            notification.fadeOut(500, function () {
                $(this).remove();
            });
        }, 3000);
    }
})