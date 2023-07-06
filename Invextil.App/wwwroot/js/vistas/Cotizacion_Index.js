
let impuesto = 19.00;
$(document).ready(function () {
    fetch("/Venta/ListaTipoDocumento")
    .then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
        if (responseJson.length > 0) {
            responseJson.forEach((item) => {
                $("#cboTipoDocumentoVenta").append(
                    $("<option>").val(item.idTipoDocumentoVenta).text(item.descripcion)
                )
            })
        }
    })



    $("#cboBuscarTela").select2({
        ajax: {
            url: "/Venta/ObtenerTela",
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
        delay: 250,
            data: function (params) {
                return {
                    busqueda: params.term
                };
            },
            processResults: function (data) {
                return {
                    results: data.map((item) => ({
                        id: item.idProducto,
                        text: item.descripcion,
                        categoria: item.nombreCategoria,
                        urlImagen: item.urlImagen,
                        precio: parseFloat(item.precio)
                    }))
                };
            },
        },
        language: "es",
        placeholder: 'Buscar tela',
        minimumInputLength: 1,
        templateResult: formatoResultado
    });

   
})

function formatoResultado(data) {
    if (data.loading)
        return data.text;

    var contenedor = $(
        `
        <table width="100%">
            <tr>
                <td style="width:60px">
                    <img style="height:60px;width:60px;margin-right:10px" src="${data.urlImagen}">
                </td>
                <td>
                    <p style="font-weight: bolder; margin:2px">${data.text}</p>
                    <p style="margin:2px">${data.categoria}</p>
                </td>
            </tr>
        </table >
        `
    );
    return contenedor;
}


$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus();
})

let TelasCotizacion = [];
$("#cboBuscarTela").on("select2:select", function (e) {

    const data = e.params.data;
    let tela_encontrada = TelasCotizacion.filter(p => p.idProducto == data.id)
    if (tela_encontrada.length > 0) {
        console.log(tela_encontrada.length)
        $("#cboBuscarTela").val("").trigger("change")
        toastr.warning("", "La tela ya fue agregada")
        return false
    }

    swal({
        title: data.text,
        text: data.categoria,
        imageUrl: data.urlImagen,
        type:"input",
        showCancelButton: true,
        closeOnConfirm: false,
        inputPlaceholder:"Ingrese la cantidad"
    },
        function (valor) {
            if (valor === false) return false;
            if (valor === "") {
                toastr.warning("", "Se requiere la cantidad a cotizar");
                return false;
            }
            if (isNaN(parseInt(valor))) {
                toastr.warning("", "Debe ingresar un valor numerico");
                return false;
            }

            let tela = {
                idProducto: data.id,
                descripcionProducto: data.text,
                categoriaProducto: data.categoria,
                cantidad: parseInt(valor),
                precio: data.precio.toString(),
                total: (parseFloat(valor) * data.precio).toString()
            }

            TelasCotizacion.push(tela)
            mostrarTelaPrecio();
            $("#cboBuscarTela").val("").trigger("change")
            swal.close()
        }
    )
})


function mostrarTelaPrecio() {
    let total = 0;
    let iva = 0;
    let subtotal = 0;
    let porcentaje = 19.00 / 100;

    $("#tbTela tbody").html("")
    TelasCotizacion.forEach((item) => {

        total = total + parseFloat(item.total)

        $("#tbTela tbody").append(
            $("<tr>").append(
                $("<td>").append(
                    $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
                        $("<i>").addClass("fas fa-trash-alt")
                    ).data("idtela", item.idProducto)
                ),
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total)
            )
        )
    })

    iva = 
    subtotal = total / (1 + porcentaje);
    igv = total - subtotal;

    //Solo 2 decimales
    $("#txtSubTotal").val(subtotal.toFixed(2))
    $("#txtIGV").val(igv.toFixed(2))
    $("#txtTotal").val(total.toFixed(2))
}


$(document).on("click","button.btn-eliminar", function () {
    const _idTela = $(this).data("idtela")
    TelasCotizacion = TelasCotizacion.filter(p => p.idProducto != _idTela);
    mostrarTelaPrecio();
})

$("#btnTerminarVentar").click(function () {
    if (TelasCotizacion.length < 1) {
        toastr.warning("", "Debe seleccionar telas");
        return;
    }

    const vmDetalleVenta = TelasCotizacion;
    const cotizacion = {
        idTipoDocumentoVenta: $("#cboTipoDocumentoVenta").val(),
        documentoCliente: $("#txtDocumentoCliente").val(),
        nombreCliente: $("#txtNombreCliente").val(),
        subTotal: $("#txtSubTotal").val(),
        impuestoTotal: $("#txtIGV").val(),
        total: $("#txtTotal").val(),
        DetalleVenta: vmDetalleVenta
    }
    
    $("#btnTerminarVentar").LoadingOverlay("show")
    console.log(JSON.stringify(cotizacion))
    console.log(JSON.stringify(vmDetalleVenta))
    fetch("/Venta/RegistrarCotizacion", {
        method: "POST",
        headers: { "Content-Type": "application/json;charset=utf-8" },
        body: JSON.stringify(cotizacion)
        
    })
        .then(response => {
            $("#btnTerminarVentar").LoadingOverlay("hide")
            return response.ok ? response.json() : Promise.reject(response);

        })
        .then(responseJson => {
            
            if (responseJson.estado) {
                TelasCotizacion = [];
                mostrarTelaPrecio();

                $("#txtDocumentoCliente").val("")
                $("#txtNombreCliente").val("")
                $("#cboTipoDocumentoVenta").val($("#cboTipoDocumentoVenta option:first").val())

                swal("La cotizacion fue realizada", `Numero de Cotizacion: ${responseJson.objeto.numeroVenta}`, "success")
            } else {
                swal("Error", "No se pudo registrar", "error")
            }
        })
})