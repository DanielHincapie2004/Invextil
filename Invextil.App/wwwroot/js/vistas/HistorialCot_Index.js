const VISTA_BUSQUEDA = {
    busquedaFecha: () => { 
        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").show()
        $(".busqueda-venta").hide()
    },

    busquedaVenta: () => {
        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").hide()
        $(".busqueda-venta").show()
    }
}


$(document).ready(function () {

    VISTA_BUSQUEDA["busquedaFecha"]()

    $.datepicker.setDefaults($.datepicker.regional["es"])
    $("#txtFechaInicio").datepicker({dateFormat:"dd/mm/yy"})
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })
})

$("#cboBuscarPor").change(function () {

    if ($("#cboBuscarPor").val() == "fecha") {
        VISTA_BUSQUEDA["busquedaFecha"]()
    } else {
        VISTA_BUSQUEDA["busquedaVenta"]()
    }

})

$("#search").click(function () {    
    if ($("#cboBuscarPor").val() == "fecha") {
        if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
            toastr.warning("", "Debes ingresar las fechas")
            return;
        }
    } else {
        if ($("#txtNumeroVenta").val().trim() == "") {
            toastr.warning("", "Debes ingresar el numero de su cotizacion")
            return;
        }
    }

    let numeroVenta = $("#txtNumeroVenta").val()
    let fechaInicio = $("#txtFechaInicio").val()
    let fechafin = $("#txtFechaFin").val()

    $(".car-body").find("div.row").LoadingOverlay("show");

    fetch(`/Venta/Historial?numeroVenta=${numeroVenta}&fechaInicio=${fechaInicio}&fechaFin=${fechafin}`)
        .then(response => {
            $(".car-body").find("div.row").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            $("#tbventa tbody").html("");
            if (responseJson.length > 0) {
                responseJson.forEach((venta) => {
                    $("#tbventa tbody").append(
                        $("<tr>").append(
                            $("<td>").text(venta.fechaRegistro),
                            $("<td>").text(venta.numeroVenta),
                            $("<td>").text(venta.tipoDocumentoVenta),
                            $("<td>").text(venta.documentoCliente),
                            $("<td>").text(venta.nombreCliente),
                            $("<td>").text(parseFloat(venta.total) / 100),
                            $("<td>").append(
                                $("<button>").addClass("btn btn-info btn-sm").append(
                                    $("<i>").addClass("fas fa-eye")
                                ).data("venta", venta)
                            )
                            
                        )
                    )
                    console.log(venta.detalleVenta) 
                })
            }
            
        })
    
})

$("#tbventa tbody").on("click", ".btn-info", function () {
    let d = $(this).data("venta")
    let total =  parseFloat(d.total)/100
    console.log(total)
    $("#txtFechaRegistro").val(d.fechaRegistro)
    $("#txtNumVenta").val(d.numeroVenta)
    $("#txtUsuarioRegistro").val(d.empleado)
    $("#txtTipoDocumento").val(d.tipoDocumentoVenta)
    $("#txtDocumentoCliente").val(d.documentoCliente)
    $("#txtNombreCliente").val(d.nombreCliente)
    $("#txtSubTotal").val( parseFloat(d.subTotal)/100)
    $("#txtIGV").val(parseFloat(d.impuestoTotal)/100)
    $("#txtTotal").val(parseFloat(d.total)/100)

    $("#tbProductos tbody").html("");
    d.detalleVenta.forEach((item) => {
        $("#tbProductos tbody").append(
            $("<tr>").append(
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total)
            )
        )
    })

    $("#linkImprimir").attr("href", `/Venta/MostrarPDFVenta?numeroVenta=${d.numeroVenta}`)

    $("#modalData").modal("show")
})