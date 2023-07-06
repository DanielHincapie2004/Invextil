$(document).ready(function () {
    $("div.container-fluid").LoadingOverlay("show");

    fetch("/Dashboard/ObtenerResumen")
        .then(response => {
            $("div.container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);

        })
        .then(responseJson => {
            if (responseJson.estado) {

                //mostrar datos para las tarjetas
                let d = responseJson.objeto

                $("#totalProductos").text(d.totalProductos)
                $("#totalFamilias").text(d.totalCategorias)
                $("#totalVenta").text(d.totalVentas)

                //obtener textos y valores para el grafico
                let barchart_labels;
                let barchart_data;

                if (d.ventasUltimaSemana.length > 0) {
                    barchart_labels = d.ventasUltimaSemana.map((item) => { return item.fecha })
                    barchart_data = d.ventasUltimaSemana.map((item) => { return item.total })
                } else {
                    barchart_labels = ["sin resultados"]
                    barchart_data = [0]
                }

                //obtener textos y valores para el grafico de torta

                let torta_labels;
                let torta_data;

                if (d.productosTopUltimaSemana.length > 0) {
                    torta_labels = d.productosTopUltimaSemana.map((item) => { return item.producto })
                    torta_data = d.productosTopUltimaSemana.map((item) => { return item.cantidad })
                } else {
                    torta_labels = ["sin resultados"]
                    torta_data = [0]
                }

                let controlVenta = document.getElementById("charVentas");
                let myBarChart = new Chart(controlVenta, {
                    type: 'bar',
                    data: {
                        labels: barchart_labels,
                        datasets: [{
                            label: "Cantidad",
                            backgroundColor: "#4e73df",
                            hoverBackgroundColor: "#2e59d9",
                            borderColor: "#4e73df",
                            data: barchart_data,
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: {
                            display: false
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 50,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 5
                                }
                            }],
                        },
                    }
                });


                let controlProducto = document.getElementById("charProductos");
                let myPieChart = new Chart(controlProducto, {
                    type: 'doughnut',
                    data: {
                        labels: torta_labels,
                        datasets: [{
                            data: torta_data,
                            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', "#FF785B"],
                            hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf', "#FF5733"],
                            hoverBorderColor: "rgba(234, 236, 244, 1)",
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        tooltips: {
                            backgroundColor: "rgb(255,255,255)",
                            bodyFontColor: "#858796",
                            borderColor: '#dddfeb',
                            borderWidth: 1,
                            xPadding: 15,
                            yPadding: 15,
                            displayColors: false,
                            caretPadding: 10,
                        },
                        legend: {
                            display: true
                        },
                        cutoutPercentage: 80,
                    },
                });
            }
        })

})  