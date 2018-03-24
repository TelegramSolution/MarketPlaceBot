function ProductSave() {
    var Product = {
        'Id': $("#Id").val(),
        'Name': $("#Name").val(),
        'CategoryId': $("#CategoryId").val(),
        'UnitId': $("#UnitId").val(),
        'Text': $("#Text").val(),
        'TelegraphUrl': $("#TelegraphUrl").val(),
        'CurrentPriceId': $("#CurrentPriceId").val(),
        'MainPhoto': $("#MainPhoto").val(),
        'Enable': $('#Enable').prop('checked'),
        'CurrentPrice.ProductId': $("#CurrentPrice.ProductId").val(),
        'CurrentPrice.CurrencyId': $("#CurrentPrice.CurrencyId").val(),
        'CurrentPrice.Value': $("#CurrentPrice.Value").val(),

    }
    $.ajax({
        type: "POST",
        url: 'Product/Save',
        data: JSON.stringify(Config),
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        dataType: "json"
    }).done(function (data) {
        alert(data);

    })
}