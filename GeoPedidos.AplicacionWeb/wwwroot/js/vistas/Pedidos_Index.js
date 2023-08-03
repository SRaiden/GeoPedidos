const modelBase = {
    idUsuario: 0,
    idSucursal: 0,
    tipo: "",
    FechaDesde: "",
    FechaHasta: ""
}

let tablaData;

$(document).ready(function () {
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

    fetch("/Usuarios/ObtenerTodasEmpresas")
    .then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => { // creamos un response a la bbdd
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

    fetch(`/Usuarios/ObtenerSucursalEmpresa?idEmpresa=1`, {  
        method: "GET"
    })
    .then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => { // creamos un response a la bbdd
        $('#cboSucursales').empty();
        $("#cboSucursales").append(
            $("<option>").val(0).text("- TODAS LAS SUCURSALES -") // OPTION DE ADMINISTRADOR
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

});

//MODIFICAR SUCURSAL SEGUN EMPRESA ELEGIDA
$("#cboEmpresas").on("change", function () {
    var optionEmpresa = $(this).find(":selected").val()
    cargarSucursales(optionEmpresa)
})


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
        })
}

$("#cboSucursales").on("change", function () {
    busqueda();
})

$('input[type=radio][name=rbReporte]').change(function () {
    busqueda();
});

$("#txtDesde").blur( function () {
    busqueda();
})

$("#txtHasta").blur(function () {
    busqueda();
})

function busqueda() {

    const modelo = structuredClone(modelBase);
    modelo["idUsuario"] = 0
    modelo["idSucursal"] = parseInt($("#cboSucursales").val())

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
            "url": `/Pedidos/Busqueda?IdUsuario=${modelo.IdUsuario}&idSucursal=${modelo.idSucursal}&tipo="${modelo.tipo}"&fechaDesde="${modelo.FechaDesde}"&fechaHasta="${modelo.FechaHasta}"`,
            "type": "GET",
            "datatype": "json"
        },
        "bDestroy": true,
        "datasrc": "",
        "columns": [
            { "data": "id" },
            { "data": "created" },
            { "data": "numeroPedido" },
            { "data": "tipo" },
            {
                "data": null,
                "render": function (data) {
                    return '<div class="pendiente">' +data.estado + '</div>';
                },
            },
            { "data": "cantidad" },
            { "data": "nombreSucursal" },
            { "data": "nombreUsuario" },
            {
                "defaultContent": '<div class="dropdown">' +
                        '<button type="button" class= "btn p-0 dropdown-toggle hide-arrow" data-bs-toggle="dropdown" >' +
                        '<i class="bx bx-dots-vertical-rounded"></i>' +
                        '</button >' +
                        '<div class="dropdown-menu">' +
                        '<a class="dropdown-item btn-ver"><i class="bx bx-show"></i> Ver Detalles</a>' +
                        '<a class="dropdown-item btn-editar"><i class="bx bx-pencil"></i> Editar</a>' +
                        '<a class="dropdown-item btn-eliminar"><i class="bx bx-trash me-1"></i> Dar de Baja</a>' +
                        '<a class="dropdown-item btn-reporte"><i class="bx bx-file"></i> Reporte</a>' +
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
                var pendiente = ($(row.node()).find('[class="pendiente"]').text());

                if (pendiente == "Pendiente") {
                    $(row.node()).find('.btn-editar').show();
                    $(row.node()).find('.btn-eliminar').show();
                } else {
                    $(row.node()).find('.btn-editar').hide();
                    $(row.node()).find('.btn-eliminar').hide();
                }

                if (pendiente == "Anulado") {
                    $(row.node()).find('.btn-eliminar').hide();
                    $(row.node()).find('.btn-editar').hide();
                    $(row.node()).find('.btn-reporte').hide();
                }
            }
        }
    })
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

let editarPedido = "";
let numeroPedidoEditar = ""
let idSucursalEditar = ""

let botonElegido = "";
$("#btnHelado").click(function () {
    mostrarModal();
    cargarElementos("Helado");
    botonElegido = "helado"
    //-------------------------//
    editarPedido = ""
    numeroPedidoEditar = ""
    idSucursalEditar = ""

});

$("#btnProducto").click(function () {
    mostrarModal();
    cargarElementos("Producto");
    botonElegido = "producto"
    //-------------------------//
    editarPedido = ""
    numeroPedidoEditar = ""
    idSucursalEditar = ""
});

$("#btnInsumo").click(function () {
    mostrarModal();
    cargarElementos("Insumo");
    botonElegido = "insumo"
    //-------------------------//
    editarPedido = ""
    numeroPedidoEditar = ""
    idSucursalEditar = ""
});

$("#btnPasteleria").click(function () {
    mostrarModal();
    cargarElementos("Pasteleria");
    botonElegido = "pasteleria"
    //-------------------------//
    editarPedido = ""
    numeroPedidoEditar = ""
    idSucursalEditar = ""
});

$("#CerrarModal").click(function () {
    $("#modalData").modal("hide");
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

    mostrarModal();
    cargarElementos(data.tipo, data.numeroPedido);

});

function cargarElementos(elemento, numeroPedido = null) {
    const modelo = structuredClone(modelBase);
    modelo["idSucursal"] = parseInt($("#cboSucursales").val())

    tablaData = $('#tbPedido').DataTable({
        responsive: true,
        "ajax": {
            "url": "/Pedidos/Cargar" + elemento + `?idSucursal=1`, // MODIFICAR SUCURSAL CUANDO COLOQUEMOS PERMISOS
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
                    return  '<div class="form-outline">' +
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
            if (numeroPedido != null) {

                fetch(`/Pedidos/ObtenerPedido?idSucursal=1&numeroPedido=${numeroPedido}`, {  // MODIFICAR SUCURSAL CUANDO COLOQUEMOS PERMISOS
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

                        editarPedido = "editarPedido"
                        numeroPedidoEditar = numeroPedido,
                        idSucursalEditar = "1" // MODIFICAR SUCURSAL CUANDO COLOQUEMOS PERMISOS
                    }
                })
            }
        }
    })
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
        row.style.backgroundColor = "#e0f7fa"
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


// GUARDAR EN UNA MATRIZ LOS ELEMENTOS QUE ELEGISTE Y LA OPCION QUE ELIGIO
const guardarDetalle = {
    codigoDetalle: 0,
    cantidadDetalle: 0,
    createdDetalle: "",
    tipoCabecera: "",
    cantidadCabecera: 0,
    createdCabecera: "",
    estadoCabecera: "",
    numeroPedidoEditar: 0,
    idSucursalEditar: 0
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

        if (checkbox.prop("checked")) { // esta checkeado la fila?
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
                    swal("Listo!", responseJson.mensaje, "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            });
    } else {
        datosListaDetalle[0].numeroPedidoEditar = parseInt(numeroPedidoEditar)
        datosListaDetalle[0].idSucursalEditar = parseInt(idSucursalEditar) 

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