$(".btnLlama").click(function () {
    pintaSolicitudes();
});
$(document).on('click','.Accept',function () {
    let id_amigo = $($(this)[0]).attr("id_amigo");
    $.get("AcceptOrDenegModal?tipo=" + 0 + "&id_amigo=" + id_amigo, function (data) {
        alert(data);
    }).always(function () {
        pintaSolicitudes();
    });
});
$(document).on('click', '.Denegar', function () {
    let id_amigo = $($(this)[0]).attr("id_amigo");
    $.get("AcceptOrDenegModal?tipo=" + 1 + "&id_amigo=" + id_amigo, function (data) {
        alert(data);
    }).always(function () {
        pintaSolicitudes();
    });
});
function pintaSolicitudes() {
    $.get("pintaSolicitudes", function (data) {
        let template = "";
        for (var i = 0; i < data.length; i++) {
            template += '<div class="well"><div class="media"><a class="pull-left" href="#"><img class="img-circle" src="' + data[i].img_perfil + '"/>';
            template += '</a><div class="media-body"><h4 class="media-heading">' + data[i].nombre + '</h4>';
            template += '<ul class="list-inline list-unstyled"><li>';
            template += '<a href="#" id_amigo='+data[i].id_amigo+' class="Accept">';
            template += '<span><i class="fas fa-user-plus"></i>Aceptar </span></a></li>';
            template += '<li><a href="#" id_amigo=' + data[i].id_amigo + ' class="Denegar"><span><i class="fas fa-user-times"></i>Rechazar </span></a></li>';
            template += '</ul></div></div></div>';
        }
        $('.pinta').html(template);
    });
}