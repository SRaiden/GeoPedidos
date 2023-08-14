$(document).ready(function () {
    $(".card-body").LoadingOverlay("show");

    fetch("/Home/ObtenerUser")
        .then(response => {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto

                $("#txtNombre").val(d.nombre);
                $("#txtApellido").val(d.apellido);
                $("#txtEmail").val(d.email);
                $("#txtRol").val(d.rol);
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        })
});

let modelBase = {
    contraseña: "",
    contraseñaNueva: "",
    nombre: "",
    apellido: ""
}

$("#guardar").click(function () {

    const modelo = structuredClone(modelBase);;

    if ($("#passwordActual").val().trim() != "") {
        //VALIDACIONES
        const inputs = $("input.input-validar").serializeArray(); // llamo a todos los imputs con la clase input-validar
        const inputsSinValor = inputs.filter((item) => item.value.trim() == "") // verifico aquellos que no ingresaron datos

        if (inputsSinValor.length > 0) {
            const mensaje = `Debe completar el campo: "${inputsSinValor[0].name}"`;
            toastr.warning("", mensaje) // llamo al JS toastr que tiene para un mensaje lindo de error
            $(`input[name="${inputsSinValor[0].name}"]`).focus() // posiciono el cursor sobre el txt que encontro
            return;
        }

        if ($("#passwordNueva").val().trim() != $("#passwordRepetir").val().trim()) {
            toastr.warning("", "Las contraseñas no coinciden");
            return;
        }

        // guardar nombre apellido y clave
        
        modelo.contraseña = $("#passwordActual").val().trim();
        modelo.contraseñaNueva = $("#passwordNueva").val().trim();
        modelo.nombre = $("#txtNombre").val();
        modelo.apellido = $("#txtApellido").val();

    } else {
        // solo nombre y apellido
        modelo.nombre = $("#txtNombre").val();
        modelo.apellido = $("#txtApellido").val();
    }


    swal({
        title: "¿Desea guardar los cambios?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "SI",
        cancelButtonText: "NO",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch("/Home/GuardarPerfil", {  // ELIMINAR USUARIO
                    method: "POST",
                    headers: { "Content-Type": "application/json; charset=utf-8" },
                    body: JSON.stringify(modelo)
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            swal("Listo!", "Los cambios fueron guardados", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    )
});
