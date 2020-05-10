$(document).ready(function () {
    $(".register").click(function () {
        pintarRegistro();
    });
    $(".forget").click(function () {
        pintaOlvide();
    });
    $(document).on("click", "#enviarCorreo", function () {
        if ($("#correo").val() != "") {
            $.get("Usuario/EnviarPass?correo=" + $("#correo").val(), function (res) {
                swal("Buen Trabajo!", res, "success");
            });
        } else {
            swal("Ha ocurrido un error!", "Primero llene el campo!", "error");
        }
    });
    $(document).on("click","#registrarse", async function () {
        let datos = await validarCampos();
        if(datos.ok){
            $.post("Usuario/Registro?name_perfil="+datos.datos_url.nombre_perfil+"&name_fondo="+datos.datos_url.nombre_fondo,datos.datos_post, function (res) {
                if(res.ok){
                    swal("Buen trabajo!",res.message,"success");
                }else{
                    swal("Ha ocurrido un error!",res.message,"error");
                }
            })
            .done(function(){
                $("#myModal").hide();
            });
        }else{
            swal("Ha ocurrido un error!",datos.message,"error");
        }
    });
});

function pintarRegistro() {
    $(".header").html("Registrate Ahora!");
    let template = `
            <form role="form">
                 <div class ="form-group">
                    <label for="nombre">Nombres*: </label>
                    <input type="text" class ="form-control" id="nombre">
                  </div>
                  <div class ="form-group">
                    <label for="apellido">Apellidos*: </label>
                    <input type="text" class ="form-control" id="apellido">
                  </div>
                  <div class ="form-group">
                    <label for="sexo">Sexo*: </label>
                    <select class ="form-control" id="sexo">
                        <option value="1">Hombre</option>
                        <option value="0">Mujer</option>
                    </select>
                  </div>
                  <div class ="form-group">
                    <label for="fecha">Fecha de necimiento*: </label>
                    <input type="date" class ="form-control" id="fecha">
                  </div>
                  <div class ="form-group">
                    <label for="correo">Correo*: </label>
                    <input type="email" class ="form-control" id="correo">
                  </div>
                  <div class ="form-group">
                    <label for="contraseña">Contraseña*: </label>
                    <input type="text" class ="form-control" id="contraseña">
                  </div>
                  <div class ="form-group">
                    <label for="c_contraseña">Confirmar Contraseña*: </label>
                    <input type="text" class ="form-control" id="c_contraseña">
                  </div>
                  <div class ="form-group">
                    <label for="tel">Telefono: </label>
                    <input type="text" class ="form-control" id="tel">
                  </div>
                  <div class ="form-group">
                    <label class ="control-label" for="file">Imagen de Perfil: </label>
                    <input type="file" class ="form-control" id="file" placeholder="Busca el archivo" name="file">
                </div>
                <div class ="form-group">
                    <label class ="control-label" for="file2">Imagen de Fondo: </label>
                    <input type="file" class ="form-control" id="file2" placeholder="Busca el archivo" name="file">
                </div>
                <div class ="form-group">
                    <label class ="control-label" for="area">Descripcion: </label>
                    <textarea class="form-control" rows="4"cols="80" id="area">Escribe una breve descripcion!</textarea>
                </div>
            </form>
        `;
    $(".pinta").html(template);
    $(".pintaBtn").html('<a class="btn btn-default" id="registrarse">Enviar</a>');
}
function pintaOlvide() {
    $(".header").html("¿Se le olvido su contraseña?");
    let template = `
            <form role="form">
                <div class ="form-group">
                    <label for="correo">Escriba su correo: </label>
                    <input type="email" class ="form-control" id="correo">
                    <p>Se le enviara un correo, con su contraseña.</p>
                  </div>
            </form>
        `
    $(".pinta").html(template);
    $(".pintaBtn").html('<a class="btn btn-default" id="enviarCorreo">Enviar</a>');
}
async function validarCampos() {
    let nombre = $("#nombre").val();
    let apellidos = $("#apellido").val();
    let sexo = $("#sexo").val();
    let fecha = $("#fecha").val();
    let correo = $("#correo").val();
    let contra = $("#contraseña").val();
    let c_contra = $("#c_contraseña").val();
    let tel = $("#tel").val();
    let file = document.querySelector('#file').files[0];
    let doc_b64 = null;
    let doc_b642 = null;
    let nombre_perfil = "";
    let nombre_fondo = "";
    let descripcion = $("#area").val();
    if(file != null){
        doc_b64 = await toBase64(file);
        nombre_perfil = file.name;
    }
    let file2 = document.querySelector('#file2').files[0];
    if(file2 != null){
        doc_b642 = await toBase64(file2);
        nombre_fondo = file2.name;
    }
    if(nombre == "" || apellidos == "" || sexo == "" || fecha == "" || correo == "" || contra == "" || c_contra == ""){
        return {"ok" : false, "message" : "Primero llena los campos obligatorios!"};
    }else{
        if(c_contra != contra){
            return {"ok" : false, "message" : "Las contraseñas no coinciden!"};
        }else{
            if(tel.length != 10){
                return {"ok" : false, "message" : "El numero de telefono ha excedido el número máximo de caracteres!"};
            }else{
                return {
                    "ok" : true,
                    "datos_post" : 
                        {
                            "nombres": nombre,
                            "apellidos": apellidos,
                            "sexo": sexo,
                            "fecha_nacimiento": fecha,
                            "email": correo,
                            "contraseña": contra,
                            "telefono": tel,
                            "path_perfil": doc_b64,
                            "path_fondo" : doc_b642,
                            "descripcion" : descripcion
                        },
                    "datos_url" : 
                        {
                            "nombre_perfil" : nombre_perfil,
                            "nombre_fondo" : nombre_fondo
                        }
                };
            }
        }
    }
}
const toBase64 = file => new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onloadend = function (e) {
        var b64 = e.target.result.split("base64,")[1];
        resolve(b64);
    };
    reader.onerror = function (error) { reject(error) };
});