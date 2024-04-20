$(document).ready(function () {
    handleMegaMenu();
    $('.carousel').slick({
        autoplay: true,
        autoplaySpeed: 2000,
        arrows: false,
    });
    $(".flickity-main-carousel").flickity({
        groupCells: true,
        pageDots: false,
        contain: true,
        autoPlay: 3500,
    })
    $("#dropdownlist").change(function (e) {
        let element = $(this).find('option:selected');
        let optionVal = element.attr("value");
        let sort;
        switch (optionVal) {
            case "price-ascending": {
                sort = "price-ascending";
                break;
            };
            case "price-descending": {
                sort = "price-descending";
                break;
            };
            case "title-ascending": {
                sort = "title-ascending";
                break;
            };
            case "title-descending": {
                sort = "title-descending";
                break;
            };
            case "best-selling": {
                sort = "best-selling";
                break;
            };
        }
        $.ajax({
            url: "/Product/Sort",
            data: { sort: sort },
            success: function (response) {
                $("#sortby").html(response);
            }
        })
    });

    $(".dropdownlist-sort-all").change(function (e) {
        let element = $(this).find('option:selected');
        let optionVal = element.attr("value");
        let sort;
        switch (optionVal) {
            case "price-ascending": {
                sort = "price-ascending";
                break;
            };
            case "price-descending": {
                sort = "price-descending";
                break;
            };
            case "title-ascending": {
                sort = "title-ascending";
                break;
            };
            case "title-descending": {
                sort = "title-descending";
                break;
            };
            case "best-selling": {
                sort = "best-selling";
                break;
            };
        }
        $.ajax({
            type: 'POST',
            url: "/Product/SortAllProduct",
            data: { sort: sort },
            success: function (res) {
                window.location.href = res.redirectUrl;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR.responseText);
                console.log(errorThrown);
            }
        })
    });
    $("#CaptchaInputText").addClass("form-control");
    $("#CaptchaImage").addClass("w-100");

    $(".btn-update").click(function () {
        $(".wrap-Info").slideToggle();
    });
    $(".btn-cancel").click(function () {
        $(".wrap-Info").slideUp();
    });

    $("#txtSearch").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Product/GetDataFromSearchAutoComplete",
                type: 'POST',
                dataType: "json",
                data: {
                    prefix: request.term
                },
                success: function (res) {
                    response(res.data);
                }
            });
        },
        open: function () {
            $(".ui-autocomplete:visible").css({ top: "+=10" });
        },
    })
        .autocomplete("instance")._renderItem = function (ul, item) {
            return $("<li>")
                .append(
                    "<a class='ui-menu-item-link' href ='/xem-chi-tiet/" + item.id + "/" + item.name.replace(/\s/g, "-") + "'><div class='ui-menu-item-left'><img class='ui-menu-img' src='/Images/" + item.image + "' alt='Hình'></div><div class='ui-menu-item-right'><div class='ui-menu-item-text'>" + item.name + "</div><div class='ui-menu-item-price'><span class='ui-menu-item-price-original'>" + item.price_original.toLocaleString() + " đ</span><span class='ui-menu-item-price-sale'>" + item.price_sale.toLocaleString() + " đ</span></div></div></a>"
                )
                .appendTo(ul);
        };
    $(window).resize(function () {
        $(".ui-autocomplete").css('display', 'none');
    });

    /* Handle mega menu mobile */

    $(".categories-item").each(function () {
        $(this).click(function () {
            $(this).toggleClass("active-mobile-menu");
            $(this).next(".sub-nav-mobile-list").slideToggle({ duration: 200 });
            $(this).find(".categories-item__right").toggleClass("rotate-arrow");
        })
    })
    $(".sub-nav-item-type").each(function () {
        $(this).click(function () {
            $(this).next(".sub-nav-mobile-list-2").slideToggle({ duration: 200 });
            $(this).find("i").toggleClass("rotate-arrow");
        })
    })
});

window.onscroll = function () { scrollFunction() };
function scrollFunction() {
    var getHeightAdvertise = $(".advertise").innerHeight();
    if ($(window).scrollTop() > 300) {
        $(".fix-header-container").css("top", -getHeightAdvertise + "px");
        $("#ui-id-1").addClass("posY-ui-menu");
        $(".categories").hover(
            function () {
                $(this).addClass("active-menu");
                $(".overlay-mega-menu").css('display', 'block');
            },
            function () {
                $(this).removeClass("active-menu");
                $(".overlay-mega-menu").css('display', 'none');
            }
        );
    } else {
        $(".fix-header-container").css("top", 0 + "px");
        $("#ui-id-1").removeClass("posY-ui-menu");
        $(".categories").hover(
            function () {
                $(this).removeClass("active-menu");
                $(".overlay-mega-menu").css('display', 'none');
            }
        );
    }
};


window.onload = function () {
    window.dispatchEvent(new Event('resize'));
}


//window.addEventListener("pageshow", function (event) {
//    var historyTraversal = event.persisted ||
//                           (typeof window.performance != "undefined" &&
//                                window.performance.navigation.type === 2);
//    if (historyTraversal) {
//        // Handle page restore.
//        window.location.reload();
//    }
//});


//-------------- If you have domain, please config your domain here

var defaultHref = "http://localhost:14315/";

//-------------------------------------------------------------------

function handleMegaMenu() {
    if (defaultHref !== window.location.href) {
        $(".categories").hover(
            function () {
                $(this).addClass("active-menu-1");
                $(".overlay-mega-menu").addClass("d-block-overlay-mega-menu");
            },
            function () {
                $(this).removeClass("active-menu-1");
                $(".overlay-mega-menu").removeClass("d-block-overlay-mega-menu");
            }
        );
    }
}

$(".btn-add-to-cart").click(function () {
    var idProduct = $(this).data("id");
    $.ajax({
        async: true,
        type: 'POST',
        url: '/Cart/AddToCart',
        data: { id: idProduct },
        success: function (res) {
            if (res.success === false) {
                OutOfStock(res.message, res.quantity);
            } else {
                $("#add-to-cart").css({ display: "block" }).animate({ opacity: 1 }, 1200, "easeOutBack");
                $("#add-to-cart").animate({ opacity: 0 }, 700,
                    function () {
                        $("#add-to-cart").css({ display: "none" })
                    });
                $("#cart").html(res);
                handleCartMobile(x);
            }

        },
    });
})


function OutOfStock(message, quantity) {
    var patialView = "<a href='/cart'><img src='/Images/cart.png' class='cart' alt='Hình' /><span class='cart-text'></span></a><span class='total_quantity'>" + quantity + "</span>";
    Toast("Thông báo", message, "error", 2000);
    $("#cart").html(patialView);
    handleCartMobile(x);
}




function handleCartMobile(x) {
    if (x.matches) {
        document.querySelector(".cart-text").innerHTML = "";
    } else {
        document.querySelector(".cart-text").innerHTML = "Giỏ hàng";
    }
}

var x = window.matchMedia("(max-width: 1023px)");
handleCartMobile(x);
x.addListener(handleCartMobile);


function hideAutocompleteMobile(screenMobile) {
    if (screenMobile.matches) {
        $("#txtSearch").autocomplete({ disabled: true })
    } else {
        $("#txtSearch").autocomplete({ disabled: false })
    }
}

var screenMobile = window.matchMedia("(max-width: 740px)");
hideAutocompleteMobile(screenMobile);
screenMobile.addListener(hideAutocompleteMobile);



var currentPage = window.location.pathname;
if (currentPage != "/thanh-toan") {
    localStorage.removeItem("totalPayment");
    localStorage.removeItem("discountPrice");
    localStorage.removeItem("discountCode");
}



/* TOAST MESSAGE */


function Toast(title = '', message = '', type = 'success', duration = 2000) {
    const mainToast = document.getElementById("toast");
    if (mainToast) {
        const toast = document.createElement("div");
        const autoRemoveID = setTimeout(function () {
            mainToast.removeChild(toast);
        }, duration + 1000);

        toast.onclick = function (e) {
            if (e.target.closest(".toast__close")) {
                mainToast.removeChild(toast);
                clearTimeout(autoRemoveID);
            }
        }

        toast.classList.add("toast", `toast--${type}`);
        const icons = {
            success: 'fas fa-check-circle',
            info: 'fas fa-info-circle',
            warning: 'fas fa-exclamation-circle',
            error: 'fas fa-exclamation-circle'
        }
        const icon = icons[type];
        toast.innerHTML = `
                    <div class="toast__icon">
                        <i class="${icon}"></i>
                    </div>
                    <div class="toast__body">
                        <h3 class="toast__title">${title}</h3>
                        <p class="toast__msg">${message}</p>
                    </div>
                    <div class="toast__close">
                        <i class="fas fa-times"></i>
                    </div>
        `;
        const delay = (duration / 1000).toFixed(2);
        toast.style.animation = `slideInLeft ease 0.3s, fadeOut linear 1s ${delay}s forwards`;

        mainToast.appendChild(toast);
    }
}

/* Config FVPP */

$('#fvpp-advice').firstVisitPopup({
    cookieName: 'MarkLeo',
    // showAgainSelector: '#show-message'
});


function goToSurvey() {
    window.location.href = "/tu-van-mua-hang";
}
















