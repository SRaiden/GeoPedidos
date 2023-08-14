const modelBase = {
    id: 0,
    nombreEmpresa: "",
    confirmar: 1,
    mailAvisoPedido: "",
    Estado: ""
}

let tablaData;

$(document).ready(function () {
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Empresas/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id",  "searchable": false },
            { "data": "nombreEmpresa" },
            {
                "data": "estado", render: function (data) {
                    if (data == "activo") {
                        return '<span class="badge rounded-pill bg-success" name="estadoEmpresa">Activo</span>'
                    }
                    else {
                        return '<span class="badge rounded-pill bg-danger" name="estadoEmpresa">Inactivo</span >'
                            
                    }
                }
            },
            {
                "data": "confirmar", render: function (data) {
                    if (data == 1) {
                        return '<span class="badge bg-success">Confirmado</span>'
                    }
                    else {
                        return '<span class="badge bg-danger"> Sin Confirmar </span >'
                    }
                }
            },
            { "data": "mailAvisoPedido" },
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
                filename: 'Reporte Empresas',
                exportOptions: {
                    columns: [0,1,2]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
        initComplete: function () {
            var table = $("#tbdata").DataTable();
            var data = table.data();

            for (var p = 0; p < data.length; p++) {
                var row = table.row(p);
                var estado = ($(row.node()).find('[name="estadoEmpresa"]').text());

                if (estado == "Inactivo") {
                    $(row.node()).find('.btn-eliminar').hide();
                }
            }
        }
    })
});

function mostrarModal(texto, modelo = modelBase) {
    $("#txtId").val(modelo.id);
    $("#txtEmpresa").val(modelo.nombreEmpresa);
    $("#cboConfirmado").val(modelo.confirmar);
    $("#txtEmail").val(modelo.mailAvisoPedido);
    $("#titulo").text(texto);

    $("#modalData").modal("show");
};

// CERRAR MODAL
$("#CerrarModal").click(function () {
    $("#modalData").modal("hide");
});

// AGREGAR
$("#btnNuevo").click(function () {
    mostrarModal("Agregar Empresa");
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

    if (data.estado == "activo") {
        //console.log(data);
        mostrarModal("Editar Empresa", data);
    } else {
        swal("Lo sentimos!", "Esta empresa fue dada de baja y no se puede editar", "error")
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
        title: "¿Esta seguro de querer dar de baja la empresa?",
        text: `Dar de baja la empresa: "${data.nombreEmpresa}"`,
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

                fetch(`/Empresas/Eliminar?id=${data.id}`, {  // ELIMINAR USUARIO
                    method: "POST"
                })
                .then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        //tablaData.row(fila).remove().draw() // actualizamos la fila seleccionada anteriormente
                        swal("Listo!", "La empresa fue dado de baja", "success")
                    } else {
                        swal("Lo sentimos!", responseJson.mensaje, "error")
                    }
                })
            }
        }
    )
});

// GUARDAR (TANTO PARA AGREGAR Y EDITAR)
$("#btnGuardar").click(function () {

    // VALIDACIONES
    if ($("#txtEmpresa").val().trim() == "") {
        toastr.warning("", "Debe de completar el campo: Empresa");
        $("#txtEmpresa").focus()
        return;
    }

    // si salio todo bien guardamos la info en la clase
    const modelo = structuredClone(modelBase);
    modelo["id"] = parseInt($("#txtId").val())
    modelo["nombreEmpresa"] = $("#txtEmpresa").val()
    modelo["confirmar"] = parseInt($("#cboConfirmado").val())
    modelo["mailAvisoPedido"] = $("#txtEmail").val()
    modelo["Estado"] = "activo"

    // ejecutamos un gif de cargando en el JS LoadingOverlay (para mas estetica y se vea bonito)
    $("#modalData").find("div.modal-content").LoadingOverlay("show")

    if (modelo.id == 0) {
        fetch("/Empresas/Crear", {
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
                swal("Listo!", "La empresa fue creada", "success")
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        });
    } else {
        fetch("/Empresas/Editar", {
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
                swal("Listo!", "La empresa fue Editada", "success")
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        })
    }
});
