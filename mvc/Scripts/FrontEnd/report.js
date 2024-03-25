var searchReportReady = true;
var searchFocusObj = false;

$(document).ready(() => {
    if ($(".search-controls-subbmit").length > 0) {
        $(document).on('keypress', e => {
            if (e.which == 13 && searchReportReady)
                SearchReport();
        });
    }
})

function SearchReport() {
    $("#page_num").val(0);
    $("#sSearch").val("");
    $("#dpdwn_count_per_page").val($('#count_per_page').val());

    return doSearchSubbmit("");
}

function ResetReport() {
    $(".search-data-grid input, .search-data-grid select").not('[data-req=1], .hidden-autocomplete-input.validation').val("");
    $('.search-data-grid [type=checkbox]').prop('checked', false);
    SearchReport();
}

function SortReport(pCol, propName) {
    $("#page_num").val(0);
    $("#sort_col").val(propName);
    $("#sort_dir").val($(pCol).find('.data-grid-title-control').hasClass("data-grid-title-control-sortable-asc") ? "desc" : "asc");
    doSearchSubbmit($("#sSearch").val());
}

function doSearchSubbmit(sSearch) {
    if (form_validation($(".search-data-grid-container"), true)) {
        if (!$(".loading").is(":visible")) {

            $('.ajax-loading-overlay').fadeIn("slow");

            gpostArray = {
                bo_type: $(".search-data-grid").attr("data-type"),
                __RequestVerificationToken: $("[name=__RequestVerificationToken]").val(),
                CountPerPage: $("#dpdwn_count_per_page").val() > 10 ? $("#dpdwn_count_per_page").val() : 10,
                PageNum: $("#page_num").val(),
                SortCol: $("#sort_col").val(),
                SortDir: $("#sort_dir").val(),
                sSearch: sSearch,
                ViewMode: $('[name=view-mode]:checked').val()
            };

            $(".search-data-grid").find(".control-edit").each(function (i) {
                let control = $(this).attr("data-control");
                eval("if (window." + control + "_on_after_update_function) " + control + "_on_after_update_function($(this),gpostArray);");
            });

            searchFocusObj = false;

            return $.post(gRootUrl + "Report/Search", gpostArray, responce => {
                if (responce.Result == 1) {
                    $('.ajax-loading-overlay').fadeOut();
                    $(".search-results").fadeOut(250, () => {
                        $(".search-results").html(responce.Data["Search_Result"]);
                        $(".search-results").fadeIn(250);
                    })
                }
                else if (responce.Result == 2)
                    window.location.reload();
            })
        }
    }
}

function doExportExcellReport() {
    if (!$(".ajax-loading-overlay").is(":visible")) {
        $('.ajax-loading-overlay').fadeIn("slow");

        gpostArray = {
            bo_type: $(".search-data-grid").attr("data-type"),
            __RequestVerificationToken: $("[name=__RequestVerificationToken]").val(),
            CountPerPage: $("#dpdwn_count_per_page").val(),
            PageNum: $("#page_num").val(),
            SortCol: $("#sort_col").val(),
            SortDir: $("#sort_dir").val(),
            sSearch: $("#sSearch").val()
        };

        $(".search-data-grid").find(".control-edit").each(function (i) {
            let control = $(this).attr("data-control");
            eval("if (window." + control + "_on_after_update_function) " + control + "_on_after_update_function($(this),gpostArray);");
        });

        $.fileDownload(gRootUrl + "Report/ExportExcell/", {
            httpMethod: "POST",
            data: gpostArray,
            successCallback: () => $('.ajax-loading-overlay').fadeOut(),
            failCallback: html => {
                $('.ajax-loading-overlay').fadeOut();
                ShowMessage(html, "error", true);
            }
        });
    }
}

function show_report_page(page) {
    $("#page_num").val(page);
    doSearchSubbmit($("#sSearch").val());
}

function count_per_page() {
    $("#page_num").val(0);
    doSearchSubbmit($("#sSearch").val());
}

function ShowQueryOptions(Model) {
    $.post(gRootUrl + "Report/Options/" + Model, message => {
        ShowConfirmMessage(message, _ => {
            $.post(gRootUrl + "Report/OptionsSave/" + Model, $("#printOptions").serialize(), data => {
                if (data.Result == 0)
                    ShowAlertMessage(data.Message);

                location.reload();
            });
            return true;
        }, _ => true, "Установить значения", "Закрыть");
    })
}

function coloring_table(RGB) {
    let countSale = document.querySelectorAll('.count_sale'),
        tmpMax = 0,
        arr = [],
        i = 0;

    countSale.forEach(item => {
        let num,
            textNum = item.textContent.trim();

        if (textNum != '-' && textNum != '0')
            num = Number(textNum);

        arr[i++] = num;
        if (num > tmpMax) {
            tmpMax = num;
            return tmpMax;
        }
        return arr;
    });

    for (let k = 0; k < arr.length; k++) {
        let num = arr[k] * 90 / tmpMax;
        countSale[k].style.backgroundColor = "rgba(" + RGB + ", " + (num + 10) + "%)";
    }
}

function update_calendar(typeCalendar, year, salesChannel = 0) {
    $('.ajax-loading-blur').show();
    $('table').css('filter', 'blur(10px)')

    postData = {
        Year: year,
        SalesChannel: salesChannel,
        TypeCalendar: typeCalendar
    };

    $.post(gRootUrl + "CustomReport/UpdateCalendar", postData, response => {
        $('.calendar-container').html(response);
        $('.ajax-loading-blur').hide();
        $('table').css('filter', 'blur(0)')
    });
}

function delete_client(Id, Namespace) {
    $.post(gRootUrl + 'Client/HasOrder', { ClientId: Id }, hasOrder => {
        if (hasOrder)
            DeleteItem(Id, Namespace, 'У клиента есть заказы, заказы с этим клиентом потеряют клиента, удалить клиента?');
        else
            DeleteItem(Id, Namespace);
    });
}

function save_popup_reload() {
    save_popup().done(() => window.location.reload());
}

function InventoryList_update() {
    $.post(gRootUrl + 'Production/UpdateInventoryList', _ => location.reload())
}

function InventoryList_change_CurrentCount(currentCountInput) {
    currentCountInput = $(currentCountInput);

    let reportRow = currentCountInput.closest('.data-grid-data-row');

    let specificProductIdBlock = reportRow.find('[name=SpecificProductId]'),
        inventoryIdBlock = reportRow.find('[name=InventoryId]'),
        stockIdBlock = reportRow.find('[name=StockId]'),
        differenceBlock = reportRow.find('.Difference');

    postData = {
        SpecificProductId: specificProductIdBlock.val(),
        CurrentCount: currentCountInput.val(),
        InventoryId: inventoryIdBlock.val(),
        StockId: stockIdBlock.val()
    };

    $.post(gRootUrl + 'Production/UpdateInventoryCurrentCount', postData, oldCurrentCount => {
        let differenceCount = Number(currentCountInput.val()) - Number(oldCurrentCount);

        differenceBlock.html(Number(differenceBlock.html()) + differenceCount);
        $('#colum-CurrentCount').html(Number($('#colum-CurrentCount').html()) + differenceCount);
        $('#colum-Difference').html(Number($('#colum-Difference').html()) + differenceCount);

        ShowMessage('Сохранено', "success", true, 300);
    });
}