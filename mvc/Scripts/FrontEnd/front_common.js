/*----------------------------------------------------:navigation--------------------------------------------*/
function show_global_menu(pMenuGroup) {
    $(".sub-menu-section").not($(pMenuGroup).next()).slideUp("slow", function () {
        $(pMenuGroup).next().slideDown("slow");
    });
}

/*----------------------------------------------------:edit-------------------------------------------------*/
function SavePage(DisableDynamicControlsOnSave) {
    if (!$(".loading").is(":visible")) {
        if (form_validation($(".inner-content-area"), true)) {
            $(".loading").show();
            $(".result-box-container").fadeOut();
            $(".error-message").fadeOut();
            $(".input").removeClass("input-error", 100);

            if (DisableDynamicControlsOnSave)
                $(".inner-content-area").find(".dynamic-section").find("input").attr("disabled", true);

            $.post($(".inner-content-area").attr("action"), $(".inner-content-area").serialize(), function (data) {

                if (DisableDynamicControlsOnSave)
                    $(".inner-content-area").find(".dynamic-section").find("input").attr("disabled", false);

                if (data["ErrorFields"]) {
                    $.each(data["ErrorFields"], function (i, item) {
                        $(item).addClass("input-error", "slow");
                        if ($(item).next().size() > 0 && $(item).next().hasClass("error-message")) {
                            $(item).next().find(".error-message-text").html($(item).attr("data-req-mess"));
                            $(item).next().show();
                        }
                        else {
                            $(item).after("<div class='error-message'><div class='error-message-decor'></div><div class='error-message-text'></div></div>");
                            $(item).next().find(".error-message-text").html($(item).attr("data-req-mess"));
                        }
                    });
                }
                if (data["Result"] == 2) {
                    ShowMessage(data["Message"], "success", true);
                    window.setTimeout(function () { window.location = data["RedirectURL"]; }, 500);
                    return;
                } else if (data["Result"] == 3) {
                    ShowMessage(data["Message"], "error", true);
                } else if (data["Result"] == 0) {
                    ShowMessage(data["Message"], "error", true);
                } else {
                    ShowMessage(data["Message"], "success", true);
                }
            });
        }
    }
    return false;
}

function DeleteItem(Id, Namespace, message = 'Подтвердить удаление?') {
    if (!$(".loading").is(":visible")) {
        ShowConfirmMessage(message, () => {
            $(".loading").show();
            $.post(gRootUrl + 'DocControl/Delete', { Id: Id, Namespace: Namespace }, data => {
                if (data.Result == 0)
                    ShowMessage(data.Message, "error", true);
                else
                    location.reload();
            });
        });
    }
}

function DeletePage(Id, Namespace) {
    if (!$(".loading").is(":visible")) {
        ShowConfirmMessage("Подтвердить удаление?", () => {
            $(".loading").show();
            $(".result-box-container").fadeOut();
            $(".error-message").fadeOut();
            $.post($(".inner-content-area").attr("action").replace("/Save", "/Delete"), { Id: Id, Namespace: Namespace }, data => {
                if (data.Result == 2) {
                    ShowMessage(data.Message, "success", false);
                    window.setTimeout(() => { $(".loading").hide(); window.location = data.RedirectURL; }, 500);
                }
                else if (data.Result == 0) {
                    ShowMessage(data.Message, "error", true);
                }
                else {
                    if (data.ErrorFields) {
                        $.each(data.ErrorFields, function (i, item) {
                            $("" + item).addClass("input-error", "slow");
                            if ($("" + item).next().size() > 0 && $("" + item).next().hasClass("error-message")) {
                                $("" + item).next().find(".error-message-text").html($("" + item).attr("data-req-mess"));
                            }
                            else {
                                $("" + item).after("<div class='error-message'></div>");
                                $("" + item).next().find(".error-message-text").html($("" + item).attr("data-req-mess"));
                            }
                        });
                    }
                    ShowMessage(data.Message, "success", true);
                }
            });
        });
    }
}

function UpdatePage() {
    if (!$(".loading").is(":visible")) {
        if (form_validation($(".inner-content-area"), true)) {
            $(".loading").show();
            $(".result-box-container").fadeOut();
            $(".error-message").fadeOut();
            $(".input").removeClass("input-error", 100);

            $(".inner-content-area").find(".dynamic-section").find("input").attr("disabled", true);

            $.post($(".inner-content-area").attr("action").replace("/Save", "/Update"), $(".inner-content-area").serialize(), function (data) {
                if (data.Result != 0) {
                    ShowMessage(data["Message"], "success", true);
                    setTimeout(() => window.location.reload(), 500);
                } else
                    ShowMessage(data["Message"], "error", true);
            });
        }
    }
}

function ShowMessage(Message, Type, hideLoading, timeToHide = 2000) {
    if (hideLoading)
        $(".loading").hide();
    if (Message)
        $(".result-box").html(Message);
    else
        $(".result-box").html("Datele sau memorizat cu success!");

    $(".result-box").removeClass("result-error").addClass("result-" + Type);

    $(".result-box-container").fadeIn("slow");

    window.setTimeout(function () { $(".result-box-container").fadeOut("slow"); }, timeToHide);
}

function PasswordEqual(pInut) {
    if ($(pInut).val() != $("[name=PasswordConfirm]").val()) {
        $("[name=PasswordConfirm]").addClass("input-error", "slow");
        return true;
    }
    return false;
}

function save_popUpHtml() {
    if (gHtmlPopupControl) {
        var val = $("#popUpHtml").val();
        gHtmlPopupControl.val($(val).text());
        gHtmlPopupControl.next().next().val(val);
    }
    if ($("#PopUpSaveFunction").size() > 0) {
        eval($("#PopUpSaveFunction").val());
    }
    else {
        $.fancybox.close();
        gHtmlPopupControl.focus();
        gHtmlPopupControl = null;
    }
    return false;
}

function clear_popUpHtml() {
    $("#popUpHtml").val("");
    return false;
}
/*----------------------------------------------------:search---------------------------------------------------*/
function doSearch(report) {
    window.location = gRootUrl + "Report/" + report + "/?s=" + $("[name=global_searh]").val();
    return false;
}

function search_on_enter(input, event) {
    if (event == null)
        event = window.event;

    var keypressed = event.keyCode || event.which;
    if (keypressed == 13)
        doSearch('OrderList');
}
/*----------------------------------------------------:print---------------------------------------------------*/
function intit_print_dialog(responce) {
    $("#printArea").html(responce);

    var timer = setInterval(function () {
        if ($("#printArea").find(".print-end").size() > 0) {
            $(".loading").hide();
            $("#printArea").printArea();
            clearInterval(timer);
        }
    }, 200);
}

function print(postArray) {
    if (!$(".loading").is(":visible")) {
        $(".loading").show();
        $.ajax({
            type: 'POST',
            url: gRootUrl + "Print/Print/",
            async: true,
            data: postArray,
            success: function (responce) {
                intit_print_dialog(responce);
            }
        });
    }
    return false;
}
function ExportToWord(postArray) {
    if (!$(".loading").is(":visible")) {
        $(".loading").show();
        $.fileDownload(gRootUrl + "Print/ExportWord/", {
            httpMethod: "POST",
            data: postArray,
            successCallback: function (url) {
                $(".loading").hide();
            },
            failCallback: function (html, url) {
                $(".loading").hide();
                ShowMessage(html, "error", true);
            }
        });
        $(".loading").hide();
    }
    return false;
}
/*-------------------------------------------------------------------------------------------------------------*/

function load_autocomplete(item) {
    var control = $(item).autocomplete({
        source: function (request, response) {
            $(item).parent().find(".clear-link").addClass("autocolmplete-loading");

            gpostArray = {};

            if ($("#autocomplite-advanced-filter")[0] != undefined)
                gpostArray = { AdvancedFilter: $("#autocomplite-advanced-filter")[0].value }

            $.ajax({
                dataType: "json",
                type: 'Get',
                url: gRootUrl + "AutoComplete/" + $(item).attr("data-namespace") + "/?term=" + request.term + "&cond=" + ($(item).attr("data-cond") != undefined ? $(item).attr("data-cond") : ''),
                data: gpostArray,
                success: function (data) {
                    $(item).parent().find(".clear-link").removeClass("autocolmplete-loading");
                    $(item).removeClass('ui-autocomplete-loading');
                    response($.map(data, function (item) {
                        item.label = latinizeDecode(unescape(JSON.parse('"' + item.label + '"')));
                        return item;
                    }));
                },
                error: (data) => $(item).parent().find(".clear-link").removeClass("autocolmplete-loading")
            });
        },
        _renderItem: function (ul, item) {
            return $("<li>")
                .append(unescape(JSON.parse('"' + item.label + '"')))
                .appendTo(ul);
        },
        formatResult: function (row) {
            alert(row);
            return $('<div/>').html(row).html();
        },
        minLength: $(item).attr("data-AutocompleteMinLen"),
        select: function (event, ui) {
            var name = $("<li></li>").html(ui.item.label).text();
            $(this).val(name);
            $(this).parent().find("input[type=hidden]").val(ui.item.value);
            if ($(this).attr("onchange") != null && $(this).attr("onchange") != "") {
                eval($(this).attr("onchange"));
            }
            return false;
        },
        focus: (event, ui) => event.preventDefault(),
        change: function (event, ui) {
            if (ui == null || ui.item == null) {
                $(this).parent().find("input[type=hidden]").val("0");
            }
        }
    }).focus(() => $(item).autocomplete("search"));

    control.data("ui-autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>").data("item.autocomplete", item)
            .append($('<div/>').html(item.label).text())
            .appendTo(ul);
    };
}

function attach_autocomplete(item) {
    var control = $(item).autocomplete({
        source: function (request, response) {
            var matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex(request.term), "i");
            response($.grep(eval($(item).attr("data-AutocompleteName")), function (item) {
                item.label = latinizeDecode(item.label);
                return matcher.test(item.label);
            }));
        },
        minLength: $(item).attr("data-AutocompleteMinLen"),
        select: function (event, ui) {
            var name = $("<li></li>").html(ui.item.label).text();
            $(this).val(name);
            $(this).parent().find("input[type=hidden]").val(ui.item.value);
            if ($(item).attr("onchange") != null && $(item).attr("onchange") != "") {
                eval($(item).attr("onchange"));
            }
            return false;
        },
        change: function (event, ui) {
            if (ui != null && ui.item == null) {
                $(this).parent().find("input[type=hidden]").val("");
            }
        },
        focus: (event, ui) => event.preventDefault()
    });
    control.focus(() => $(this).autocomplete("search"));
}

function initUploadImageFile(purl, pUniqueId, pAdminWidth, pAdminHeight, pWidth, pHeight, pBOName) {
    $("#UploadFile").ajaxUpload({
        url: purl + "/?AdminWidth=" + pAdminWidth + "&AdminHeight=" + pAdminHeight + "&Width=" + pWidth + "&Height=" + pHeight + "&BOName=" + pBOName,
        name: "file",
        dataType: "JSON",
        onSubmit: function () {
            $("#upload_image_container_" + pUniqueId).find('.upload-image-loading').show();
            return true;
        },
        onComplete: function (responce) {
            $("#upload_image_container_" + pUniqueId).find('.upload-image-loading').hide();

            jsonResponce = JSON.parse(responce);
            if (jsonResponce.Result == 0) {
                alert(jsonResponce.Message);
                return true;
            }
            $("[name=" + pUniqueId + "]").val(jsonResponce.Data.Id);
            $("#" + pUniqueId + "_link").attr("href", jsonResponce.Data.thumb);
            $("#" + pUniqueId + "_link").find("img").attr("src", jsonResponce.Data.thumb);

            return true;
        }
    });
}

function initUploadImageFileAutoSave(purl, pUniqueId, pAdminWidth, pAdminHeight, pWidth, pHeight, pBOName, NameSpace, Id) {
    $("#UploadFile").ajaxUpload({
        url: purl + "/?AdminWidth=" + pAdminWidth + "&AdminHeight=" + pAdminHeight + "&Width=" + pWidth + "&Height=" + pHeight + "&BOName=" + pBOName,
        name: "file",
        dataType: "JSON",
        onSubmit: function () {
            $("#upload_image_container_" + pUniqueId).find('.upload-image-loading').show();
            return true;
        },
        onComplete: function (responce) {
            $("#upload_image_container_" + pUniqueId).find('.upload-image-loading').hide();

            jsonResponce = JSON.parse(responce);
            if (jsonResponce.Result == 0) {
                alert(jsonResponce.Message);
                return true;
            }
            $("[name=" + pUniqueId + "]").val(jsonResponce.Id);
            $("#" + pUniqueId + "_link").attr("href", jsonResponce.thumb);
            $("#" + pUniqueId + "_link").find("img").attr("src", jsonResponce.thumb);

            $.ajax({
                type: "POST",
                url: gRootUrl + "Order/UpdataImage",
                data: {
                    NameSpace: NameSpace,
                    Id: Id,
                    ImageId: jsonResponce.Id
                }
            });

            return true;
        }
    });
}

function get_type_model(GroupProductId) {
    $.post(gRootUrl + "Product/GetTypeProductOptions", { GroupProductId: GroupProductId }, options => $('[name=TypeProduct]').html(options));
}

function set_finding_specie(findingSpecieId) {
    $.post(gRootUrl + "Production/GetFindingSubspecieOptions", { FindingSpecieId: findingSpecieId }, options => $('[name=FindingSubspecie]').html(options));
}

function get_scrollWidth() {
    let div = document.createElement('div');

    div.style.overflowX = 'scroll';
    div.style.width = '50px';
    div.style.height = '50px';

    document.body.append(div);

    let scrollWidth = div.offsetWidth - div.clientWidth;

    div.remove();

    return scrollWidth;
}