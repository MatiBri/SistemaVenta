
//Creacion de modelos
const MODELO_BASE = {
    //Propiedades traídas de los ViewModels. Tienen que estar en el mismo orden que el VMUsuario y así con los demás
    //Nota: Los nombres acá estan en minúscula, mientras que en VMUSuario estan en maýúsculas, Javascript no es case sensitive así que no hay problema con ello
    idUsuario: 0,
    nombre: "",
    correo: "",
    telefono: "",
    idRol: 0,
    esActivo: 1,
    urlFoto: ""
}

//Crear evento para definir cuando el documento ya está cargado
//El signo dolar "$" es un JQuery

let tablaData;
$(document).ready(function () {

    //Peticion fetch para solicitar una URL con toda la respuesta. Fetch trabaja con promesas, así que hay que poner un "then"
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

    //Obtengo toda la lista de usuarios para llevarlos a la tabla
    tablaData = $('#tbdata').DataTable({
        responsive: true,
         "ajax": {
             "url": '/Usuario/Lista',
             "type": "GET",
             "datatype": "json"
         },
        "columns": [
            { "data": "idUsuario", "visible": false, "searchable": false },
            {
                "data": "urlFoto", render: function (data) {
                    return `<img style="height: 60px" src=${data} class="rounded mx-auto d-block"/>`
                }
            },
             { "data": "nombre" },
             { "data": "correo" },
             { "data": "telefono" },
            { "data": "nombreRol" },
            {
                "data": "esActivo", render: function (data) {
                    if (data == 1) {
                        return '<span class="badge badge-info">Activo</span>';
                    } else {
                        return '<span class="badge badge-danger">No Activo</span>';
                    }
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
         order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Usuarios',
                exportOptions: {
                    //Estas son las columnas que exportaremos, son las que vienen de "columns" mas arriba
                    columns: [2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            //El lenguaje en el cual se va a mostrar nuestro DataTable
            //Hago uso de una URL externa el cual convierte todo en español
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})


//Esta funcion recibe como parámetro un modelo, y pintar en todas las cajas de texto, sus valores correspondientes
function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idUsuario) //El modelo que va a tomar va a ser el idUsuario para el Index.cshtml para la vista de usuario en el Modal
    $("#txtNombre").val(modelo.nombre)
    $("#txtCorreo").val(modelo.correo)
    $("#txtTelefono").val(modelo.telefono)
    $("#cboRol").val(modelo.idRol == 0 ? $("#cboRol option:first").val() : modelo.idRol)
    $("#cboEstado").val(modelo.esActivo)
    $("#txtFoto").val("")
    $("#imgUsuario").attr("src", modelo.urlFoto)

    //Mostrar el modal
    $("#modalData").modal("show")

    //Esta función se ejecuta cada vez que el usuario presione el botón de "Nuevo Usuario"
}

//BtnNuevo
$("#btnNuevo").click(function () {
    mostrarModal()
})

$("#btnGuardar").click(function () {

    //debugger; //Este debugger es para saber qué es lo que está sucediendo acá

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    //1.-Si inputs_sin_valor es mayor a 0 (osea que tiene elementos)
    if (inputs_sin_valor.length > 0) {
        //2.-Armamos un mensaje
        const mensaje = `Debe completar el campo: "${inputs_sin_valor[0].name}"`;
        //3.-Mensaje de alerta
        toastr.warning("", mensaje)
        $(`input[name ="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idUsuario"] = parseInt($("#txtId").val())
    modelo["nombre"] = $("#txtNombre").val()
    modelo["correo"] = $("#txtCorreo").val()
    modelo["telefono"] = $("#txtTelefono").val()
    modelo["idRol"] = $("#cboRol").val()
    modelo["esActivo"] = $("#cboEstado").val()

    //Esta caja de texto recibe archivos. Entonces la forma mas sencilla es con Javascript para recibir el elemento de una Foto
    const inputFoto = document.getElementById("txtFoto")

    //Esto es lo que enviamos al método de UsuarioController, al método Crear. En Crear, recibe desde un formulario, una foto y un modelo
    const formData = new FormData();

    formData.append("foto", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))
    formData.append("foto", inputFoto.files[0])

    //Mostramos un preview de cargando mientras se envía toda la info
    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    //Si es = a 0, es para crear un usuario
    if (modelo.idUsuario == 0) {
        //Enviamos el método Crear de UsuarioController
        //1.- A esta petición (el fetch)
        fetch("/Usuario/Crear", {
            method: "POST", //2.- Que es un POST
            body: formData //Le enviamos toda la data de formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                //Si estado (que viene de Crear() a través del método gResponse) es true
                if (responseJson.estado) {

                    //responseJson nos devuelve un objeto, y el objeto es el usuario que hemos creado
                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    swal("Listo!", "El usuario fue creado", "success")
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    } else {
        //Si no es igual a 0, lo va a editar
        fetch("/Usuario/Editar", {
            method: "PUT", //2.- Que es un POST
            body: formData //Le enviamos toda la data de formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                //Si estado (que viene de Crear() a través del método gResponse) es true
                if (responseJson.estado) {
                    //responseJson nos devuelve un objeto, y el objeto es el usuario que hemos creado
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null; //Lo pongo como nulo porque ya lo hemos utilizado arriba
                    $("#modalData").modal("hide");
                    swal("Listo!", "El usuario fue modificado", "success")
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    }
})

let filaSeleccionada;
//BOTON EDITAR
$("#tbdata tbody").on("click", ".btn-editar", function () {

    //El tr es el que contiene toda la información en el index.cshtml de Usuario (pero selecciona el tr anterior, el que tiene class "odd parent")
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    //Hago el const porque necesito almacenar toda la data que contiene esta fila
    const data = tablaData.row(filaSeleccionada).data();

    mostrarModal(data);
})

//BOTON ELIMINAR
$("#tbdata tbody").on("click", ".btn-eliminar", function () {

    //El tr es el que contiene toda la información en el index.cshtml de Usuario (pero selecciona el tr anterior, el que tiene class "odd parent")

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    //Hago el const porque necesito almacenar toda la data que contiene esta fila
    const data = tablaData.row(fila).data();

    swal({
        title: "¿Estás seguro de eliminar?",
        text: `Eliminar al usuario "${data.nombre}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $("#showSweetAlert").LoadingOverlay("show");

                //El parámetro IdUsuario es el que hacemos llamado en el método de Eliminar (en su parámetro) en Usuario Controller
                fetch(`/Usuario/Eliminar?IdUsuario=${data.idUsuario}`, {
                    method: "DELETE", //2.- Que es un POST
                })
                    .then(response => {
                        $("#showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        //Si estado (que viene de Crear() a través del método gResponse) es true
                        if (responseJson.estado) {
                            //responseJson nos devuelve un objeto, y el objeto es el usuario que hemos creado
                            tablaData.row(fila).remove().draw()
                            
                            swal("Listo!", "El usuario fue eliminado", "success")
                        } else {
                            swal("Lo sentimos", responseJson.mensaje, "error")
                        }
                    })

            }
        }
    )
})