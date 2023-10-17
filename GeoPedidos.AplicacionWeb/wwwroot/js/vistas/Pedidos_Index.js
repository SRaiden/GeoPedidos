const modelBase = {
    idUsuario: 0,
    idSucursal: 0,
    idEmpresa: 0,
    tipo: "",
    FechaDesde: "",
    FechaHasta: ""
};

let tablaData;
let tablaDataVer;
let tablaDataPedido;

// PARA LOGIN
var empLogin = $("#cboEmpresas").val();
var sucLogin = $("#cboSucursales").val();
var userLogin = $("#userLogin").val();
var rolLogin = $("#rolLogin").val();


$(document).ready(function () {
    $("#rbTodos").prop("checked", true);

    // Obtener la fecha de hoy
    const hoy = new Date();

    // Calcular la fecha de 30 días antes
    const fecha30DiasAntes = new Date(hoy);
    fecha30DiasAntes.setDate(hoy.getDate() - 30);

    // Formatear la fecha como YYYY-MM-DD
    const fechaFormateada = fecha30DiasAntes.toISOString().split('T')[0];
    const fechahoyFormateada = hoy.toISOString().split('T')[0];

    // Establecer la fecha en el campo de fecha
    $('#txtDesde').val(fechaFormateada);
    $('#txtHasta').val(fechahoyFormateada);

    if (empLogin == null && sucLogin == null) { // SOY SUPERADMIN
        fetch("/Usuarios/ObtenerTodasEmpresas")
            .then(response => {
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.length > 0) {
                    responseJson.sort((a, b) => a.nombreEmpresa.localeCompare(b.nombreEmpresa));
                    responseJson.forEach((item) => {
                        $("#cboEmpresas").append(
                            $("<option>").val(item.id).text(item.nombreEmpresa)
                        )
                    })
                }
            })
        if (empLogin == null) empLogin = "7"; 
    }
    if(empLogin != null && sucLogin == null)
    { // SOY ADMIN DE SUCURSALES
        
        fetch(`/Usuarios/ObtenerSucursalEmpresa?idEmpresa=${empLogin}`, {
            method: "GET"
        })
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => { 
            $('#cboSucursales').empty();
            $("#cboSucursales").append(
                $("<option>").val(0).text("- TODAS LAS SUCURSALES -") // OPTION DE ADMINISTRADOR
            )
            if (responseJson.length > 0) { 
                responseJson.sort((a, b) => a.nombreSucursal.localeCompare(b.nombreSucursal));
                responseJson.forEach((item) => {
                    $("#cboSucursales").append( 
                        $("<option>").val(item.id).text(item.nombreSucursal)
                    )
                })
            }
        })
    }

    // filtro
    //var filtrada = $("#Filtrada").prop("checked");
    //if (filtrada) {
    //    // DECLARACION
    //    var EmpresaElegida = $("#EmpresaElegida").val();
    //    var SucursalElegida = $("#SucursalElegida").val();
    //    var RadioElegida = $("#RadioElegida").val();
    //    var FechaDesdeElegida = $("#FechaDesdeElegida").val();
    //    var FechaHastaElegida = $("#FechaHastaElegida").val();

    //    //
    //    if (empLogin == "7" && sucLogin == null) { // SOY SUPERADMIN
    //        $("#cboEmpresas").val(EmpresaElegida);
    //        var optionEmpresa = $(this).find(":selected").val()
    //        cargarSucursales(optionEmpresa)
    //        $("#cboSucursales").val(SucursalElegida);
    //    } else if (empLogin != null && sucLogin == null) { // SOY ADMIN DE SUC
    //        $("#cboSucursales").val(SucursalElegida);
    //    }

    //    // RADIO
    //    $(RadioElegida).prop("checked", true);

    //    // RANGO FECHAS
    //    $("#txtDesde").val(FechaDesdeElegida);
    //    $("#txtHasta").val(FechaHastaElegida);

    //    // BOTONES
    //    $("#tbdata tbody tr").each(function () {

    //        var estado = $(this).find(".pendiente").text();

    //        if (estado == "Anulado") {
    //            $(this).find('.btn-eliminar').hide();
    //            $(this).find('.btn-editar').hide();
    //            $(this).find('.btn-reporte').hide();
    //        }

    //        if (estado != "Anulado" && estado != "Pendiente") {
    //            $(this).find('.btn-editar').hide();
    //            $(this).find('.btn-eliminar').hide();
    //        }

    //        // Soy Admin de suc o superAdmin??
    //        if (rolLogin == "0") {
    //            if (userLogin != "1") { // Soy admin de suc??
    //                if (estado == "Pendiente") {
    //                    $(this).find('.btn-editar').hide();
    //                    $(this).find('.btn-eliminar').show();
    //                }
    //            } else { // soy superAdmin
    //                $(this).find('.btn-editar').hide();
    //                $(this).find('.btn-eliminar').hide();
    //            }
    //        } else { // soy User
    //            $(this).find('.btn-editar').show();
    //            $(this).find('.btn-eliminar').show();
    //        } 

    //    });
    //}
});

$("#cboEmpresas").on("change", function () {
    var optionEmpresa = $(this).find(":selected").val()
    cargarSucursales(optionEmpresa)

});

function cargarSucursales(idEmpresa) {
    fetch(`/Usuarios/ObtenerSucursalEmpresa?idEmpresa=${idEmpresa}`, {  // ELIMINAR USUARIO
        method: "GET"
    })
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => { // creamos un response a la bbdd
            $('#cboSucursales').empty();
            $("#cboSucursales").append(
                $("<option>").val(0).text("- TODAS LAS SUCURSALES -"), // OPTION DE ADMINISTRADOR
            )
            if (responseJson.length > 0) { // encontro datos en la bbdd?
                responseJson.sort((a, b) => a.nombreSucursal.localeCompare(b.nombreSucursal));
                responseJson.forEach((item) => {
                    $("#cboSucursales").append( // hacemos llamado a la etiqueta cbroRol
                        $("<option>").val(item.id).text(item.nombreSucursal)
                    )
                })
            }
        });
};

$("#cboEmpresas").on("change", function () {
    busqueda();
});

$("#cboSucursales").on("change", function () {
    busqueda();
});

$('input[type=radio][name=rbReporte]').change(function () {
    busqueda();
});

$("#txtDesde").blur(function () {
    busqueda();
});

$("#txtHasta").blur(function () {
    busqueda();

});


// TERMINAR LA IMPLEMENTACION DEL DROPDOWN BTNS
$("#tbdata").on("show.bs.dropdown", ".dropdown", function () {
    var fila = $(this).closest("tr");
    var estado = fila.find(".estado").text();

    // Soy Admin de suc o superAdmin??
    if (rolLogin == "0") {
        if (userLogin != "1") { // Soy admin de suc??
            if (estado == "Pendiente") {
                fila.find(".btn-editar, .btn-eliminar").show();
            }
            else if (estado == "Anulado") {
                fila.find(".btn-editar, .btn-eliminar, .btn-reporte").hide();
            }
            else {
                fila.find(".btn-editar, .btn-eliminar").hide();
                fila.find(".btn-reporte").show();
            }
        } else { // soy superAdmin
            fila.find(".btn-editar, .btn-eliminar").hide();
        }
    } else { // soy User
        if (estado == "Pendiente") {
            fila.find(".btn-editar, .btn-eliminar, .btn-reporte").show();
        }
        else if (estado == "Anulado") {
            fila.find(".btn-editar, .btn-eliminar, .btn-reporte").hide();
        }
        else {
            fila.find(".btn-editar, .btn-eliminar").hide();
            fila.find(".btn-reporte").show();
        }
    }
});


function busqueda() {

    const modelo = structuredClone(modelBase);
    modelo["idUsuario"] = parseInt(userLogin);
    modelo["idEmpresa"] = parseInt($("#cboEmpresas").val());

    if (sucLogin == null) { // soy algun admin
        modelo["idSucursal"] = parseInt($("#cboSucursales").val())
    } else { // soy user
        modelo["idSucursal"] = sucLogin;
    }

    if ($('#rbTodos').prop('checked')) modelo["tipo"] = "todos"
    else if ($('#rbHelados').prop('checked')) modelo["tipo"] = "helado"
    else if ($('#rbProductos').prop('checked')) modelo["tipo"] = "producto"
    else if ($('#rbInsumos').prop('checked')) modelo["tipo"] = "insumo"
    else modelo["tipo"] = "pasteleria"

    modelo["FechaDesde"] = $("#txtDesde").val()
    modelo["FechaHasta"] = $("#txtHasta").val()

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": `/Pedidos/Busqueda?data=${encodeURIComponent(JSON.stringify(modelo))}`,
            "type": "POST",
            "datatype": "json"
        },
        "bDestroy": true,
        "columns": [
            { "data": "id" },
            { "data": "created" },
            { "data": "numeroPedido" },
            { "data": "tipo" },
            {
                "data": null,
                "render": function (data) {
                    return '<div class="estado">' + data.estado + '</div>';
                },
            },
            { "data": "cantidad" },
            { "data": "nombreSucursal" },
            { "data": "nombreUsuario" },
            {
                "defaultContent": '<div class="dropdown">' +
                    '<button type="button" name="dropdownList" class="btn p-0 dropdown-toggle hide-arrow" data-bs-toggle="dropdown">' +
                    '<i class="bx bx-dots-vertical-rounded"></i>' +
                    '</button >' +
                    '<div class="dropdown-menu">' +
                    '<a class="dropdown-item btn-ver"><i class="bx bx-show"></i> Ver Detalles</a>' +
                    '<a class="dropdown-item btn-editar"><i class="bx bx-pencil"></i> Editar</a>' +
                    '<a class="dropdown-item btn-eliminar"><i class="bx bx-trash me-1"></i> Dar de Baja</a>' +
                    '<a class="dropdown-item btn-reporte" id="btn-reporte"><i class="bx bx-file"></i> Reporte</a>' +
                    '</div>' +
                    '</div > ',
                "orderable": false,
                "searchable": false
            }
        ],
        order: [[0, "desc"]],
        columnDefs: [
            {
                targets: [0, 2],
                searchable: true
            },
            {
                targets: [1, 3, 4, 5, 6, 7, 8],
                searchable: false
            }
        ],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Pedidos',
                exportOptions: {
                    columns: [0, 1, 2, 5, 6]
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
                var estado = ($(row.node()).find('[class="estado"]').text());

                // Soy Admin de suc o superAdmin??
                if (rolLogin == "0") {
                    if (userLogin != "1") { // Soy admin de suc??
                        if (estado == "Pendiente") {
                            $(row.node()).find('.btn-editar').show();
                            $(row.node()).find('.btn-eliminar').show();
                        }
                        else if (estado == "Anulado") {
                            $(row.node()).find('.btn-eliminar').hide();
                            $(row.node()).find('.btn-editar').hide();
                            $(row.node()).find('.btn-reporte').hide();
                        }
                        else {
                            $(row.node()).find('.btn-editar').hide();
                            $(row.node()).find('.btn-eliminar').hide();
                            $(row.node()).find('.btn-reporte').show();
                        }
                    } else { // soy superAdmin
                        $(row.node()).find('.btn-editar').hide();
                        $(row.node()).find('.btn-eliminar').hide();
                    }
                } else { // soy User
                    if (estado == "Pendiente") {
                        $(row.node()).find('.btn-reporte').show();
                        $(row.node()).find('.btn-editar').show();
                        $(row.node()).find('.btn-eliminar').show();
                    }
                    else if (estado == "Anulado") {
                        $(row.node()).find('.btn-eliminar').hide();
                        $(row.node()).find('.btn-editar').hide();
                        $(row.node()).find('.btn-reporte').hide();
                    }
                    else {
                        $(row.node()).find('.btn-editar').hide();
                        $(row.node()).find('.btn-eliminar').hide();
                        $(row.node()).find('.btn-reporte').show();
                    }
                } 
            }
        }
    });
}


//------------- MODAL PEDIR PRODUCTO ---------------------------//

const modelPedido = {
    Codigo: "",
    Nombre: "",
    Categoria: "",
    Cantidad: 0
};

function mostrarModal(modelo = modelPedido) {
    $("#modalData").modal("show");
};

function mostrarModalVer(modelo = modelPedido) {
    $("#modalDataVer").modal("show");
};

let editarPedido = "";
let idPedidoEditar = "";
let botonElegido = "";

$("#btnHelado").click(function () {
    mostrarModal();
    cargarElementos("Helado", null, "Agregar");
    botonElegido = "helado";
    //-------------------------//
    editarPedido = "";
    idPedidoEditar = "";
});

$("#btnProducto").click(function () {
    mostrarModal();
    cargarElementos("Producto", null, "Agregar");
    botonElegido = "producto";
    //-------------------------//
    editarPedido = "";
    idPedidoEditar = "";
});

$("#btnInsumo").click(function () {
    mostrarModal();
    cargarElementos("Insumo", null, "Agregar");
    botonElegido = "insumo";
    //-------------------------//
    editarPedido = "";
    idPedidoEditar = "";
});

$("#btnPasteleria").click(function () {
    mostrarModal();
    cargarElementos("Pasteleria", null, "Agregar");
    botonElegido = "pasteleria";
    //-------------------------//
    editarPedido = "";
    idPedidoEditar = "";
});


$("#CerrarModal").click(function () {
    $("#modalData").modal("hide");
});

$("#CerrarModalVer").click(function () {
    $("#modalDataVer").modal("hide");
});

// EDITAR
let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {
    //var fila = $(this).closest("tr");
    //var id = fila.find("td:first").text();
    //var tipo = fila.find("td:nth-child(4)").text();

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();

    if (rolLogin != 0) {
        mostrarModal();
        cargarElementos(data.tipo, data.id, "Editar");
    }

});

// VER
$("#tbdata tbody").on("click", ".btn-ver", function () {
    //var fila = $(this).closest("tr");
    //var id = fila.find("td:first").text();
    //var tipo = fila.find("td:nth-child(4)").text();

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();

    mostrarModalVer();
    cargarElementos(data.tipo, data.id, "Ver");

});

// REPORTE
$("#tbdata tbody").on("click", ".btn-reporte", function () {
    //var fila = $(this).closest("tr");
    //var id = fila.find("td:first").text();

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();

    $("#Reporte").attr("href", `/Pedidos/MostrarPDFPedido?idPedido=${data.id}`)
    //$("#Reporte").attr("target", "_blank");
});

// ELIMINAR
$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    //var fila = $(this).closest("tr");
    //var numeroPedido = fila.find("td:nth-child(3)").text();


    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();
    swal({
        title: "¿Esta seguro de querer eliminar el pedido?",
        text: `Dar de baja al Pedido: "${data.numeroPedido}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "SI, eliminar el pedido",
        cancelButtonText: "Cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`/Pedidos/Eliminar?idPedido=${data.id}`, {
                    method: "POST"
                })
                .then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        swal("Listo!", "El pedido fue eliminado", "success")
                    } else {
                        swal("Lo sentimos!", responseJson.mensaje, "error")
                    }
                })
            }
        }
    )
});

function cargarElementos(elemento, idPedido = null, tipo = null) {
    const modelo = structuredClone(modelBase);
    modelo["idSucursal"] = parseInt($("#cboSucursales").val())

    // Colocar por defecto la fecha de mañana
    const fechaEntrega = new Date();
    const DiaDespues = new Date(fechaEntrega);
    DiaDespues.setDate(fechaEntrega.getDate() + 1);
    const formatoFecha = DiaDespues.toISOString().split("T")[0];
    $('#txtFechaEntrega').val(formatoFecha);

    // Bloquear fechas anteriores a mañana
    $('#txtFechaEntrega').attr("min", formatoFecha);

    if (tipo == null || tipo == "Agregar") {
        tablaDataPedido = $('#tbPedido').DataTable({
            responsive: true,
            "ajax": {
                "url": "/Pedidos/Cargar" + elemento + `?idSucursal=${sucLogin}`,
                "type": "GET",
                "datatype": "json"
            },
            "bDestroy": true,
            "datasrc": "",
            "columns": [
                {
                    "data": null, render: function (data) {
                        return '<div class="codigo">' +
                            data.codigo
                        '</div>'

                    },
                },
                { "data": "nombre" },
                { "data": "categoria" },
                {
                    "data": null, render: function (data) {
                        return '<div class="form-outline">' +
                            '<input type="number" name="Cantidad_' + data.codigo + '" id="CantidadInput_' + data.codigo + '" class="form-control cantidad" min=1 disabled />' +
                            '</div>'

                    },
                    "orderable": false,
                    "searchable": false

                },
                {
                    "data": null, render: function (data) {
                        return '<div class="form-check" style="text-align: center">' +
                            '<input class="form-check-input cambioCheck" type="checkbox" name="checking" id="CantidadCheck_' + data.codigo + '" onclick="seleccionar(' + data.codigo + ')" />' +
                            '</div>'

                    },
                    "orderable": false,
                    "searchable": false
                }
            ],
            order: [[2, "asc"], [1, "asc"]],
            columnDefs: [
                {
                    targets: [0, 1],
                    searchable: true
                },
                {
                    targets: [2, 3],
                    searchable: false
                }
            ]
        })
    } else if (tipo == "Editar") { // DEBEMOS DE TRAER LOS ELEMENTOS ACTIVOS + INACTIVOS(SI ESTUVO EN UN PEDIDO)
        tablaDataPedido = $('#tbPedido').DataTable({
            responsive: true,
            "ajax": {
                "url": "/Pedidos/Cargar" + elemento + `ActivoEInactivoPedido?idSucursal=${sucLogin}&idPedido=${idPedido}`,
                "type": "GET",
                "datatype": "json"
            },
            "bDestroy": true,
            "datasrc": "",
            "columns": [
                {
                    "data": null, render: function (data) {
                        return '<div class="codigo">' +
                            data.codigo
                        '</div>'

                    },
                },
                { "data": "nombre" },
                { "data": "categoria" },
                {
                    "data": null, render: function (data) {
                        return '<div class="form-outline">' +
                            '<input type="number" name="Cantidad_' + data.codigo + '" id="CantidadInput_' + data.codigo + '" class="form-control cantidad" min=1 disabled />' +
                            '</div>'

                    },
                    "orderable": false,
                    "searchable": false

                },
                {
                    "data": null, render: function (data) {
                        return '<div class="form-check" style="text-align: center">' +
                            '<input class="form-check-input cambioCheck" type="checkbox" name="checking" id="CantidadCheck_' + data.codigo + '" onclick="seleccionar(' + data.codigo + ')" />' +
                            '</div>'

                    },
                    "orderable": false,
                    "searchable": false
                }
            ],
            order: [[2, "asc"], [1, "asc"]],
            columnDefs: [
                {
                    targets: [0, 1],
                    searchable: true
                },
                {
                    targets: [2, 3],
                    searchable: false
                }
            ],
            initComplete: function () {
                if (idPedido != null) {

                    fetch(`/Pedidos/ObtenerPedido?idPedido=${idPedido}`, {
                        method: "GET"
                    })
                        .then(response => {
                            return response.ok ? response.json() : Promise.reject(response);
                        })
                        .then(responseJson => {
                            if (responseJson.length > 0) {
                                var table = $("#tbPedido").DataTable();
                                var data = table.data();
                                for (var p = 0; p < data.length; p++) {
                                    var row = table.row(p);
                                    for (let i = 0; i < responseJson.length; i++) {
                                        const elemento = responseJson[i];
                                        const codigoTable = $(row.node()).find('[class="codigo"]').text()

                                        if (elemento.codigo == codigoTable) {
                                            $(row.node()).find('.cambioCheck').prop('checked', true);
                                            $(row.node()).css('background-color', '#e0f7fa'); // 
                                            $(row.node()).find('.form-control.cantidad').prop('disabled', false);
                                            $(row.node()).find('.form-control.cantidad').val(elemento.cantidad);
                                        }
                                    }
                                }

                                // MENSAJE SI ENCONTRO PRODUCTOS CON ACTIVO = 0
                                if (responseJson[0].mensaje != null) {
                                    swal("ALERTA!", responseJson[0].mensaje , "info")
                                }

                                $("#txtComentario").val(responseJson[0].comentario)

                                editarPedido = "editarPedido"
                                idPedidoEditar = idPedido

                                if (responseJson[0].fechaEntrega == null) {
                                    // Colocar por defecto la fecha de mañana
                                    $('#txtFechaEntrega').val(formatoFecha);
                                } else {
                                    var entregaPartido = responseJson[0].fechaEntrega.split("-")

                                    //var fechaConvertido = new Date(entregaPartido[2] + "/" + entregaPartido[1] + "/" + entregaPartido[0]);
                                    $("#txtFechaEntrega").val(fechaConentregaPartido[2] + "-" + entregaPartido[1] + "-" + entregaPartido[0])
                                }


                            }
                        })
                }
            }
        })
    }
    else { // VER DETALLE PEDIDO
        tablaDataVer = $('#tbPedidoVer').DataTable({
            responsive: true,
            "ajax": {
                "url": `/Pedidos/VerDetallesPedido?idPedido=${idPedido}&tipoPedido="${elemento}"`,
                "type": "POST",
                "datatype": "json"
            },
            "bDestroy": true,
            "datasrc": "",
            "columns": [
                { "data": "codigoDetalle" },
                { "data": "descripcionDetalle" },
                { "data": "categoriaDetalle" },
                { "data": "cantidadDetalle" }
            ],
            order: [[0, "desc"]],
            columnDefs: [
                {
                    targets: [0, 1],
                    searchable: true
                },
                {
                    targets: [2, 3],
                    searchable: false
                }
            ],
            initComplete: function () {
                if (idPedido != null) {

                    fetch(`/Pedidos/ObtenerPedido?idPedido=${idPedido}`, {
                        method: "GET"
                    })
                    .then(response => {
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.length > 0) {
                            $("#txtVerComentario").val(responseJson[0].comentario)
                            if (responseJson[0].fechaEntrega == null) {
                                $('#txtVerFechaEntrega').val("No hay una fecha de entrega");
                            } else {
                                $("#txtVerFechaEntrega").val(responseJson[0].fechaEntrega)
                            }
                        }
                    })
                }
            }
        });
    }
};

$(document).on("change", ".cambioCheck", function () {
    const codigo = this.id.replace("CantidadCheck_", "");
    seleccionar(codigo);
});

function seleccionar(codigo) {
    const checkbox = document.getElementById("CantidadCheck_" + codigo);
    const inputCantidad = document.getElementById("CantidadInput_" + codigo);
    const row = checkbox.closest("tr");

    if (checkbox.checked) {
        row.style.backgroundColor = "#e0f7fa";
        inputCantidad.value = 1;
        inputCantidad.disabled = false;
    } else {
        row.style.backgroundColor = "";
        inputCantidad.value = "";
        inputCantidad.disabled = true;
    }
}

//--//

$("#btnGuardar").click(function () {
    guardarElementos();
});

$("#btnPendiente").click(function () {
    guardarElementos("Pendiente");
});


const guardarDetalle = {
    codigoDetalle: 0,
    cantidadDetalle: 0,
    createdDetalle: "",
    tipoCabecera: "",
    cantidadCabecera: 0,
    createdCabecera: "",
    estadoCabecera: "",
    idPedido: 0,
    Comentario: "",
    entregaCabecera: ""
};

function guardarElementos(element = null) {
    // ELIGIO AL MENOS 1 ELEMENTO?
    var seleccion = false;
    var table = $("#tbPedido").DataTable(); 
    var data = table.data(); 

    for (var i = 0; i < data.length; i++) {
        var row = table.row(i);
        var checkbox = $(row.node()).find('input[type="checkbox"][name="checking"]');

        if (checkbox.prop("checked")) { 
            seleccion = true;
            break;
        }
    }

    if (!seleccion) {
        swal("Lo sentimos!", "Seleccione por lo menos un elemento", "error")
        return;
    }

    // GUARDAR DETALLES
    var datosListaDetalle = [];
    var sumaCantidades = 0;
    const fecha = new Date();

    for (var p = 0; p < data.length; p++) {
        var row = table.row(p);
        var checkbox = $(row.node()).find('input[type="checkbox"][name="checking"]');

        if (checkbox.prop("checked")) {
            var pedidoDetalle = Object.assign({}, guardarDetalle);

            pedidoDetalle.codigoDetalle = parseInt($(row.node()).find('[class="codigo"]').text());
            pedidoDetalle.cantidadDetalle = parseInt($(row.node()).find(`[name="Cantidad_${pedidoDetalle.codigoDetalle}"]`).val());
            pedidoDetalle.createdDetalle = fecha;

            datosListaDetalle.push(pedidoDetalle);
            sumaCantidades += pedidoDetalle.cantidadDetalle;
        }
    }

    datosListaDetalle[0].tipoCabecera = botonElegido
    datosListaDetalle[0].cantidadCabecera = sumaCantidades
    datosListaDetalle[0].createdCabecera = fecha

    var fechaEntrega = $("#txtFechaEntrega").val();
    const fechaHoy = fecha.toISOString().split("T")[0];
    if (fechaEntrega < fechaHoy) {
        const partir = fechaHoy.split("-")
        fechaHoy = partir[2] + "-" + partir[1] + "-" + partir[0]
        swal("Lo sentimos!", "Debe de ingresar / modificar la fecha de entrega. Debe ser mayor a " + fechaHoy, "error")
        return;
    }

    datosListaDetalle[0].entregaCabecera = fechaEntrega

    datosListaDetalle[0].Comentario = $("#txtComentario").val();
    if (element != null) {
        datosListaDetalle[0].estadoCabecera = "Pendiente";
    } else {
        datosListaDetalle[0].estadoCabecera = "Enviado";
    }

    $("#modalData").find("div.modal-content").LoadingOverlay("show")
    var a = JSON.stringify(datosListaDetalle)

    if (editarPedido == "") {
        fetch("/Pedidos/GuardarPedido", {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify(datosListaDetalle)
        })
        .then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                $("#modalData").modal("hide")
                //swal("Listo!", responseJson.mensaje, "success")

                swal({
                    title: "Listo!",
                    text: responseJson.mensaje,
                    type: "info",
                    showCancelButton: false,
                    confirmButtonClass: "btn-success",
                    confirmButtonText: "OK",
                    closeOnConfirm: false,
                    closeOnCancel: true
                },
                    function (respuesta) {
                        location.reload();
                    }
                )

            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        });
    } else {
        datosListaDetalle[0].idPedido = parseInt(idPedidoEditar)

        fetch("/Pedidos/EditarPedido", {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify(datosListaDetalle)
        })
        .then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                $("#modalData").modal("hide")
                swal("Listo!", responseJson.mensaje, "success")
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        });
    }

}

