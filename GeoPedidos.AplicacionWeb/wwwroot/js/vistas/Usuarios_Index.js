const modelBase = {
    id: 0,
    nombre: "",
    apellido: "",
    email: "",
    idEmpresa: 0,
    idSucursal: 0,
    active: 0,
    rol: "",
    contraseña: "",
    created: "",
    modified: "",
    okLogin: 0
}

let tablaData;

$(document).ready(function () {
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Usuarios/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id", "searchable": false },
            {
                "data": null, render: function (data) {
                    return data.nombre + " " +  data.apellido
                }
            },
            { "data": "email" },
            {
                "data": "active", render: function (data) {
                    if (data == 1) {
                        return '<span class="badge bg-success">Activo</span>'
                    }
                    else {
                        return '<span class="badge bg-danger">Inactivo</span >'
                    }
                }
            },
            {
                "data": "rol", render: function (data) {
                    if (data == "admin") {
                        return '<span class="badge bg-info">Admin</span>'
                    }
                    else {
                        return '<span class="badge bg-secondary"> User </span >'
                    }
                }
            },
            
            { "data": "nombreSucursal" },
            { "data": "nombreEmpresa" },
            {
                "defaultContent": '<div class="dropdown">' +
                    '<button type="button" class= "btn p-0 dropdown-toggle hide-arrow" data-bs-toggle="dropdown" >' +
                    '<i class="bx bx-dots-vertical-rounded"></i>' +
                    '</button >' +
                    '<div class="dropdown-menu">' +
                    '<a class="dropdown-item btn-editar"><i class="bx bx-trash me-1"></i> Editar</a>' +
                    '<a class="dropdown-item btn-eliminar"><i class="bx bx-trash me-1"></i> Dar de Baja</a>' +
                    '</div>' +
                    '</div > ',
                "orderable": false,
                "searchable": false
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Usuarios',
                exportOptions: {
                    columns: [0, 1, 2, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    })

    fetch("/Usuarios/ObtenerTodasEmpresas")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => { // creamos un response a la bbdd
            $("#cboEmpresas").append(
                $("<option>").val(0).text("- ADMINISTRADOR DE EMPRESAS -")
            )
            if (responseJson.length > 0) { // encontro datos en la bbdd?
                responseJson.sort((a, b) => a.nombreEmpresa.localeCompare(b.nombreEmpresa));
                responseJson.forEach((item) => {
                    $("#cboEmpresas").append( // hacemos llamado a la etiqueta cbroRol
                        // cargamos el comboBox en tiempo de ejecucion con las columnas idrol (como value) y descripcion (como text)
                        $("<option>").val(item.id).text(item.nombreEmpresa)
                    )
                })
            }
        })

    fetch(`/Usuarios/ObtenerSucursalEmpresa?idEmpresa=1`, {  // ELIMINAR USUARIO
        method: "GET"
    })
    .then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => { // creamos un response a la bbdd
        $('#cboSucursales').empty();
        $("#cboSucursales").append(
            $("<option>").val(0).text("- ADMINISTRADOR DE SUCURSALES -") // OPTION DE ADMINISTRADOR
        )
        if (responseJson.length > 0) { // encontro datos en la bbdd?
            responseJson.sort((a, b) => a.nombreSucursal.localeCompare(b.nombreSucursal));
            responseJson.forEach((item) => {
                $("#cboSucursales").append( // hacemos llamado a la etiqueta cbroRol
                    $("<option>").val(item.id).text(item.nombreSucursal)
                )
            })
        }
    })

    $("#txtUsuario").val("user")
});

//MODIFICAR SUCURSAL SEGUN EMPRESA ELEGIDA
$("#cboEmpresas").on("change", function () {
    var optionEmpresa = $(this).find(":selected").val()
    cargarSucursales(optionEmpresa)
})

$("#cboSucursales").on("change", function () {
    var optionSucursal = $(this).find(":selected").val()
    if (optionSucursal == 0) $("#txtUsuario").val("admin")
    else $("#txtUsuario").val("user")
})

function cargarSucursales(idEmpresa, idSucursal = null) {
    fetch(`/Usuarios/ObtenerSucursalEmpresa?idEmpresa=${idEmpresa}`, {  // ELIMINAR USUARIO
        method: "GET"
    })
    .then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => { // creamos un response a la bbdd
        $('#cboSucursales').empty();
        $("#cboSucursales").append(
            $("<option>").val(0).text("- ADMINISTRADOR DE SUCURSALES -"), // OPTION DE ADMINISTRADOR
        )
        $("#txtUsuario").val("Admin")
        if (responseJson.length > 0) { // encontro datos en la bbdd?
            responseJson.sort((a, b) => a.nombreSucursal.localeCompare(b.nombreSucursal));
            responseJson.forEach((item) => {
                $("#cboSucursales").append( // hacemos llamado a la etiqueta cbroRol
                    $("<option>").val(item.id).text(item.nombreSucursal)
                )
            })
        }
        if (idSucursal != null) {
            $("#cboSucursales").val(idSucursal); // Selecciono el select
        }
    })
}

function mostrarModal(texto, modelo = modelBase) {
    $("#txtId").val(modelo.id);
    $("#txtNombre").val(modelo.nombre);
    $("#txtApellido").val(modelo.apellido);
    $("#txtEmail").val(modelo.email);

    if (texto == "Agregar Usuario") {
        $("#cboEmpresas").val(1);
        $("#cboSucursales").val(1);
        $("#cboEstado").val(1);
        $("#cboUsuario").val("user");
    } else {
        $("#cboEmpresas").val(modelo.idEmpresa);
        cargarSucursales(modelo.idEmpresa, modelo.idSucursal)
        $("#cboEstado").val(modelo.active);
        $("#cboUsuario").val(modelo.rol);
    }

    $("#txtPassword").val(modelo.contraseña);
    $("#txtRepetir").val(modelo.contraseña);
    $("#titulo").text(texto);
    $("#modalData").modal("show");
};

// CERRAR MODAL
$("#CerrarModal").click(function () {
    $("#modalData").modal("hide");
});

// AGREGAR
$("#btnNuevo").click(function () {
    mostrarModal("Agregar Usuario");
});

// EDITAR
let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();

    if (data.active == 1) {
        //console.log(data);
        mostrarModal("Editar Usuario", data);
    } else {
        swal("Lo sentimos!", "Esta usuario fue dada de baja y no se puede editar", "error")
    }

});

// ELIMINAR
$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();
    swal({
        title: "¿Esta seguro de querer dar de baja al usuario?",
        text: `Dar de baja al Usuario: "${data.nombre}"` + ` "${data.apellido}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "SI, dar de baja",
        cancelButtonText: "Volver",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`/Usuarios/Eliminar?id=${data.id}`, {  // ELIMINAR USUARIO
                    method: "POST"
                })
                .then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        //tablaData.row(fila).remove().draw() // actualizamos la fila seleccionada anteriormente
                        swal("Listo!", "El usuario fue dado de baja", "success")
                    } else {
                        swal("Lo sentimos!", responseJson.mensaje, "error")
                    }
                })
            }
        }
    )
});


// GUARDAR (TANTO PARA AGREGAR COMO EDITAR)
$("#btnGuardar").click(function () {

    // VALIDACIONES
    if ($("#txtNombre").val().trim() == "") {
        toastr.warning("", "Debe de completar el campo: Nombre");
        $("#txtNombre").focus()
        return;
    }
    if (!isNaN($("#txtNombre").val().trim())) {
        toastr.warning("", "Ingrese solo letras en el campo: Nombre");
        $("#txtNombre").focus()
        return;
    }
    //--//
    if ($("#txtApellido").val().trim() == "") {
        toastr.warning("", "Debe de completar el campo: Apellido");
        $("#txtApellido").focus()
        return;
    }
    if (!isNaN($("#txtApellido").val().trim())) {
        toastr.warning("", "Ingrese solo letras en el campo: Apellido");
        $("#txtApellido").focus()
        return;
    }
    //--//
    if ($("#txtEmail").val().trim() == "") {
        toastr.warning("", "Debe de completar el campo: Email");
        $("#txtEmail").focus()
        return;
    }
    var emailRegex = /^[-\w.%+]{1,64}@(?:[A-Z0-9-]{1,63}\.){1,125}[A-Z]{2,63}$/i;
    if (!emailRegex.test($("#txtEmail").val().trim())) {
        toastr.warning("", "Formato de Email no valido");
        $("#txtEmail").focus()
        return;
    }
    //--//
    if ($("#txtPassword").val().trim() == "") {
        toastr.warning("", "Debe de completar el campo: Password");
        $("#txtPassword").focus()
        return;
    }
    if ($("#txtPassword").val().trim() != $("#txtRepetir").val().trim()) {
        toastr.warning("", "Las contraseñas no coinciden");
        $("#txtPassword").focus()
        return;
    }

    // si salio todo bien guardamos la info en la clase
    const modelo = structuredClone(modelBase);
    modelo["id"] = parseInt($("#txtId").val())
    modelo["nombre"] = $("#txtNombre").val()
    modelo["apellido"] = $("#txtApellido").val()
    modelo["email"] = $("#txtEmail").val()
    modelo["idEmpresa"] = parseInt($("#cboEmpresas").val())
    modelo["idSucursal"] = parseInt($("#cboSucursales").val())
    modelo["active"] = parseInt($("#cboEstado").val())
    modelo["rol"] = $("#txtUsuario").val()
    modelo["contraseña"] = $("#txtPassword").val()

    // ejecutamos un gif de cargando en el JS LoadingOverlay (para mas estetica y se vea bonito)
    $("#modalData").find("div.modal-content").LoadingOverlay("show")

    if (modelo.id == 0) {

        const fecha = new Date();
        modelo["created"] = fecha
        modelo["modified"] = fecha

        fetch("/Usuarios/Crear", {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    swal("Listo!", "El Usuario fue creado", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            });
    } else {

        const fecha = new Date();
        
        modelo["modified"] = fecha

        fetch("/Usuarios/Editar", {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(filaSeleccionada).draw(responseJson.objeto).draw(false); // actualizamos la fila seleccionada anteriormente
                    filaSeleccionada = null;
                    $("#modalData").modal("hide")
                    swal("Listo!", "El Usuario fue Editado", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    }
});