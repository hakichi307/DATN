$(document).ready(function () {
    // Bootstrap datepicker
    $('#update_day .input-group.date').datepicker({
        todayBtn: "linked",
        keyboardNavigation: false,
        forceParse: false,
        calendarWeeks: true,
        autoclose: true
    });

});

$("#form-sample-1").validate({
    rules: {
        TenSP: {
            minlength: 15,
            required: !0
        },
        GiaNiemYet: {
            required: !0,
            number: !0
        },
        GiaKhuyenMai: {
            required: !0,
            number: !0
        },
        SoLuongTon: {
            required: !0,
            number: !0
        },
        SoLanMua: {
            required: !0,
            number: !0
        },
        TenQT: {
            required: !0,
        },
        DonGia: {
            required: !0,
            number: !0
        },
        DonGiaNhap: {
            required: !0,
            number: !0
        },
        SoLuongNhap: {
            required: !0,
            number: !0
        },
        TaiKhoan: {
            required: !0,
        },
        MatKhau: {
            required: !0,
        },
        HoTen: {
            required: !0,
        },
        DiaChi: {
            required: !0,
        },
        Email: {
            required: !0,
        },
        SoDienThoai: {
            required: !0,
            number: !0
        },
        TieuDeTin: {
            required: !0,
            minlength: 15,
        },
        DoanTrich: {
            required: !0,
            minlength: 15,
        },
        NoiDung: {
            required: !0,
            minlength: 30,
        },
    },
    errorClass: "help-block error",
    highlight: function (e) {
        $(e).closest(".form-group.row").addClass("has-error")
    },
    unhighlight: function (e) {
        $(e).closest(".form-group.row").removeClass("has-error")
    },
});


