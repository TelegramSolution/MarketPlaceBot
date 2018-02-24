function ajaxAddHistory(ActionId, Text) {

    var Config = {
        'OrderId': $("#Id").val(),
        'Text': Text,
        'ActionId': ActionId
    }

    $.ajax({
        type: "POST",
        url: '/Order/AddHistory',
        data: JSON.stringify(Config),
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        dataType: "json"
    }).done(function (data) {
        console.info(data);
        if (data === "Добавлено") { // Пользователь добавляет запись о том что заказ согласован
            $('#DoneBtn').removeClass('disabled');
            $('#TmpConfirm').val('true'); //
            $("#Example").val(data);
            //getСonfirm(); // запрос табличных данных
        }
        else {
            console.log(data);
            alert(data);
        }
        //$("#Example").val(data);
    }).error(function (data) {
        // если с ответом сервера чтото пошло не так...
    })
}