/*----------------------------------------------------:Error Handling--------------------------------------------*/

/*----------------------------------------------------:validation-----------------------------------------------*/

/*----------------------------------------------------:edit-----------------------------------------------------*/
function link_on_after_update_function(pControl, gpostArray) {
    gpostArray[pControl.find("input").attr("name")] = pControl.find("input").val();
    return pControl.find("input").val();
}