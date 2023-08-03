const modelBase = {
    idUsuario: 0,
    idSucursal: 0,
    tipo: "",
    FechaDesde: "",
    FechaHasta: ""
}

let tablaData;

$(document).ready(function () {

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
            { "data": "estado" },
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
                targets: [0, 2], // Índices de las columnas "codigo" y "descripcion"
                searchable: true // Habilitar la búsqueda en estas columnas
            },
            {
                targets: [1, 3, 4, 5, 6, 7, 8], // Índices de las otras columnas (excluyendo "codigo" y "descripcion")
                searchable: false // Deshabilitar la búsqueda en estas columnas
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

$("#btnHelado").click(function () {
    mostrarModal();
    cargarElementos("Helado");
});

$("#btnProducto").click(function () {
    mostrarModal();
    cargarElementos("Producto");
});

$("#btnInsumo").click(function () {
    mostrarModal();
    cargarElementos("Insumo");
});

$("#btnPasteleria").click(function () {
    mostrarModal();
    cargarElementos("Pasteleria");
});

$("#CerrarModal").click(function () {
    $("#modalData").modal("hide");
});

function cargarElementos(elemento) {
    const modelo = structuredClone(modelBase);
    modelo["idSucursal"] = parseInt($("#cboSucursales").val())

    tablaData = $('#tbPedido').DataTable({
        responsive: true,
        "ajax": {
            //"url": "/Pedidos/Cargar" + elemento + `?idSucursal=${modelo.idSucursal}`,
            "url": "/Pedidos/Cargar" + elemento + `?idSucursal=1`,
            "type": "GET",
            "datatype": "json"
        },
        "bDestroy": true,
        "datasrc": "",
        "columns": [
            { "data": "codigo"},
            { "data": "nombre" },
            { "data": "categoria" },
            {
                "data": null, render: function (data) {
                    return  '<div class="form-outline">' +
                                            '<input type="number" name="Cantidad_' + data.codigo + '" id="CantidadInput_' + data.codigo + '" class="form-control" disabled/>' +
                                       '</div>'

                },
                "orderable": false,
                "searchable": false
               
            },
            {
                "data": null, render: function (data) {
                    return '<div class="form-check" style="text-align: center">' +
                                '<input class="form-check-input cambioCheck" type="checkbox" name="Cantidad_' + data.codigo + '" id="CantidadCheck_' + data.codigo + '" onclick="seleccionar(' + data.codigo + ')" />' +
                            '</div>'

                },
                "orderable": false,
                "searchable": false
            }
        ],
        order: [[0, "desc"]],
        columnDefs: [
            {
                targets: [0, 1], // Índices de las columnas "codigo" y "descripcion"
                searchable: true // Habilitar la búsqueda en estas columnas
            },
            {
                targets: [2, 3], // Índices de las otras columnas (excluyendo "codigo" y "descripcion")
                searchable: false // Deshabilitar la búsqueda en estas columnas
            }
        ]
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


const guardarPedido = {
    codigo: 0,
    nombre: 0,
    categoria: "",
    cantidad: "",
    pendiente: ""
};

function guardarElementos(element = null) {
    // ELIGIO AL MENOS 1 ELEMENTO?
    var seleccion = false;
    $(".cambioCheck").each(function () {
        if ($(this).prop("checked")) {
            seleccion = true;
        }
    });

    if (!seleccion) {
        swal("Lo sentimos!", "Seleccione por lo menos un pedido", "error")
        return;
    }


    // GUARDAR EN UNA MATRIZ LOS ELEMENTOS QUE ELEGISTE Y LA OPCION QUE ELIGIO
    var datosLista = [];

    $("tbody tr").each(function () {
        var fila = $(this);
        var checkbox = fila.find(".cambioCheck"); // Encontrar el checkbox de la fila

        if (checkbox.prop("checked")) { // Verificar si el checkbox está activo
            var pedido = Object.assign({}, guardarPedido); // Crear una copia del objeto guardarPedido

            // Asignar los valores de las columnas a las propiedades del objeto pedido
            pedido.codigo = parseInt(fila.find(".codigo").text());
            pedido.nombre = parseInt(fila.find(".nombre").text());
            pedido.categoria = fila.find(".categoria").text();
            pedido.cantidad = fila.find(".cantidad").text();

            datosLista.push(pedido); // Agregar el objeto pedido a la lista general
        }
    });

    // ejecutamos un gif de cargando en el JS LoadingOverlay (para mas estetica y se vea bonito)
    $("#modalData").find("div.modal-content").LoadingOverlay("show")

    // OPCION GUARDAR
    fetch("/Pedidos/GuardarPedido", {
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
            swal("Listo!", "El Pedido fue creado", "success")
        } else {
            swal("Lo sentimos!", responseJson.mensaje, "error")
        }
    });
}