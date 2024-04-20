$(function () {
    var doughnutData = {
        labels: ["Desktop", "Tablet", "Mobile"],
        datasets: [{
            data: [47, 30, 23],
            backgroundColor: ["rgb(255, 99, 132)", "rgb(54, 162, 235)", "rgb(255, 205, 86)"]
        }]
    };


    var doughnutOptions = {
        responsive: true,
        legend: {
            display: false
        },
    };
});