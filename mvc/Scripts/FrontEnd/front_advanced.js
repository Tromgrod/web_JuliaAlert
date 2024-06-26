var gHtmlPopupControl = null;
var gCkEditorConfig;

$(document).ready(function () {
    set_calendar();
    set_calendar_flatpickr();

    if ($(".edit-section-table-row").size() == 1)
        $(".edit-section-table-add").css("display", "table-row");

    $(".autocomplete-input").each((i, item) => load_autocomplete(item));

    if ($(".control-multyselect").size() > 0) {
        $.each($(".control-multyselect select"), function (i, item) {
            if ($.trim($(item).html()) == "") {
                gpostArray = {
                    values: $(item).attr("data-values")
                };

                $.post(gRootUrl + "MultySelect/" + $(item).attr("data-namespace"), gpostArray, responce => {
                    $(item).html(responce);
                    $(item).select2();
                })
            }
            else
                $(item).select2();
        });
    }

    $(".select-multyselect").select2({
        closeOnSelect: false
    });

    init_htmlpopup_link();

    update_minimu_page_height();

    if ($('.footer').length > 0) {
        if ($('.page-controls').length > 0)
            $('.page-controls').css('margin-bottom', '50px');
        else if ($('.content').length > 0)
            $('.content').css('margin-bottom', '50px');
    }
});

function set_calendar_flatpickr() {
    let calendarInputs = $(".calendar-flatpickr-input");

    if (calendarInputs.length > 0)
        calendarInputs.flatpickr();
}

function set_calendar(containerHtml = document, calendarOptions = {}) {
    let calendarInputs = $(containerHtml).find(".calendar-input");

    calendarInputs.each((i, elem) => {
        if ($(elem).hasClass('without-years')) {
            calendarOptions['format'] = 'dd/mm';
        }
        else if ($(elem).hasClass('years-only')) {
            calendarOptions['format'] = 'yyyy';
        }
        else if ($(elem).hasClass('months-only')) {
            calendarOptions['format'] = 'mm/yyyy';
        }
        else {
            calendarOptions['format'] = 'dd/mm/yyyy';
        }

        $(elem).datepicker(calendarOptions);

        $(elem).click(event => {
            if ($(event.target).hasClass('without-years')) {
                $('.datepicker').addClass('without-years');
            }
            else {
                $('.datepicker').removeClass('without-years');
            }
        })
    });

    let dateRangeInputs = $(containerHtml).find('.control-DateRange .calendar-input');

    dateRangeInputs.on('change', event => {
        let controlDateRange = $(event.target).closest('.control-DateRange');
        let currentYear = new Date().getFullYear();

        let fromDateInput = controlDateRange.find('.from'),
            toDateInput = controlDateRange.find('.to');

        let fromDateArray = fromDateInput.val().split('/'),
            toDateArray = toDateInput.val().split('/');

        let fromDay = Number(fromDateArray[0]),
            fromMonth = fromDateArray[1] ? Number(fromDateArray[1]) : 1,
            fromYear = fromDateArray[2] ? Number(fromDateArray[2]) : currentYear;

        let fromDate = new Date(fromYear, fromMonth - 1, fromDay);
        let toDate = undefined;

        if (toDateArray[0]) {
            let toDay = Number(toDateArray[0]),
                toMonth = toDateArray[1] ? Number(toDateArray[1]) : 1,
                toYear = toDateArray[2] ? Number(toDateArray[2]) : currentYear;

            toDate = new Date(toYear, toMonth - 1, toDay);
        }

        if (toDate && fromDate > toDate) {
            fromDateInput.datepicker('setDate', toDate);
            toDateInput.datepicker('setDate', fromDate);
        }
    })
}

function update_minimu_page_height() {
    $(".confirmation-overlay").height($("body").height());
}

$(window).resize(function () {
    update_minimu_page_height();
});

/*----------------------------------------------------:dynamic_section-----------------------------------------*/
function toggle_dynamic_section(pBlock, pNamespace, parentId) {
    let sectionHeader = $(pBlock).closest('.edit-section-header');

    if (!sectionHeader.hasClass("edit-section-header-expanded")) {
        load_dynamic_section(pBlock, pNamespace, parentId);
    }
    else {
        sectionHeader.removeClass("edit-section-header-expanded");
        $(pBlock).closest('.dynamic-section').find('.edit-section-body-samples').height(0);
    }
}
function load_dynamic_section(pBlock, pNamespace, parentId) {
    let header = $(pBlock).closest('.edit-section-header'),
        body = $(pBlock).closest('.dynamic-section').find('.edit-section-body-samples');

    header.addClass("edit-section-header-expanded");

    let data = {
        Namespace: pNamespace,
        ParentId: parentId && parentId > 0 ? parentId : $("[name=Id]").val()
    };

    $.post(gRootUrl + "DynamicControl/Load/", data, responce => {
        body.html(responce);
        body.height(body.find('.edit-section-table').height());

        if (body.find(".edit-section-table-row").size() == 1) {
            add_new_dynamic_section(pBlock, pNamespace);
        }
    });
}

function add_new_dynamic_section(pBlock, pNamespace) {
    let header = $(pBlock).closest('.edit-section-header'),
        body = $(pBlock).closest('.dynamic-section').find('.edit-section-body-samples');

    if (!header.hasClass("edit-section-header-expanded")) {
        toggle_dynamic_section(pBlock, pNamespace);
    }
    else {
        body.find(".edit-section-table-add").removeClass('edit-section-table-add');
        body.height(body.find('.edit-section-table').height());
    }
}

function add_new_dynamic_section_hard(pBlock) {
    let body = $(pBlock).closest('.dynamic-section').find('.edit-section-body-samples');

    body.find(".edit-section-table-add").removeClass('edit-section-table-add');
    body.height(body.find('.edit-section-table').height());
}

function save_all_dynamic_section(pBtn, parentId, Namespace) {
    if ($('.dynamic-section').find('.edit-section-header-expanded').length === 0) {
        ShowMessage('Секция не открыта', "error", true);
        return;
    }

    if ($(".loading").is(":visible"))
        return;

    let newRow = $(pBtn).closest('.dynamic-section').find(".edit-section-table-add");

    if ((newRow.attr('style') !== undefined && form_validation(newRow, false)) || newRow.attr('style') === undefined) {
        $(".loading").show();

        gpostArray = {
            Namespace: Namespace,
            ParentId: parentId
        };

        let rows = $(pBtn).closest('.dynamic-section').find('.edit-section-table-row');

        if (newRow.attr('style') === undefined)
            rows = rows.not('.edit-section-table-add');

        rows.each((indexRow, row) => $(row).find('input, select, textarea').each((i, input) => {
            if (gpostArray[input.name] === undefined)
                gpostArray[input.name] = '';

            gpostArray[input.name] += input.value + (indexRow != (rows.length - 1) ? ', ' : '');
        }));

        $.post(gRootUrl + "DynamicControl/AllSave/", gpostArray, responce => {
            $(pBtn).closest('.dynamic-section').find('.edit-section-body').html(responce);
            $(".loading").hide();
        });
    }
}

function save_Dynamic(pBtn, parentId, Namespace) {
    if (!$(".loading").is(":visible")) {
        if (form_validation($(pBtn).closest(".edit-section-table-row"), false)) {
            let body = $(pBtn).closest('.dynamic-section').find('.edit-section-body-samples');

            $(".loading").show();

            gpostArray = {
                Namespace: Namespace,
                ParentId: parentId
            };

            let dynamicRow = $(pBtn).closest(".edit-section-table-row");

            dynamicRow.find('input, select, textarea').not('[type=checkbox]').each((i, elem) => gpostArray[elem.name] = elem.value);
            dynamicRow.find('[type=checkbox]').each((i, checkbox) => gpostArray[checkbox.name] = Number(checkbox.checked));

            $.post(gRootUrl + 'DynamicControl/Save', gpostArray, responce => {
                $(pBtn).closest(".edit-section-body").html(responce);
                body.height(body.find('.edit-section-table').height());
                $(".loading").hide();
            });
        }
    }
}

function delete_Dynamic(pBlock, pId, pNamespace, doneFunction) {
    let postArray = {
        Id: pId,
        Namespace: pNamespace
    }
    remove_item(postArray, "Удалить запись?", pBlock, doneFunction);
}

function remove_item(postArray, message, pBlock, doneFunction) {
    ShowConfirmMessage(message, () => {
        postAjax = $.post(gRootUrl + "DynamicControl/Delete/", postArray, () => {
            let body = $(pBlock).closest('.dynamic-section').find('.edit-section-body-samples');

            $(pBlock).closest(".edit-section-table-row").remove();

            body.height(body.find('.edit-section-table').height());

            if (doneFunction)
                doneFunction();
        });
        return true;
    });
}

function check_number_input(input) {
    input.value = input.value.replace(/^\.|[^\d\.]|\.(?=.*\.)|^0+(?=\d)/g, '')
}