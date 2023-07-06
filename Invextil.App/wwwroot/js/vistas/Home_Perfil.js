
$(document).ready(function () {
    $(".container-fluid").LoadingOverlay("show");

    fetch("/Home/ObtenerUsuario")
        .then(response => { 
            $(".container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response)
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto

                $("#imgFoto").attr("src", d.urlFoto)
                $("#txtNombre").val(d.nombre)
                $("#txtCorreo").val(d.correo)
                $("#txtRol").val(d.nombreRol)
            } else {
                swal("Error", responseJson.mensaje,"error")
            }
        })


})

$("#btnCambiarClave").click(function () {
    const inputs = $("input.input-validar").serializeArray();
    const inputsSinValor = inputs.filter((item) => item.value.trim() == "")

    if (inputsSinValor.length > 0) {
        const mensaje = `Debe completar el campo: "${inputsSinValor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name = "${inputsSinValor[0].name}"]`).focus()
        return;
    }

    if ($("#txtClaveNueva").val().trim() != $("#txtConfirmarClave").val().trim()) {
        toastr.warning("", "Las claves tienen que ser iguales")
        return
    }

    let modelo = {
        claveActual: $("#txtClaveActual").val().trim(),
        claveNueva: $("#txtClaveNueva").val().trim()
    }

    fetch("/Home/CambiarClave", {
        method: "POST",
        headers: { "Content-Type": "application/json;charset=utf-8" },
        body: JSON.stringify(modelo)
    })
        .then(response => {
            $("showSweetAlert").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response)
        })
        .then(responseJson => {
            if (responseJson.estado) {
                swal("Listo", "Su contraseña fue modificada", "success")
                $("input.input-validar").val("");
            } else {
                swal("Error", responseJson.mensaje, "error")
            }
        })

})