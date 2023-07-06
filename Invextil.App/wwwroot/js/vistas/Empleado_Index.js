

const MODELO_BASE = {
    idEmpleado: 0,
    nombre: "",
    correo: "",
    idRol: 0,
    esActivo: 1,
    urlFoto:""
}

let tablaData;

$(document).ready(function () {


    fetch("/Usuario/ListaRoles")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboRol").append(
                        $("<option>").val(item.idRol).text(item.descripcion)
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
             "url": '/Usuario/Lista',
             "type": "GET",
             "datatype": "json"
         },
         "columns": [
             { "data": "idEmpleado", "visible": false, "searchable": false },
             {
                 "data": "urlFoto", render: function (data) {
                     return `<img style="height:60px" src=${data} class="rounded mx-auto d-block" />`;
                 }
             },
             { "data": "nombre" },
             { "data": "correo" },
             { "data": "nombreRol" },
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
                filename: 'Reporte Empleados',
                exportOptions: {
                    columns: [2,3,4,5]
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
    $("#txtId").val(modelo.idEmpleado)
    $("#txtNombre").val(modelo.nombre)
    $("#txtCorreo").val(modelo.correo)
    $("#cboRol").val(modelo.idRol == 0 ? $("#cboRol option:first").val() : modelo.idRol)
    $("#cboEstado").val(modelo.esActivo)

    $("#imgUsuario").attr("src", modelo.urlFoto)

    $("#modalData").modal("show")
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
    modelo["idEmpleado"] = parseInt($("#txtId").val())
    modelo["nombre"] = $("#txtNombre").val()
    modelo["correo"] = $("#txtCorreo").val()
    modelo["idRol"] = $("#cboRol").val()
    modelo["esActivo"] = $("#cboEstado").val()

    const inputFoto = document.getElementById("txtFoto")

    const formData = new FormData();

    formData.append("foto", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show")

    if (modelo.idEmpleado == 0) {
        fetch("/Usuario/Crear", {
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
                    swal("Registro satisfactorio!", "El empleado se registro correctamente", "success")
                    inputFoto.value = ""
                } else {
                    swal("Error!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Usuario/Editar", {
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
                    swal("Registro editado!", "El empleado se edito correctamente", "success")
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
        text: `Eliminar al empleado "${data.nombre}"`,
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

                fetch(`/Usuario/Eliminar?idEmpleado=${data.idEmpleado}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw()
                            swal("Listo!", "El empleado fue eliminado", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    )
})
