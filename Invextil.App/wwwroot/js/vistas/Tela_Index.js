const MODELO_BASE = {
    idProducto: 0,
    codigoBarra: "",
    descripcion: "",
    idCategoria: 0,
    stock: 0,
    urlImagen: "",
    precio: 0,
    esActivo: 1,
}

let tablaData;

$(document).ready(function () {


    fetch("/Familia/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboFamilia").append(
                        $("<option>").val(item.idCategoria).text(item.descripcion)
                    )
                })
            }
        })


    //Selecciona la tabla 
    tablaData = $('#tbdata').DataTable({
        //Se trabaaj de moda responsive 
        responsive: true,
        //El metodo ajax nos sirve para traer toda la informacion
        "ajax": {
            "url": '/Tela/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idProducto", "visible": false, "searchable": false },
            {
                "data": "urlImagen", render: function (data) {
                    return `<img style="height:60px; width:60px"  src=${data} class="rounded mx-auto d-block" />`;
                }
            },
            { "data": "codigoBarra" },
            { "data": "descripcion" },
            { "data": "nombreCategoria" },
            { "data": "stock" },
            { "data": "precio" },
            {
                "data": "esActivo", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-info">Activo</span>';
                    else
                        return '<span class="badge badge-danger">No Activo</span>';
                }
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm mr-2"><i class="fas fa-trash-alt"></i></button>' +
                    '<button class="btn btn-info btn-detalles btn-sm"><i class="fas fa-search"></i></button>', 
                "orderable": false,
                "searchable": false,
                "width": "120px"
            }
        ],
        //Ordenar
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            //Configuracion para exportar el excel 
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Telas',
                exportOptions: {
                    columns: [2, 3, 4, 5,6]
                }
            }, 'pageLength'
        ],
        //Lenguaje sdata table
        //SE envia en español por medio de la url 
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idProducto)
    $("#txtCodigoBarra").val(modelo.codigoBarra)
    $("#txtDescripcion").val(modelo.descripcion)
    $("#txtStock").val(modelo.stock)
    $("#txtPrecio").val(modelo.precio)
    $("#txtImagen").val("")
    $("#cboFamilia").val(modelo.idCategoria == 0 ? $("#cboFamilia option:first").val() : modelo.idCategoria)
    $("#cboEstado").val(modelo.esActivo)

    $("#imgTela").attr("src", modelo.urlImagen)

    $("#modalData").modal("show")
}


function mostrarModalInfo(modelo = MODELO_BASE) {
    $("#imgTela2").attr("src", modelo.urlImagen)

    var titulo = document.getElementById("nombreTela")
    titulo.textContent = modelo.descripcion
    var stock = document.getElementById("stock")
    stock.textContent = "Stock: " + modelo.stock
    var precio = document.getElementById("precio")
    precio.textContent = "Precio: " + modelo.precio


    $("#modalInfo").modal("show")
}



$("#btnNuevo").click(function () {
    mostrarModal()
})


$("#btnGuardar").click(function () {

    //debugger;
    const inputs = $("input.input-validar").serializeArray();
    const inputsSinValor = inputs.filter((item) => item.value.trim() == "")

    if (inputsSinValor.length > 0) {
        const mensaje = `Debe completar el campo: "${inputsSinValor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name = "${inputsSinValor[0].name}"]`).focus()
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idProducto"] = parseInt($("#txtId").val())
    modelo["codigoBarra"] = $("#txtCodigoBarra").val()
    modelo["descripcion"] = $("#txtDescripcion").val()
    modelo["idCategoria"] = $("#cboFamilia").val()
    modelo["stock"] = $("#txtStock").val()
    modelo["precio"] = $("#txtPrecio").val()
    modelo["esActivo"] = $("#cboEstado").val()

    const inputFoto = document.getElementById("txtImagen")

    const formData = new FormData();

    formData.append("imagen", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show")

    if (modelo.idProducto == 0) {
        fetch("/Tela/Crear", {
            method: "POST",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide")
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    swal("Registro satisfactorio!", "La Tela se registro correctamente", "success")
                    inputFoto.value = ""
                } else {
                    swal("Error!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Tela/Editar", {
            method: "PUT",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide")
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalData").modal("hide")
                    swal("Registro editado!", "La Tela se edito correctamente", "success")
                    inputFoto.value = ""
                } else {
                    swal("Error!", responseJson.mensaje, "error")
                }
            })

    }
})

let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {
    //Recoge la informacion que tiene la row
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
})


$("#tbdata tbody").on("click", ".btn-detalles", function () {
    //Recoge la informacion que tiene la row
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    mostrarModalInfo(data);
})

$("#tbdata tbody").on("click", ".btn-eliminar", function () {


    let fila;
    //Recoge la informacion que tiene la row
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();
    swal({
        title: "¿Esta seguro?",
        text: `Eliminar la Tela "${data.descripcion}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, eliminar",
        cancelBurronText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`/Tela/Eliminar?idTela=${data.idProducto}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw()
                            swal("Listo!", "La tela fue eliminada", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    )
})
