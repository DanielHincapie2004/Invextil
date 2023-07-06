const MODELO_BASE = {
    idMuestra: 0,
    idTela: 0,
    documentoCliente: "",
    nombreCliente: "",
    direccion: "",
    fecha: "",
}

let tablaData;

$(document).ready(function () {


    fetch("/Tela/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboTela").append(
                        $("<option>").val(item.idProducto).text(item.descripcion)
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
            "url": '/Muestras/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idMuestra", "visible": false, "searchable": false },
            { "data": "tela" },
            { "data": "documentoCliente" },
            { "data": "nombreCliente" },
            { "data": "direccion" },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm mr-2"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "100px"
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
                filename: 'Reporte Citas',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5]
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
    $("#txtId").val(modelo.idMuestra)
    $("#cboTela").val(modelo.idTela == 0 ? $("#cboEmpleado option:first").val() : modelo.idTela)
    $("#txtDocumentoCliente").val(modelo.documentoCliente)
    $("#txtNombreCliente").val(modelo.nombreCliente)
    $("#txtDireccion").val(modelo.direccion)
    $("#modalData").modal("show")
}

$("#btnNuevo").click(function () {
    mostrarModal()
})


$("#btnGuardar").click(function () {

   // debugger;
    const inputs = $("input.input-validar").serializeArray();
    const inputsSinValor = inputs.filter((item) => item.value.trim() == "")

    if (inputsSinValor.length > 0) {
        const mensaje = `Debe completar el campo: "${inputsSinValor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name = "${inputsSinValor[0].name}"]`).focus()
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idMuestra"] = parseInt($("#txtId").val())
    modelo["idTela"] = $("#cboTela").val()
    modelo["documentoCliente"] = $("#txtDocumentoCliente").val()
    modelo["nombreCliente"] = $("#txtNombreCliente").val()
    modelo["direccion"] = $("#txtDireccion").val()



    const formData = new FormData();
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show")

    if (modelo.idMuestra == 0) {
        fetch("/Muestras/Crear", {
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
                    swal("Registro satisfactorio!", "La Muestra se solicito correctamente", "success")
                    location.reload();
                } else {
                    swal("Error!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Muestras/Editar", {
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
                    swal("Registro editado!", "La Cita se reagendo correctamente", "success")
                    location.reload();
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
        text: `Eliminar la muestra`,
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

                fetch(`/Muestras/Eliminar?idMuestra=${data.idMuestra}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw()
                            swal("Listo!", "La muestra Eliminada", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    )
})