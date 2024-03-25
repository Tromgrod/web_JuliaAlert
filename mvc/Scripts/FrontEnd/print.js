function print_item_document(viewPrint, data = {}) {
    data['ViewPrint'] = viewPrint;

    if (!data['ModelId']) {
        data['ModelId'] = $('[name=Id]').val();
    }
    if (!data['Namespace']) {
        data['Namespace'] = $('[name=Namespace]').val();
    }

    $.ajax({
        type: "POST",
        url: gRootUrl + "Print/CustomPrint",
        data: data,
        success: print => {
            $('.ajax-loading-overlay').fadeOut();

            $("#printArea").html(print);

            var timer = setInterval(() => {
                if ($("#printArea").find(".print-end").size() > 0) {
                    $('.ajax-loading-overlay').fadeOut();
                    $("#printArea").printArea();
                    clearInterval(timer);
                }
            }, 200);
        }
    });
}

function invoice_print(viewPrint) {
    data = {
        TAX: $('[name=TAX]').val(),
        Delivery: $('[name=Delivery]').val()
    };

    print_item_document(viewPrint, data)
}

function preview_barcode_CODE128(barcodeStr, barcodeType = 0, count = 1) {
    data = {
        BarcodeStr: barcodeStr,
        BarcodeType: barcodeType,
        Count: count
    };

    open_simple_popup_customData(data, '~/Views/BarcodeHelper/PreviewBarcode_CODE128.cshtml');
}

function preview_barcode_EAN13(barcodeStr, movingDate, count = 1) {
    data = {
        BarcodeStr: barcodeStr,
        MovingDate: movingDate,
        Count: count
    };

    open_simple_popup_customData(data, '~/Views/BarcodeHelper/PreviewBarcode_EAN13.cshtml');
}

function print_barcode(BarcodeStr) {
    $.post(gRootUrl + "Print/PrintBarcode", { BarcodeStr: BarcodeStr }, () => $.fancybox.close());
}