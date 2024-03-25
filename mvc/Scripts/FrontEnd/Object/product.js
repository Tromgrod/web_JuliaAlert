function set_UniqueProduct_data_dynamic(autocomplite) {
    let dynamicRow = $(autocomplite).closest(".edit-section-table-row");

    set_UniqueProduct_data(dynamicRow);
}

function set_UniqueProduct_data_form(autocomplite) {
    let section = $(autocomplite).closest(".edit-section-body");

    set_UniqueProduct_data(section);
}

function set_UniqueProduct_data(block) {
    let UniqueProductId = block.find('[name=UniqueProduct]').val();
    let SalesChannelId = $('[name=SalesChannel]').val();

    let data = {
        UniqueProductId: UniqueProductId,
        SalesChannelId: SalesChannelId
    };

    if (!UniqueProductId || UniqueProductId === '0')
        return;

    set_UniqueProduct_size_options(block, UniqueProductId);

    $.post(gRootUrl + "Product/GetUniqueProduct", data, uniqueProduct => {
        let ColorProduct = block.find('[name=ColorProduct]'),
            ColorProductName = block.find('[name=ColorProduct_Name]'),
            Decor = block.find('[name=Decor]'),
            DecorName = block.find('[name=Decor_Name]'),
            Code = block.find('[name=Code]'),
            Product = block.find('[name=Product]'),
            FactoryPrice = block.find('[name=FactoryPrice]'),
            Price = block.find('[name=Price]'),
            FactoryTailoring = block.find('[name=FactoryTailoring]'),
            TailoringCost = block.find('[name=TailoringCost]'),
            FactoryCut = block.find('[name=FactoryCut]'),
            SupplySpecificProductUnit = block.find('[name=SupplySpecificProductUnit]'),
            CutCost = block.find('[name=CutCost]');

        if ((ColorProduct.length > 0 || ColorProductName.length > 0) && uniqueProduct.ColorProduct) {
            ColorProductName.val(uniqueProduct.ColorProductName);
            ColorProduct.val(uniqueProduct.ColorProduct);
        }
        if ((Decor.length > 0 || DecorName.length > 0) && uniqueProduct.Decor) {
            DecorName.val(uniqueProduct.DecorName);
            Decor.val(uniqueProduct.Decor);
        }
        if (Product.length > 0 && uniqueProduct.Product) {
            Product.val(uniqueProduct.Product);
        }
        if (FactoryPrice.length > 0 && uniqueProduct.LastFactoryPrice) {
            FactoryPrice.val(uniqueProduct.LastFactoryPrice);
        }
        if (Price.length > 0 && uniqueProduct.Price) {
            Price.val(uniqueProduct.Price);

            let Count = block.find('[name=Count]'),
                FinalPrice = block.find('[name=FinalPrice]');

            if (FinalPrice.length > 0 && !FinalPrice.val()) {
                FinalPrice.val(uniqueProduct.Price);
            }

            if (Count.length > 0 && !Count.val()) {
                Count.val(1);
            }
            get_price(Price);
        }
        if (FactoryTailoring.length > 0 && uniqueProduct.LastFactoryTailoring != undefined) {
            FactoryTailoring.val(uniqueProduct.LastFactoryTailoring);
        }
        if (TailoringCost.length > 0 && uniqueProduct.LastTailoringCost) {
            TailoringCost.val(uniqueProduct.LastTailoringCost);
        }
        if (FactoryCut.length > 0 && uniqueProduct.LastFactoryCut != undefined) {
            FactoryCut.val(uniqueProduct.LastFactoryCut);
        }
        if (CutCost.length > 0 && uniqueProduct.LastCutCost) {
            CutCost.val(uniqueProduct.LastCutCost);
        }
        if (SupplySpecificProductUnit.length > 0 && uniqueProduct.LastFactoryCut) {
            SupplySpecificProductUnit.val(uniqueProduct.LastFactoryCut);
        }
        if (Code.length > 0 && uniqueProduct.Code) {
            Code.val(uniqueProduct.Code);
        }
    });
}

function get_SpecificProduct_barcode(autocomplite, widthBarcode, heightBarcode) {
    let block = $(autocomplite).closest(".edit-section-body");

    let uniqueProductId = block.find('[name=UniqueProduct]').val();
    let productSizeId = block.find('[name=ProductSize]').val();

    let data = {
        UniqueProductId: uniqueProductId,
        ProductSizeId: productSizeId,
        WidthBarcode: widthBarcode,
        HeightBarcode: heightBarcode
    };

    if (!uniqueProductId || uniqueProductId === '0' || !productSizeId || productSizeId === '0')
        return;

    $.post(gRootUrl + "Product/GetSpecificProductBarcode", data, barcode => {
        let barcodeBlock = block.find('#barcode');

        barcodeBlock.attr('src', 'data:image/png;base64, ' + barcode);

        barcodeBlock.removeClass('unvisible');
    });
}

function set_UniqueProduct_size_options(dynamicRow, UniqueProductId) {
    $.post(gRootUrl + "Product/GetUniqueProductSizeOptions", { UniqueProductId: UniqueProductId }, options => dynamicRow.find('[name=ProductSize]').html(options));
}

function change_unique_model_show(switchSlayder) {
    let rows = $('#unique-model-container .edit-section-row'),
        rowsNotEnabled = $('#unique-model-container .edit-section-row.display-none');

    if (switchSlayder.checked) {
        rowsNotEnabled.each((i, row) => {
            row.classList.remove('display-none');
            row.style.background = '#aaa';
        });
    }
    else {
        rows.not('.enabled').each((i, row) => row.classList.add('display-none'));
    }

    console.log(rowsNotEnabled)
}

function unique_model_action(url, btn) {
    let Switch = $('#switch');

    if (Switch.length <= 0 || !Switch[0].checked) {
        location = url;
    }
    else {
        let row = $(btn).closest('.edit-section-row');

        let rowIsEnabled = row.hasClass('enabled')

        if (rowIsEnabled) {
            row.removeClass('enabled');
            row.css('background', '#aaa');
        }
        else {
            row.addClass('enabled');
            row.removeAttr('style');
        }

        let data = {
            UniqueProductId: row.find('[name=UniqueProductList]').val(),
            Enabled: Number(!rowIsEnabled)
        };

        $.post(gRootUrl + "Product/UpdateEnabledUniqueModel", data);
    }
}

function toggle_section_ProductPrice(pBlock) {
    if (!$('#ProductPrice_section .edit-section-body').html()) {
        $.post(gRootUrl + "Product/GetProductPrices", { ProductId: $('[name=Id]').val() }, view => $('#ProductPrice_section .edit-section-body').html(view));
    }

    if (!$(pBlock).closest(".edit-section").find(".edit-section-body").is(":visible")) {
        $(pBlock).closest(".edit-section").find(".edit-section-body").slideDown(300);
        $(pBlock).closest(".edit-section-header").addClass("edit-section-header-expanded");
    }
    else {
        $(pBlock).closest(".edit-section").find(".edit-section-body").slideUp(300);
        $(pBlock).closest(".edit-section-header").removeClass("edit-section-header-expanded");
    }
}

function open_return_popup(productForOrderNameSpace, productForOrderId) {
    if (gIsLocalOrder) {
        open_simple_popup_dynamic(productForOrderNameSpace, productForOrderId, 'ReturnProduct_LocalSales');
    }
    else {
        open_simple_popup_dynamic(productForOrderNameSpace, productForOrderId, 'ReturnProduct');
    }
}