/*----------------------------------------------------:Error Handling--------------------------------------------*/

/*----------------------------------------------------:validation-----------------------------------------------*/

/*----------------------------------------------------:edit-----------------------------------------------------*/
function input_on_after_update_function(pControl, gpostArray) {
    var pInput;
    var value = ""
    if (pControl.find("input").size() == 1) {
        pInput = pControl.find("input");
        value = pInput.val();
    }
    else {
        pInput = $(pControl.find("input")[0]);
        value = pInput.val();

        gpostArray[$(pControl.find("input")[1]).attr("name")] = $(pControl.find("input")[1]).val();
        value += " " + $(pControl.find("input")[1]).val();

        gpostArray[$(pControl.find("input")[2]).attr("name")] = $(pControl.find("input")[2]).val();
        value += ":" + $(pControl.find("input")[2]).val();
    }
    gpostArray[pInput.attr("name")] = pInput.val();
    if (pControl.prev().hasClass("control-view")) {
        pControl.prev().html(value);
    }
    return { "name": pInput.attr("name"), "value": pInput.val() };
}

function color_on_after_update_function(pControl, gpostArray) {
    let pInput = pControl.find("input");
    let value = pInput.val();

    gpostArray[pInput.attr("name")] = pInput.val();

    if (pControl.prev().hasClass("control-view")) {
        pControl.prev().css('background', value);
    }
    return { "name": pInput.attr("name"), "value": pInput.val() };
}

function textarea_on_after_update_function(pControl, gpostArray) {
    gpostArray[pControl.find("textarea").attr("name")] = pControl.find("textarea").val();
    if (pControl.prev().hasClass("control-view")) {
        pControl.prev().html(pControl.find("textarea").val());
    }
    return { "name": pControl.find("textarea").attr("name"), "value": encodeURIComponent(pControl.find("textarea").val()) };
}

function input_on_clear(pControl) {
    pControl.find("textarea").val("input");
}

function textarea_on_clear(pControl) {
    pControl.find("textarea").val("");
}