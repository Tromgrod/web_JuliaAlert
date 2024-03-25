function stock_change(stock) {
    stock = $(stock);

    let countBlock = stock.closest('.edit-section-table-content').next().find('input');

    let dynamicRow = stock.closest(".edit-section-table-row");

    let data = {
        StockId: stock.val(),
        UniqueProductId: dynamicRow.find('[name=UniqueProduct]').val(),
        ProductSizeId: dynamicRow.find('[name=ProductSize]').val()
    };

    $.post(gRootUrl + "Stock/GetStockCurrentCount", data, count => countBlock.val(count));

    let stockFrom = dynamicRow.find('[name=StockFrom]'),
        stockTo = dynamicRow.find('[name=StockTo]');

    let stockFromOptions = stockFrom.find('option'),
        stockToOptions = stockTo.find('option');

    stockFromOptions.attr('hidden', '');
    stockToOptions.attr('hidden', '');

    stockFromOptions.each((i, stockFromOption) => {
        stockFromOption = $(stockFromOption);

        if (stockFromOption.val() != stockTo.val()) {
            stockFromOption.removeAttr('hidden');
        }
    });

    stockToOptions.each((i, stockToOption) => {
        stockToOption = $(stockToOption);

        if (stockToOption.val() != stockFrom.val()) {
            stockToOption.removeAttr('hidden');
        }
    });

    if (stockFrom.val() === stockTo.val()) {
        stock.val(0);
        stock_change(stock);
    }
}