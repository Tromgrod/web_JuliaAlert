function import_order(inportSetting) {
    $('.ajax-loading-blur').show();

    $.ajax({
        type: "POST",
        url: gRootUrl + "Import/" + inportSetting,
        data: {
            File: failPath
        },
        success: response => {
            $('.ajax-loading-blur').hide();

            if (response.Message != undefined && response.Message != '')
                alert(response.Message)

            if (response.Result != 0)
                $.fancybox.close();
        }
    });
}