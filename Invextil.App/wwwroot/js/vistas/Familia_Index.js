const MODELO_BASE = {
    idCategoria: 0,
    descripcion: "",
    esActivo: 1,
}

let tablaData;

$(document).ready(function () {

    //Selecciona la tabla 
    tablaData = $('#tbdata').DataTable({
        //Se trabaaj de moda responsive 
        responsive: true,
        //El metodo ajax nos sirve para traer toda la informacion
        "ajax": {
            "url": '/Familia/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idCategoria", "visible": false, "searchable": false },
            { "data": "descripcion" },
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
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
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
                filename: 'Reporte Familias',
                exportOptions: {
                    columns: [1,2]
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
    $("#txtId").val(modelo.idCategoria)
    $("#txtDescripcion").val(modelo.descripcion)
    $("#cboEstado").val(modelo.esActivo)
    $("#modalData").modal("show")
}

$("#btnNuevo").click(function () {
    mostrarModal()
})

$("#btnGuardar").click(function () {

    //debugger;
    if ($("#txtDescripcion").val().trim() == "") {
        toastr.warning("", "Debe completar el campo Descripcion")
        $("#txtDescripcion").focus()
        return;
    }


    const modelo = structuredClone(MODELO_BASE);
    modelo["idCategoria"] = parseInt($("#txtId").val())
    modelo["descripcion"] = $("#txtDescripcion").val()
    modelo["esActivo"] = $("#cboEstado").val()

    $("#modalData").find("div.modal-content").LoadingOverlay("show")

    if (modelo.idCategoria == 0) {
        fetch("/Familia/Crear", {
            method: "POST",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide")
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    swal("Registro satisfactorio!", "La Familia de tela se registro correctamente", "success")
                } else {
                    swal("Error!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Familia/Editar", {
            method: "PUT",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(modelo)
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
                    swal("Registro editado!", "La Familia de tela se edito correctamente", "success")
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
        text: `Eliminar la familia "${data.descripcion}"`,
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

                fetch(`/Familia/Eliminar?idFamilia=${data.idCategoria}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw()
                            swal("Listo!", "La familia fue eliminada", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    )
})
