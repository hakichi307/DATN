$(document).ready(function () {
    var monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ff3838', '#fff200', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    var toDay = new Date();

    function GetRevenue(res, year) {
        var arrYAsis = JSON.parse(res.yAsis);
        var arrRevenue = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
        for (var i = 0; i < arrYAsis.length; i++) {
            arrRevenue[arrYAsis[i].Month - 1] = arrYAsis[i].Money;
        }
        Highcharts.chart('bar-chart', {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Thống kê doanh thu'
            },
            subtitle: {
                text: 'Tổng doanh thu theo từng tháng trong năm ' + year
            },
            xAxis: {
                categories: monthNames,
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Tổng doanh thu (VNĐ)'
                },
                labels: {
                    formatter: function () {
                        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(this.value);
                    }
                }
            },
            tooltip: {
                borderWidth: 1,
                shadow: true,
                useHTML: true,
                shared: true
                //formatter: function () {
                //    return '<span style="font-size:10px">' + this.x + '</span><table>' + '<tr><td style="color:' + this.series.color + ';padding:0">' + this.series.name +': </td>' +
                //        '<td style="padding:0"><b>' + Highcharts.numberFormat(this.y, 0, ',') +' VNĐ</b></td></tr></table>';
                //},
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            series: [{
                name: 'Tổng doanh thu',
                data: arrRevenue
            }]
        });
    }

    function GetTotalOrder(res, year) {
        var arrYAsis = JSON.parse(res.yAsis);
        var arrValueTotalOrder = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
        for (var i = 0; i < arrYAsis.length; i++) {
            arrValueTotalOrder[arrYAsis[i].Month - 1] = arrYAsis[i].Value;
        }

        Highcharts.chart('line-chart', {
            chart: {
                type: 'spline'
            },
            title: {
                text: 'Thống kê lượng đơn hàng năm ' + year,
            },
            xAxis: {
                categories: monthNames,
            },
            yAxis: {
                title: {
                    text: 'Số lượng'
                },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                }
            },
            tooltip: {
                crosshairs: true,
                shared: true,
            },
            plotOptions: {
                spline: {
                    marker: {
                        radius: 4,
                        lineColor: '#666666',
                        lineWidth: 1
                    }
                }
            },
            series: [{
                name: 'Tổng đơn hàng',
                marker: {
                    symbol: 'circle'
                },
                data: arrValueTotalOrder
            }]
        });
    }

    function GetStatusOrder(res, month, year) {
        Highcharts.chart('pie-chart', {
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                type: 'pie'
            },
            title: {
                text: 'Tình trạng giao hàng tháng ' + month + ' năm ' + year,
            },
            tooltip: {
                pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
            },
            accessibility: {
                point: {
                    valueSuffix: '%'
                }
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false
                    },
                    showInLegend: true
                }
            },
            series: [{
                colorByPoint: true,
                name: 'Tổng',
                data: [{
                    name: 'Chờ xác nhận',
                    y: res.WaitForConfirmation,
                }, {
                    name: 'Chờ lấy hàng',
                    y: res.WaitForGetGoods
                }, {
                    name: 'Đang giao',
                    y: res.Delivering
                }, {
                    name: 'Đã giao',
                    y: res.Delivered
                }, {
                    name: 'Đã huỷ',
                    y: res.Canceled
                }]
            }]
        });
    }

    function GetTopProductBestSelling(res) {
        var arrProduct = [];
        for (var i = 0; i < res.length; i++) {
            var obj = new Object();
            obj.name = res[i].Name;
            obj.y = res[i].QuantitySold;
            arrProduct.push(obj);
        }
        Highcharts.chart('pie-chart-top-product', {
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                type: 'pie'
            },
            title: {
                text: 'Top 5 sản phẩm bán chạy'
            },
            tooltip: {
                pointFormat: '{series.name}: <b>{point.y}</b>'
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false
                    },
                    showInLegend: true
                }
            },
            series: [{
                colorByPoint: true,
                name: 'Đã bán',
                data: arrProduct
            }]
        });
    }
    
    $('#startDate').datepicker({
        todayBtn: "linked",
        keyboardNavigation: false,
        forceParse: false,
        calendarWeeks: true,
        autoclose: true,
        format: "mm-yyyy",
        startView: "months",
        minViewMode: "months"
    }).on('changeDate', function (e) {
        var pickedMonth = new Date(e.date).getMonth() + 1;
        var pickedYear = new Date(e.date).getFullYear();
        $.ajax({
            type: 'POST',
            url: '/Highchart/PostDataGetStatusOrder',
            data: { month: pickedMonth, year: pickedYear },
            success: function (response) {
                GetStatusOrder(response, pickedMonth, pickedYear);
            }
        })
    });

    $('#startDate').datepicker("setDate", new Date());

    $('#yearRevenue').datepicker({
        todayBtn: "linked",
        keyboardNavigation: false,
        forceParse: false,
        calendarWeeks: true,
        autoclose: true,
        format: "yyyy",
        startView: "years",
        minViewMode: "years"
    }).on('changeYear', function (e) {
        var pickedYear = new Date(e.date).getFullYear();    
        $.ajax({
            type: 'POST',
            url: '/Highchart/PostDataGetRevenue',
            data: { year: parseInt(pickedYear) },
            success: function (response) {
                GetRevenue(response, pickedYear);
            }
        });
    });
    $('#yearRevenue').datepicker("setDate", new Date());
    $('#yearTotalOrder').datepicker({
        todayBtn: "linked",
        keyboardNavigation: false,
        forceParse: false,
        calendarWeeks: true,
        autoclose: true,
        format: "yyyy",
        startView: "years",
        minViewMode: "years"
    }).on('changeYear', function (e) {
        var pickedYear = new Date(e.date).getFullYear();
        $.ajax({
            type: 'POST',
            url: '/Highchart/PostDataGetTotalOrder',
            data: { year: parseInt(pickedYear) },
            success: function (response) {
                GetTotalOrder(response, pickedYear)
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR.status);
                console.log(errorThrown);
            }
        })
    })
    $('#yearTotalOrder').datepicker("setDate", new Date());

    $.getJSON("/Highchart/GetRevenue", function (response) {
        GetRevenue(response, toDay.getFullYear());
    })


    $.getJSON("/Highchart/GetStatusOrder", function (response) {
        GetStatusOrder(response, toDay.getMonth() + 1, toDay.getFullYear());
    })

    $.getJSON("/Highchart/GetTopProductBestSelling", function (response) {
        GetTopProductBestSelling(response);
    })

    $.getJSON("/Highchart/GetTotalOrder", function (response) {
        GetTotalOrder(response, toDay.getFullYear())
    })
})