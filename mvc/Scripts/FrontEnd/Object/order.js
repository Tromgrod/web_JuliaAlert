function get_discount(input) {
    let dynamicRow = $(input).closest('.edit-section-table-row');

    let price = dynamicRow.find('[name=Price]');
    let finalPrice = dynamicRow.find('[name=FinalPrice]');

    let discount = dynamicRow.find('[name=Discount]');

    if (price.val() > 0 && finalPrice.val() > 0)
        discount.val(((1 - finalPrice.val() / price.val()) * 100).toFixed(0));

    get_final_price(input);
    change_header_price();
}

function get_price(input) {
    let dynamicRow = $(input).closest('.edit-section-table-row');

    let price = dynamicRow.find('[name=Price]');
    let finalPrice = dynamicRow.find('[name=FinalPrice]');

    let discount = dynamicRow.find('[name=Discount]');

    if (!price.val() && finalPrice.val() && discount.val() > 0) {
        let newPrice = finalPrice.val() / (1 - discount.val() / 100);

        price.val(newPrice.toFixed(2));
    }
    else if (price.val() && discount.val() > 0 && input != finalPrice.context) {
        let newFinalPrice = price.val() * (1 - discount.val() / 100)

        finalPrice.val(newFinalPrice.toFixed(2));
    }

    get_final_price(input);
    change_header_price();
}

function delete_return() {
    delete_popup(false).done(response => {
        if (response.Result !== 0) {
            $('#product_for_order_dynamic [name=Id]').each((i, elem) => {
                if (elem.value === $('.popup-container [name=ProductForOrder]').val()) {
                    $(elem).closest('.edit-section-table-row').find('.return-btn').removeClass('button-red');
                    return;
                }
            });

            $.fancybox.close();
        }
    });
}

function save_return(gpostArray = {}) {
    save_popup(false, 'HtmlPopUp/Save', gpostArray).done(response => {
        if (response.Result !== 0) {
            $('#product_for_order_dynamic [name=Id]').each((i, elem) => {
                if (elem.value === $('.popup-container [name=ProductForOrder]').val()) {
                    $(elem).closest('.edit-section-table-row').find('.return-btn').addClass('button-red');
                    return;
                }
            });

            let OrderState = $('[name=OrderState]')[0];

            let productForOrderCount = 0;

            let allRow = $('#product_for_order_dynamic .edit-section-table-row').not('.edit-section-table-add');

            allRow.each((i, row) => {
                if ($(row).find('[name=Id]').val() === $('.popup-content').find('[name=ProductForOrder]').val())
                    productForOrderCount = Number($(row).find('[name=Count]').val());
            });

            if ($('.popup-content [name=ReceivingReturnDate]').length > 0 && $('.popup-content [name=ReceivingReturnDate]').val() === '') {
                OrderState.selectedIndex = gRefundClaimedId;
            }
            else if (productForOrderCount !== Number($('[name=ReturnCount]').val()) || $('#product_for_order_dynamic .button-red').length !== allRow.length) {
                OrderState.selectedIndex = gPartialReturnId;
            }
            else {
                OrderState.selectedIndex = gReturnId;
            }

            $.fancybox.close();
        }
    });
}

function save_local_return() {
    let gpostArray = {
        ReceivingReturnDate: $('[name=ReturnDate]').val(),
        InCountry: 1
    };

    save_return(gpostArray);
}

function change_date() {
    let OrderDate = $('[name=OrderDate]'),
        DepartureDate = $('[name=DepartureDate]'),
        ReceivingDate = $('[name=ReceivingDate]');

    let OrderState = $('[name=OrderState]')[0];

    if (OrderDate.val() !== '') {
        if (DepartureDate.val() !== '' && ReceivingDate.val() !== '') {
            OrderState.selectedIndex = gDeliveredId;
        }
        else if (DepartureDate.val() !== '' && ReceivingDate.val() === '') {
            OrderState.selectedIndex = gSentId;
        }
        else if (DepartureDate.val() === '' && ReceivingDate.val() === '') {
            OrderState.selectedIndex = gAcceptedId;
        }
    }
}

function get_final_price(input) {
    let dynamicRow = $(input).closest('.edit-section-table-row');

    let count = dynamicRow.find('[name=Count]');
    let finalPrice = dynamicRow.find('[name=FinalPrice]');

    dynamicRow.find('[name=FinalPriceCount]').val((Number(finalPrice.val()) * Number(count.val())).toFixed(2));

    change_header_price();
}

function change_header_price() {
    let price = 0,
        discount = 0;

    let TAX = Number($('#TAX').val()),
        Delivery = Number($('#Delivery').val());

    let rows = $('#product_for_order_dynamic .edit-section-table-row');
    let newRow = $('#product_for_order_dynamic .edit-section-table-add');

    if (newRow.attr('style') === undefined)
        rows = rows.not('.edit-section-table-add');
    rows.each((i, row) => {
        let rowPrice = Number($(row).find('[name=Price]').val()),
            rowCount = Number($(row).find('[name=Count]').val()),
            rowFinalPrice = Number($(row).find('[name=FinalPriceCount]').val());

        price += rowPrice * rowCount;
        discount += rowPrice * rowCount - rowFinalPrice;
    });

    if (TAX && Delivery) {
        price += TAX + Delivery;
    }

    let priceHeader = 'Заказы: ' + price.toFixed(2) + gCurrencyName + ' - ' + discount.toFixed(2) + gCurrencyName + ' = ' + (price - discount).toFixed(2) + gCurrencyName;

    $('#header-caption-price').html(priceHeader)
}

function clone_ProductForOrder_Dynamic(pBtn) {
    let row = $(pBtn).closest('.edit-section-table-row'),
        table = $(pBtn).closest('.edit-section-table'),
        newRow = table.find('.edit-section-table-add'),
        body = $(pBtn).closest('.dynamic-section').find('.edit-section-body-samples');

    let newRowHtml = newRow.html();

    newRow.find('[name=UniqueProduct_autocomplete]').val(row.find('[name=UniqueProduct_Name]').val());
    newRow.find('[name=UniqueProduct]').val(row.find('[name=UniqueProduct]').val());

    set_UniqueProduct_data_dynamic(newRow.find('[name=UniqueProduct_autocomplete]'));

    newRow.find('[name=Count]').val(row.find('[name=Count]').val());
    newRow.find('[name=FactoryPrice]').val(row.find('[name=FactoryPrice]').val());
    newRow.find('[name=Price]').val(row.find('[name=Price]').val());
    newRow.find('[name=FinalPrice]').val(row.find('[name=FinalPrice]').val());
    newRow.find('[name=FinalPriceCount]').val(row.find('[name=FinalPriceCount]').val());
    newRow.find('[name=Discount]').val(row.find('[name=Discount]').val());

    add_new_dynamic_section_hard(pBtn);

    newRowHtml = '<div class="edit-section-table-row edit-section-table-add">' + newRowHtml + '</div>'

    newRow.before(newRowHtml);

    body.height(body.find('.edit-section-table').height());

    change_header_price();
}

function change_header_count_gift(pBtn) {
    let dynamicSection = $(pBtn).closest('.dynamic-section'),
        rows = dynamicSection.find('.edit-section-table-row'),
        header = dynamicSection.find('.edit-section-header-caption');

    if (dynamicSection.find('.edit-section-table-add').attr('style') === undefined)
        rows = rows.not('.edit-section-table-add');

    let count = 0;

    rows.find('[name=Count]').each((i, input) => count += Number(input.value));

    header.html('Подарки: ' + count + ' ШТ')
}