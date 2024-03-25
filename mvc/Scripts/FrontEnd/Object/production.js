function set_Textile_data_dynamic(dynamicCell) {
    let dynamicRow = $(dynamicCell.closest(".edit-section-table-row"));

    let TextileId = dynamicRow.find('[name=Textile]').val();

    if (!TextileId || TextileId === '0')
        return;

    set_Textile_color_options(dynamicRow, TextileId);

    $.post(gRootUrl + "Production/GetTextile", { TextileId: TextileId }, textile => {
        dynamicRow.find('[name=Code]').val(textile.Code);
        dynamicRow.find('[name=Compound]').val(textile.Compound.Name);
    });
}

function set_ImplementSupplySpecificProductUnit_data(block, implementSupplySpecificProductUnitId) {
    let popUp = $('.popup-content');

    let count = popUp.find('[name=Count]'),
        date = popUp.find('[name=Date]'),
        id = popUp.find('[name=Id]');

    if (implementSupplySpecificProductUnitId > 0) {
        $.post(gRootUrl + "Production/GetImplementSupplySpecificProductUnit", { ImplementSupplySpecificProductUnitId: implementSupplySpecificProductUnitId }, implementSupplySpecificProductUnit => {
            count.val(implementSupplySpecificProductUnit.Count);
            date.val(implementSupplySpecificProductUnit.DateString);
            id.val(implementSupplySpecificProductUnit.Id);
        });
    }
    else {
        let clearInput = popUp.find('input, select, textarea').not('[dontAutoClear]');

        clearInput.val(0);
        clearInput.not('[type=hidden]').val('');
    }

    $('.select-item.hover-button').removeClass('hover-button');
    $(block).addClass('hover-button');
}

function set_TailoringSupplySpecificProductUnit_data(block, tailoringSupplySpecificProductUnitId) {
    let popUp = $('.popup-content');

    let factoryTailoring = popUp.find('[name=FactoryTailoring]'),
        tailoringCost = popUp.find('[name=TailoringCost]'),
        count = popUp.find('[name=Count]'),
        date = popUp.find('[name=Date]'),
        id = popUp.find('[name=Id]'),
        print_btn = popUp.find('#print_FindingColorConsumptionList'),
        dynamic_FindingLocationStorageTailoringSupplySpecificProductUnit = popUp.find('.dynamic-section'),
        dynamicRows_FindingLocationStorageTailoringSupplySpecificProductUnit = dynamic_FindingLocationStorageTailoringSupplySpecificProductUnit.find('.edit-section-table-row');

    if (tailoringSupplySpecificProductUnitId > 0) {
        $.post(gRootUrl + "Production/GetTailoringSupplySpecificProductUnit", { TailoringSupplySpecificProductUnitId: tailoringSupplySpecificProductUnitId }, tailoringSupplySpecificProductUnit => {
            factoryTailoring.val(tailoringSupplySpecificProductUnit.FactoryTailoringId);
            tailoringCost.val(tailoringSupplySpecificProductUnit.TailoringCost);
            count.val(tailoringSupplySpecificProductUnit.Count);
            date.val(tailoringSupplySpecificProductUnit.DateString);
            id.val(tailoringSupplySpecificProductUnit.Id);
            print_btn.attr('onclick', "print_item_document('FindingColorConsumptionList', { ModelId: " + tailoringSupplySpecificProductUnit.Id + ", Namespace: 'JuliaAlert.Models.Objects.TailoringSupplySpecificProductUnit' })");
            dynamicRows_FindingLocationStorageTailoringSupplySpecificProductUnit.each((i, dynamicRow) => {
                let findingLocationStorageTailoringSupplySpecificProductUnitBlock = $(dynamicRow).find('[name=FindingLocationStorageTailoringSupplySpecificProductUnit]'),
                    locationStorage = $(dynamicRow).find('[name=LocationStorage]'),
                    tailoringSupplySpecificProductUnitBlock = $(dynamicRow).find('[name=TailoringSupplySpecificProductUnit]');

                findingLocationStorageTailoringSupplySpecificProductUnitBlock.val(0);
                locationStorage.val(0);
                tailoringSupplySpecificProductUnitBlock.val(tailoringSupplySpecificProductUnitId);

                tailoringSupplySpecificProductUnit.FindingLocationStorageTailoringSupplySpecificProductUnits.forEach(findingLocationStorageTailoringSupplySpecificProductUnit => {
                    if (String(findingLocationStorageTailoringSupplySpecificProductUnit.FindingColor.Id) === $(dynamicRow).find('[name=FindingColor]').val()) {
                        findingLocationStorageTailoringSupplySpecificProductUnitBlock.val(findingLocationStorageTailoringSupplySpecificProductUnit.Id);
                        locationStorage.val(findingLocationStorageTailoringSupplySpecificProductUnit.LocationStorage.Id);
                    }
                })
            });
            dynamic_FindingLocationStorageTailoringSupplySpecificProductUnit.removeClass('display-none');
        });
    }
    else {
        let clearInput = popUp.find('input, select, textarea').not('[dontAutoClear]');

        dynamic_FindingLocationStorageTailoringSupplySpecificProductUnit.addClass('display-none');

        clearInput.val(0);
        clearInput.not('[type=hidden]').val('');
    }

    $('.select-item.hover-button').removeClass('hover-button');
    $(block).addClass('hover-button');
}

function set_Textile_color_options(dynamicRow, TextileId) {
    $.post(
        gRootUrl + "Production/GetTextileColorOptions",
        { TextileId: TextileId },
        options => dynamicRow.find('[name=ColorProduct]').html(options)
    )
}

function set_Finding_data_dynamic(autocomplite) {
    let dynamicRow = $(autocomplite.closest(".edit-section-table-row"));

    let FindingId = dynamicRow.find('[name=Finding]').val();

    if (!FindingId || FindingId === '0')
        return;

    set_Finding_color_options(dynamicRow, FindingId);

    $.post(gRootUrl + "Production/GetFinding", { FindingId: FindingId }, finding => dynamicRow.find('[name=Code]').val(finding.Code));
}

function set_Finding_color_options(dynamicRow, FindingId) {
    $.post(
        gRootUrl + "Production/GetFindingColorOptions",
        { FindingId: FindingId },
        options => dynamicRow.find('[name=ColorProduct]').html(options)
    )
}

function get_final_price(dynamicCell) {
    let dynamicRow = $(dynamicCell).closest(".edit-section-table-row");

    let count = dynamicRow.find('[name=Count]');
    let price = dynamicRow.find('[name=Price]');

    if (count.val() && price.val()) {
        dynamicRow.find('[name=FinalPrice]').val((Number(count.val()) * Number(price.val())).toFixed(2));
    }
}

function save_supply_textile_return() {
    save_popup(false).done(response => {
        if (response.Result !== 0) {
            $('#supply_textile_unit_dynamic [name=Id]').each((i, Id) => {
                if (Id.value === $('.popup-container [name=SupplyTextileUnit]').val()) {
                    $(Id).closest('.edit-section-table-row').find('.return-btn').addClass('button-red');
                    return;
                }
            });
            $.fancybox.close();
        }
    });
}

function save_supply_finding_return() {
    save_popup(false).done(response => {
        if (response.Result !== 0) {
            $('#supply_finding_unit_dynamic [name=Id]').each((i, Id) => {
                if (Id.value === $('.popup-container [name=SupplyFindingUnit]').val()) {
                    $(Id).closest('.edit-section-table-row').find('.return-btn').addClass('button-red');
                    return;
                }
            });
            $.fancybox.close();
        }
    });
}

function swap_places_stocks() {
    let stockFrom = $('[name=StockFrom]'),
        countOnStockFrom = $('[name=CountOnStockFrom]'),
        stockTo = $('[name=StockTo]'),
        countOnStockTo = $('[name=CountOnStockTo]');

    let stockFromVal = stockFrom.val(),
        countOnStockFromVal = countOnStockFrom.val();

    stockFrom.val(stockTo.val());
    countOnStockFrom.val(countOnStockTo.val());

    stockTo.val(stockFromVal);
    countOnStockTo.val(countOnStockFromVal);
}

function save_ImplementSupplySpecificProductUnit(popupNamespace, popupItemId, popupView, customData) {
    let popUp = $('.popup-content');

    save_popup_reload_popup(popupNamespace, popupItemId, popupView, customData);

    $('#supply_specific_product_unit_dynamic [name=Id]').each((i, idBlock) => {
        if (idBlock.value === popUp.find('[name=SupplySpecificProductUnit]').val()) {
            let button = $(idBlock).closest('.edit-section-table-row').find('#implement-button');

            $.post(gRootUrl + "Production/GetSupplySpecificProductUnit", { SupplySpecificProductUnitId: idBlock.value }, supplySpecificProductUnit => {
                button.removeClass('button-green');
                button.removeClass('button-orange');

                if (supplySpecificProductUnit.TotalImplementCount === supplySpecificProductUnit.Count)
                    button.addClass('button-green');
                else
                    button.addClass('button-orange');
            });

            return;
        }
    });
}

function save_TailoringSupplySpecificProductUnit(popupNamespace, popupItemId, popupView, customData) {
    let popUp = $('.popup-content');

    save_popup_reload_popup(popupNamespace, popupItemId, popupView, customData);

    $('#supply_specific_product_unit_dynamic [name=Id]').each((i, idBlock) => {
        if (idBlock.value === popUp.find('[name=SupplySpecificProductUnit]').val()) {
            let button = $(idBlock).closest('.edit-section-table-row').find('#tailoring-button');

            $.post(gRootUrl + "Production/GetSupplySpecificProductUnit", { SupplySpecificProductUnitId: idBlock.value }, supplySpecificProductUnit => {
                button.removeClass('button-green');
                button.removeClass('button-orange');

                if (supplySpecificProductUnit.TotalTailoringCount === supplySpecificProductUnit.Count)
                    button.addClass('button-green');
                else
                    button.addClass('button-orange');
            });

            return;
        }
    });
}

function delete_ImplementSupplySpecificProductUnit(popupNamespace, popupItemId, popupView, customData) {
    let popUp = $('.popup-content');

    delete_popup_reload_popup(popupNamespace, popupItemId, popupView, customData);

    $('#supply_specific_product_unit_dynamic [name=Id]').each((i, idBlock) => {
        if (idBlock.value === popUp.find('[name=SupplySpecificProductUnit]').val()) {

            let button = $(idBlock).closest('.edit-section-table-row').find('#implement-button');

            $.post(gRootUrl + "Production/GetSupplySpecificProductUnit", { SupplySpecificProductUnitId: idBlock.value }, supplySpecificProductUnit => {
                button.removeClass('button-green');
                button.removeClass('button-orange');

                if (supplySpecificProductUnit.TotalImplementCount === supplySpecificProductUnit.Count)
                    button.addClass('button-green');
                else
                    button.addClass('button-orange');
            });

            return;
        }
    });
}

function delete_TailoringSupplySpecificProductUnit(popupNamespace, popupItemId, popupView, customData) {
    let popUp = $('.popup-content');

    delete_popup_reload_popup(popupNamespace, popupItemId, popupView, customData);

    $('#supply_specific_product_unit_dynamic [name=Id]').each((i, idBlock) => {
        if (idBlock.value === popUp.find('[name=SupplySpecificProductUnit]').val()) {

            let button = $(idBlock).closest('.edit-section-table-row').find('#tailoring-button');

            $.post(gRootUrl + "Production/GetSupplySpecificProductUnit", { SupplySpecificProductUnitId: idBlock.value }, supplySpecificProductUnit => {
                button.removeClass('button-green');
                button.removeClass('button-orange');

                if (supplySpecificProductUnit.TotalTailoringCount === supplySpecificProductUnit.Count)
                    button.addClass('button-green');
                else
                    button.addClass('button-orange');
            });

            return;
        }
    });
}

function save_LocationStorage_dynamic(dynamicCell) {
    let dynamicRow = $(dynamicCell).closest('.edit-section-table-row');

    let findingLocationStorageTailoringSupplySpecificProductUnit = dynamicRow.find('[name=FindingLocationStorageTailoringSupplySpecificProductUnit]');

    let data = {
        FindingLocationStorageTailoringSupplySpecificProductUnit: findingLocationStorageTailoringSupplySpecificProductUnit.val(),
        TailoringSupplySpecificProductUnit: dynamicRow.find('[name=TailoringSupplySpecificProductUnit]').val(),
        LocationStorage: dynamicRow.find('[name=LocationStorage]').val(),
        FindingColor: dynamicRow.find('[name=FindingColor]').val(),
        Consumption: dynamicRow.find('[name=Consumption]').val()
    };

    $.post(gRootUrl + "Production/SaveLocationStorage", data, findingLocationStorageTailoringSupplySpecificProductUnitId => findingLocationStorageTailoringSupplySpecificProductUnit.val(findingLocationStorageTailoringSupplySpecificProductUnitId));
}