$(document).ready(function () {
});


function change_document_mode(pInput) {
    if (!$("[name=DocumentMode_in]").is(":checked") && $(pInput).attr("name") == "DocumentMode_in") {
        $("[name=DocumentMode_in]").prop("checked", true);
        return;
    }

    if (!$("[name=DocumentMode_out]").is(":checked") && $(pInput).attr("name") == "DocumentMode_out") {
        $("[name=DocumentMode_out]").prop("checked", true);
        return;
    }

    if ($(pInput).attr("name") == "DocumentMode_out") {
        $("[name=DocumentMode_in]").prop("checked", false);
    }

    if ($(pInput).attr("name") == "DocumentMode_in") {
        $("[name=DocumentMode_out]").prop("checked", false);
    }
}

/*----------------------------------------------------:upload-----------------------------------------------*/
var failPath;

function initUploadDocumentFile(purl, pUniqueId, acceptType) {
    $("#UploadFile").ajaxUpload({
        url: purl,
        name: "file",
        dataType: "JSON",
        acceptType: acceptType,
        onSubmit: function () {
            if (!$(".loading").is(":visible")) {
                $(".loading").show();
                return true;
            }
            return false;
        },
        onComplete: responce => {
            jsonResponce = JSON.parse(responce);
            if (jsonResponce.Result == 0) {
                alert(jsonResponce.Message);
                $(".loading").hide();
                return true;
            }
            $("[name=" + pUniqueId + "]").val(jsonResponce.Data.Id);
            let UniqueFielInput = $("#" + pUniqueId + "_link");
            UniqueFielInput.attr("href", jsonResponce.Data.file);
            UniqueFielInput.html(jsonResponce.Data.name);
            UniqueFielInput.removeClass('display-none');

            if ($('.popup-btn').length > 0)
                $('.popup-btn').removeClass('display-none');

            failPath = jsonResponce.Data.path;

            $(".loading").hide();
            return true;
        }
    });
}