$(document).ready(function () {
    $.widget.bridge('uibutton', $.ui.button);

    reloadControls();

    $(".btn-add").fancybox({
        autoSize: true,
        padding: 7,
        helpers: {
            overlay: {
                speedIn: 0,
                speedOut: 300,
                opacity: 0.8,
                css: {
                    cursor: 'default'
                },
                closeClick: false
            }
        },
        afterShow: function () {
            reloadControls();
        }
    });
});

function reloadControls() {

    $(".calendar-input").datepicker({
        format: "dd/mm/yyyy",
        changeYear: true,
        clearBtn: true,
        yearRange: "1900:" + (new Date()).getFullYear()
    }).on('changeDate', function (e) {
        $(this).datepicker('hide');
    });

    if ($('input[type="checkbox"].flat-red, input[type="radio"].flat-red').iCheck != undefined) {
        $('input[type="checkbox"].flat-red, input[type="radio"].flat-red').iCheck({
            checkboxClass: 'icheckbox_flat-green',
            radioClass: 'iradio_flat-green'
        });
    }
    if ($(".control-input-html").size() > 0) {
        $(".control-input-html").find("textarea").ckeditor(gCkEditorConfig);
    }
    $(".th-tooltip").tooltip();
    
    if ($(".autocomplete-input").size() > 0) {
        $.each($(".autocomplete-input"), function (i, item) {
            load_autocomplete(item);
        });
    }

    if ($(".control-multyselect").size() > 0) {
        $.each($(".control-multyselect select"), function (i, item) {
            load_multyselect(item);
        });
    }
}

function doLogin() {
    $.post($('form').attr("action"), $('form').serialize(), function(data) {
        
        if (data["Result"] == 1) {
            window.location = data["RedirectURL"];
        } else {
            alert(data["Message"]);
        }
        
    } );
}

//---------------------------------:User---------------------------------
function ValidateUserName(pInut) {
    var Value = $(pInut).val();
    var result = false;
    if ($.trim(Value) != "") {
        var ID = ($(".data-item-container").size() > 0 ? $(".data-item-container").attr("data-id") : $($(pInut).closest("tr").find("td")[1]).html());
        $.ajax({
            type: "POST",
            url: gRootUrl + "ValidationHelper/ValidateUserName/",
            data: { Login: Value, Userid: ID },
            async: false,
            dataType: "json",
            success: function (data) {
                if (data["Result"] == 1) {
                    result = true;
                    gErrorMessage = data["Message"];
                }
            }
        });
    }
    else {
        return true;
    }
    return result;

}

function ValidatePassword(pInut) {
    var Value = $(pInut).val();
    var ID = ($(".data-item-container").size() > 0 ? $(".data-item-container").attr("data-id") : $($(pInut).closest("tr").find("td")[1]).html());
    if ($.trim(Value) == "" && ID=="0") {
        return true;
    }
    return false;

}
//-----------------------------------------------------------------------

//---------------------------------:Controls-----------------------------
function UpdateTag(pcontrol) {
    if ($(pcontrol).closest("tr").parent().find("[name=Tag]").val() == "") {
        $(pcontrol).closest("tr").parent().find("[name=Tag]").val(generateTag($(pcontrol).val()));
    }
}
//-----------------------------------------------------------------------