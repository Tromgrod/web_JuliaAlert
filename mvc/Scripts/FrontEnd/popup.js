function standart_popup(data, action) {
    $.fancybox.open([
        {
            fitToView: true,
            autoSize: true,
            autoScale: true,
            openEffect: 'fade',
            closeEffect: 'fade',
            scrolling: false,
            padding: 0,
            closeBtn: true,
            helpers: {
                overlay: {
                    speedIn: 0,
                    speedOut: 300,
                    opacity: 0.8,
                    css: {
                        cursor: 'default'
                    },
                    closeClick: false
                }
            },
            type: 'ajax',
            ajax: {
                cache: false,
                type: "POST",
                data: data
            },
            href: gRootUrl + action
        }
    ]);
}

function open_simple_popup_dynamic(Namespace, Id, View) {
    let data = {
        Namespace: Namespace,
        ModelId: Id,
        ParentId: $('[name=Id]').val(),
        View: View
    };

    standart_popup(data, 'HtmlPopUp/Open');
}

function open_simple_popup_dynamic_customData(data, namespace, id, view) {
    data["Namespace"] = namespace;
    data["ModelId"] = id;
    data["ParentId"] = $('[name=Id]').val();
    data["View"] = view;

    standart_popup(data, 'HtmlPopUp/Open');
}

function open_report_popup(model, BOLink = undefined, NamespaceLink = undefined, Id = undefined) {
    let dateRangeFrom = $('.popup-search.search-from'),
        dateRangeTo = $('.popup-search.search-to');

    let data = {
        ModelName: model,
        BOLink: BOLink,
        NamespaceLink: NamespaceLink,
        Id: Id,
        DateRangeFrom: dateRangeFrom !== undefined ? dateRangeFrom.val() : undefined,
        DateRangeTo: dateRangeTo !== undefined ? dateRangeTo.val() : undefined
    };

    $(".search-data-grid").find("input, select, textarea").each((i, elem) => data[elem.name] = elem.value);

    standart_popup(data, 'Report/View_Min');
}

function open_simple_popup(View, Namespace, Id) {
    let data = {
        Namespace: Namespace === undefined ? $('[name=Namespace]').val() : Namespace,
        ModelId: Id === undefined ? $('[name=Id]').val() : Id,
        View: View
    };

    standart_popup(data, 'HtmlPopUp/Open');
}

function open_simple_popup_noData(View) {
    let data = {
        View: View
    };

    standart_popup(data, 'HtmlPopUp/OpenNoData');
}

function open_simple_popup_customData(data, View) {
    data["View"] = View;

    standart_popup(data, 'HtmlPopUp/OpenNoData');
}

function save_popup_reload_popup(parentNamespace, parentId, popupView, customData = {}) {
    save_popup().done(_ => open_simple_popup_dynamic_customData(customData, parentNamespace, parentId, popupView));
}

function save_popup(closePopUp = true, action = 'HtmlPopUp/Save', gpostArray = {}) {
    let popUp = $('.popup-content');

    if (form_validation(popUp, false)) {
        popUp.find('[name]input, [name]select, [name]textarea').not('[type=checkbox], .select-multyselect').each((i, elem) => gpostArray[elem.name] && elem.name && elem.value ? gpostArray[elem.name] += ',' + elem.value : gpostArray[elem.name] = elem.value);
        popUp.find('[type=checkbox]').each((i, checkbox) => gpostArray[checkbox.name] = Number(checkbox.checked));
        popUp.find('.select-multyselect').each((i, multyselect) => {
            optionsVal = '';
            $(multyselect).find(':selected').each((i, option) => optionsVal += $(option).val() + ',');
            gpostArray[multyselect.name] = optionsVal.slice(0, -1);
        });

        let postAjax = $.post(gRootUrl + action, gpostArray, response => {
            if (response.Result != 0) {
                ShowMessage(response.Message, 'success', true);
                if (closePopUp)
                    $.fancybox.close();
            }
            else
                ShowMessage(response.Message, 'error', true);
        });

        return postAjax;
    }
}

function delete_popup_reload_popup(parentNamespace, parentId, popupView, customData = {}) {
    delete_popup().done(_ => open_simple_popup_dynamic_customData(customData, parentNamespace, parentId, popupView));
}

function delete_popup(closePopUp = true, action = 'HtmlPopUp/Delete') {
    let popUp = $('.popup-content');

    gpostArray = {
        Namespace: popUp.find('[name=Namespace]').val(),
        Id: popUp.find('[name=Id]').val()
    };

    let postAjax = $.post(gRootUrl + action, gpostArray, response => {
        if (response.Result != 0) {
            ShowMessage(response.Message, 'success', true);
            if (closePopUp)
                $.fancybox.close();
        }
        else
            ShowMessage(response.Message, 'error', true);
    });

    return postAjax;
}