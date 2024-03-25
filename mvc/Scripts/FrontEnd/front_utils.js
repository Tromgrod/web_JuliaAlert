$(document).ready(function () {
    $('.ajax-loading-overlay')
        .ajaxStart(() => {
            if (!$(".loading").is(":visible"))
                $(this).fadeIn("slow");
        })
        .ajaxStop(() => $(this).fadeOut("slow"));
});
/*-------------------------------------------------------:message-----------------------------------------------*/

function ShowConfirmMessage(message, ok_fn, cancel_fn, caption_ok, caption_cancel, caption_other, other_fn, init_fn, pwidth) {
    if (!caption_ok)
        caption_ok = 'Да';
    if (!caption_cancel)
        caption_cancel = 'Нет';

    $(".confirmation-message").html(message);
    set_calendar();
    if (pwidth != undefined) {
        $(".confirmation-container").css("width", pwidth + "px");
    }
    else {
        $(".confirmation-container").css("width", null);
    }
    if (init_fn != undefined) {
        init_fn();
    }
    $(".confirmation-overlay").fadeIn("slow");
    $("#message_ok").unbind("click");
    $("#message_ok").html(caption_ok);
    $("#message_ok").on("click", function () {
        if (ok_fn != undefined) {
            if (ok_fn()) {
                $(".confirmation-overlay").fadeOut("slow");
            }
        }
        else {
            $(".confirmation-overlay").fadeOut("slow");
        }
    });
    $("#message_cancel").show();
    $("#message_cancel").html(caption_cancel);
    $("#message_cancel").unbind("click");
    $("#message_cancel").on("click", function () {
        if (cancel_fn != undefined) {
            if (cancel_fn()) {
                $(".confirmation-overlay").fadeOut("slow");
            }
        }
        else {
            $(".confirmation-overlay").fadeOut("slow");
        }
    });

    $("#message_other").unbind("click");

    $("#message_other").hide();
    if (caption_other != undefined) {
        $("#message_other").show();
        $("#message_other").html(caption_other);
        $("#message_other").on("click", function () {
            if (other_fn != undefined) {
                if (other_fn()) {
                    $(".confirmation-overlay").fadeOut("slow");
                }
            }
            else {
                $(".confirmation-overlay").fadeOut("slow");
            }
        });

    }

    $(".confirmation-container").css("marginTop", $("body").height() / 2 - $(".confirmation-container").height() / 2);
}

function ShowAlertMessage(message, ok_fn, cancel_fn) {
    $(".confirmation-message").html(message);
    $(".confirmation-overlay").fadeIn("slow");

    $("#message_ok").html("OK");
    $("#message_ok").unbind("click");
    $("#message_ok").on("click", function () {
        if (ok_fn != undefined) {
            if (ok_fn()) {
                $(".confirmation-overlay").fadeOut("slow");
            }
        }
        else {
            $(".confirmation-overlay").fadeOut("slow");
        }
    });
    $("#message_cancel").hide();
}

/*--------------------------------------------------------------------------------------------------------------*/

/*-------------------------------------------------------:html--------------------------------------------------*/
function init_htmlpopup_link() {

    gCkEditorConfig = {
        toolbar:
            [
                ["Bold", "Italic", "Underline", '-', "P", "BulletedList", "Indent", "Outdent", 'FontSize', "Font"],
                ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'],
                ["JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock"],
                ["Maximize", "ShowBlocks"]
            ],
        removePlugins: 'elementspath',
        resize_enabled: false,
        allowedContent: true,
        ignoreEmptyParagraph: true,
        width: $(".header").width() < 1100 ? 907 : 954,
        height: 345
    };

    $(".htmlpopup-link").fancybox({
        width: 1000,
        height: 550,
        autoSize: false,
        ajax: { cache: false },
        padding: 7,
        wrapCSS: "htmlpopup" + ($(".header").width() < 1100 ? " htmlpopup-limited" : ""),
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
        beforeShow: function () {
            gHtmlPopupControl = $(this.element).prev();
        },
        afterShow: function () {
            if (gHtmlPopupControl.next().next().val() != "") {
                $("#popUpHtml").val(gHtmlPopupControl.next().next().val());
                $(".popup-content-tabs").hide();
            }
            $("#popUpHtml").ckeditor(gCkEditorConfig);
        }
    });
}

function init_htmlpopup_tab_link(pBlock) {
    gCkEditorConfigNoTabs = {
        toolbar:
            [
                ["Bold", "Italic", "Underline", '-', "P", "BulletedList", "NumberedList", "Indent", "Outdent", 'FontSize'],
                ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'],
                ["JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock"],
                ["Maximize", "ShowBlocks"]
            ],
        removePlugins: 'elementspath',
        resize_enabled: false,
        allowedContent: true,
        ignoreEmptyParagraph: true,
        width: $(".header").width() < 1100 ? 907 : 954,
        height: 345
    };
    $(pBlock).find(".control-edit").fancybox({
        width: 1000,
        height: 550,
        autoSize: false,
        padding: 7,
        wrapCSS: "htmlpopup htmlpopup-tab" + ($(".header").width() < 1100 ? " htmlpopup-limited" : ""),
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
        beforeShow: function () {
            gHtmlPopupControl = null;
        },
        afterShow: function () {
            $("#popUpHtml").ckeditor(gCkEditorConfigNoTabs);
            set_calendar();
        }
    });
    $(pBlock).find(".control-copy").fancybox({
        width: 1000,
        height: 550,
        autoSize: false,
        padding: 7,
        wrapCSS: "htmlpopup htmlpopup-tab" + ($(".header").width() < 1100 ? " htmlpopup-limited" : ""),
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
        beforeShow: function () {
            gHtmlPopupControl = null;
        },
        afterShow: function () {
            $("#popUpHtml").ckeditor(gCkEditorConfigNoTabs);
            set_calendar();
        }
    });
}

/*-------------------------------------------Cookie----------------------------------------------------------*/
let gWarnedCookie = false;

function writeCookie(name, val, expires = 0) {
    if (navigator.cookieEnabled === false && gWarnedCookie === false) {
        alert("Cookies отключены!");
        gWarnedCookie = true;
    }
    else if (expires !== 0) {
        var date = new Date;
        date.setDate(date.getDate() + expires);
        document.cookie = name + "=" + val + "; path=/; expires=" + date.toUTCString();
    }
    else {
        document.cookie = name + "=" + val + "; path=/";
    }
}

function readCookie(name) {
    var matches = document.cookie.match(new RegExp(
        "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}