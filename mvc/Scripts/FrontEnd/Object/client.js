function client_data_output() {
    let clientId = $('[name=Client]').val();

    if (clientId > 0) {
        let data = {
            ClientId: clientId
        }

        let clientOrderInfoSection = $('#client_order_info');

        if (clientOrderInfoSection.length > 0) {
            clientOrderInfoSection.html('');
            $.post(gRootUrl + "Client/ClientOrders", data, view => clientOrderInfoSection.html(view))
        }

        $.post(gRootUrl + "Client/GetClitenData", data, Data => {
            let Client = Data.Client,
                Currency = Data.Currency;

            $('#client_name_ajax').html(Client.Name);

            $('[name=Index]').val(Client.Index);
            $('[name=Address]').val(Client.Address);
            $('[name=Phone]').val(Client.Phone);
            $('[name=Discount]').val(Client.Discount);
            $('[name=Email]').val(Client.Email);
            $('[name=Comment]').val(Client.Comment);
            $('[name=Birthday]').val(Client.BirthdayString);

            if (Client.Countries.Id > 0) {
                $('[name=Countries_autocomplete]').val(Client.Countries.Name + ' (' + Client.Countries.ShortName + ')');
                $('[name=Countries]').val(Client.Countries.Id);
            }

            if (Client.State.Id > 0) {
                $('[name=State_autocomplete]').val(Client.State.Name + ' (' + Client.State.ShortName + ')');
                $('[name=State]').val(Client.State.Id);
            }

            if (Client.City.Id > 0) {
                $('[name=City_autocomplete]').val(Client.City.Name);
                $('[name=City]').val(Client.City.Id);
            }

            let currency = $('#TransactionCurrency');

            if (currency.length > 0 && Currency != undefined) {
                currency.val(Currency.Name.trim());
            }
        });
    }
}

function clear_client_autocomplete(autocomplite) {
    clear_autocomplete(autocomplite);
    $('#client_name_ajax').html('');
    $('#client_order_info').html('');
    $(autocomplite).closest('.edit-section-body').find('input').val('');

    let currency = $('#TransactionCurrency');
    if (currency.length > 0)
        currency.val('');
}

function show_description(input) {
    input = $(input);

    let sectionRow = input.closest('.edit-section-row');

    sectionRow.toggleClass('not-vivible');
    input.toggleClass('visible-description');
}

$

function toggle_client_section(pBlock) {
    let clientInfoSection = $('#client_info'),
        clientOrderInfoSection = $('#client_order_info');

    let duration = 300;

    if (!clientInfoSection.is(":visible")) {
        clientOrderInfoSection.slideUp(duration);
        setTimeout(() => clientInfoSection.slideDown(duration), duration);
    }
    else {
        clientInfoSection.slideUp(duration);
        setTimeout(() => clientOrderInfoSection.slideDown(duration), duration);
    }

    $(pBlock).closest(".edit-section-header").toggleClass("edit-section-header-expanded");
}