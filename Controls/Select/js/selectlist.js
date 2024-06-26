/*----------------------------------------------------:edit-----------------------------------------------------*/
function selectlist_on_after_update_function(pControl, gpostArray) {
    if (pControl.find(".hidden-autocomplete-input").size() != 0) {
        gpostArray[pControl.find(".hidden-autocomplete-input").attr("name")+ '_id'] = pControl.find(".hidden-autocomplete-input").val();

        if (pControl.prev().hasClass("control-view")) {
            pControl.prev().html(pControl.find(".autocomplete-input").val());
        }

        return { "name": pControl.find(".autocomplete-input").attr("name") + "_id", "value": pControl.find("[name=" + pControl.find(".autocomplete-input").attr("name") + "_id]").val() };
    }
    else {
        gpostArray[pControl.find("input").attr("name")] = pControl.find("input").val();

        return { "name": pControl.find("input").attr("name"), "value": pControl.find("input").val() };
    }

    return null;
}


function selectlist_on_clear(pControl) {
    pControl.find(".autocomplete-input").val("");
    pControl.find("name=[" + pControl.find(".autocomplete-input").attr("name") + "_id]").val("");
}
