var readingScaner = false;

$(() => {
    let barcode = "";
    let keypressTime;
    let scanerMaxDuration = 25;

    document.addEventListener('keydown', e => {
        searchReportReady = false;

        if (!readingScaner) {
            keypressTime = new Date().getTime();
            readingScaner = true;
        }

        let keypressDuration = new Date().getTime() - keypressTime;

        if (keypressDuration < scanerMaxDuration && e.keyCode === 13 && barcode.length > 4) {
            $.post(gRootUrl + 'BarcodeHelper/Scanning', { Code: barcode }, resultData => {
                if ($('html').hasClass('fancybox-lock'))
                    $.fancybox.close();

                if (resultData.Result == 1) {
                    if (resultData.Data != undefined && resultData.Data.BarFun != undefined) {
                        eval('BarFun_' + resultData.Data.BarFun + '(resultData)');
                    }
                }
                else if (resultData.Result == 0) {
                    ShowMessage(resultData.Message, "error", true);
                }
            })
            readingScaner = false;
            barcode = "";
            setTimeout(() => searchReportReady = true, 500);
        }
        else if (keypressDuration < scanerMaxDuration) {
            barcode += e.key;

            searchReportReady = true;
        }
        else {
            readingScaner = false;
            searchReportReady = true;
            barcode = "";

            if (keypressDuration > 500)
                barcode += e.key;
        }

        keypressTime = new Date().getTime();
    });
})

function BarFun_textileColor(resultData) {
    let supplyTextileUnitDynamic = $('#supply_textile_unit_dynamic');

    if (supplyTextileUnitDynamic.length > 0 && Number($('[name=Id]').val()) > 0) {
        supplyTextileUnitDynamic.find('#btn_open_dynamic').click();

        setTimeout(() => {
            supplyTextileUnitDynamic.find('.edit-section-body-samples').find(".edit-section-table-add").css("display", "table-row");

            let textileColor = resultData.Data.Object;

            let isUniqe = true;

            supplyTextileUnitDynamic.find('.edit-section-table-row').each((i, elem) => {
                let textileId = $(elem).find('[name=Textile]').val();
                let colorId = $(elem).find('[name=ColorProduct]').val();

                if (Number(textileId) === textileColor.Textile.Id && Number(colorId) === textileColor.ColorProduct.Id) {
                    ShowMessage('Ткань "' + textileColor.Textile.Name + '" с цветом "' + textileColor.ColorProduct.Name + '" уже была добавлена!', "error", true);
                    isUniqe = false;
                    return;
                }
            })

            if (isUniqe) {
                let dynamicRow = supplyTextileUnitDynamic.find('.edit-section-table-add');

                dynamicRow.find('[name=Textile_autocomplete]').val(textileColor.Textile.Name);
                dynamicRow.find('[name=Textile]').val(textileColor.Textile.Id);
                dynamicRow.find('[name=Code]').val(textileColor.Textile.Code);
                dynamicRow.find('[name=ColorProduct]').val(textileColor.ColorProduct.Id);
                dynamicRow.find('[name=Compound]').val(textileColor.Textile.Compound.Name);

                dynamicRow.find('[name=ColorProduct] > option').each((i, option) => {
                    option.setAttribute('hidden', true);

                    if (Number($(option).val()) === textileColor.ColorProduct.Id) {
                        option.removeAttribute('hidden');
                    }
                });

                ShowMessage('Отсканированно!', "success", true, 500);
            }
        }, 300)
    }
    else {
        window.open(String(resultData.RedirectURL));
    }
}

function BarFun_findingColor(resultData) {
    let supplyFindingUnitDynamic = $('#supply_finding_unit_dynamic');

    if (supplyFindingUnitDynamic.length > 0 && Number($('[name=Id]').val()) > 0) {
        supplyFindingUnitDynamic.find('#btn_open_dynamic').click();

        setTimeout(() => {
            supplyFindingUnitDynamic.find('.edit-section-body-samples').find(".edit-section-table-add").css("display", "table-row");

            let findingColor = resultData.Data.Object;

            let isUniqe = true;

            supplyFindingUnitDynamic.find('.edit-section-table-row').each((i, dynamicRow) => {
                let findingId = $(dynamicRow).find('[name=Finding]').val();
                let colorId = $(dynamicRow).find('[name=ColorProduct]').val();
                let count = $(dynamicRow).find('[name=Count]');

                if (Number(findingId) === findingColor.Finding.Id && Number(colorId) === findingColor.ColorProduct.Id) {
                    if (!count.val()) {
                        count.val(1);
                    }
                    else {
                        ShowMessage('Отсканированно!', "success", true, 500);
                        count.val(Number(count.val()) + 1);

                        get_final_price(dynamicRow);
                    }

                    isUniqe = false;
                    return;
                }
            })

            if (isUniqe) {
                let dynamicRow = supplyFindingUnitDynamic.find('.edit-section-table-add');

                dynamicRow.find('[name=Finding_autocomplete]').val(findingColor.Finding.Name);
                dynamicRow.find('[name=Finding]').val(findingColor.Finding.Id);
                dynamicRow.find('[name=Code]').val(findingColor.Finding.Code);
                dynamicRow.find('[name=ColorProduct]').val(findingColor.ColorProduct.Id);
                dynamicRow.find('[name=Count]').val(1);

                if (findingColor.LastPrice > 0)
                    dynamicRow.find('[name=Price]').val(findingColor.LastPrice);

                dynamicRow.find('[name=ColorProduct] > option').each((i, option) => {
                    option.setAttribute('hidden', true);

                    if (Number($(option).val()) === findingColor.ColorProduct.Id) {
                        option.removeAttribute('hidden');
                    }
                });

                get_final_price(dynamicRow);

                ShowMessage('Отсканированно!', "success", true, 500);
            }
        }, 300)
    }
    else {
        window.open(String(resultData.RedirectURL));
    }
}

var oldSpecificProductId;

function BarFun_specificProduct(resultData) {
    let specificProduct = resultData.Data.Object;

    if ($('[name=Inventory]').length > 0) {
        let searchDataGrid = $('.search-data-grid');

        searchDataGrid.find('[data-autocompletename=UniqueProduct]').val(specificProduct.FullName);
        searchDataGrid.find('[name=UniqueProduct]').val(specificProduct.UniqueProduct.Id);
        searchDataGrid.find('[name=ProductSize]').val(specificProduct.ProductSize.Id);

        if (searchFocusObj && oldSpecificProductId && oldSpecificProductId === specificProduct.Id) {
            uploadCurrentCount(specificProduct);
        }
        else {
            SearchReport().done(_ => {
                searchFocusObj = true;
                setTimeout(_ => uploadCurrentCount(specificProduct), 500);
            });
        }

        function uploadCurrentCount(specificProduct) {
            let specificProductBlock = $('.data-grid-container [name=SpecificProductId][value=' + specificProduct.Id + ']');

            let currentCountBlock = specificProductBlock.closest('.data-grid-data-row').find('.CurrentCount');

            currentCountBlock.val(Number(currentCountBlock.val()) + 1);

            InventoryList_change_CurrentCount(currentCountBlock);
        }
    }
    else {
        window.open(String(resultData.RedirectURL));
    }

    oldSpecificProductId = specificProduct.Id;
}