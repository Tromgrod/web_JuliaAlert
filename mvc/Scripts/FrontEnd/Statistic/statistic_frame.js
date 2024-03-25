var frame = undefined;

var colors = [
    '#FFCC00',
    '#FF9933',
    '#CC3300',
    '#FF9999',
    '#CC6666',
    '#FF3366',
    '#FF3399',
    '#FF00CC',
    '#FF99FF',
    '#CC66CC',
    '#CC33FF',
    '#9933CC',
    '#9966FF',
    '#9999FF'
];

/*------------- frame -------------*/

function set_statistic_frame(frameData) {
    frame = new Chart('statisticFrame', {
        type: 'line',
        data: {
            labels: get_statisticDataLabels(),
            datasets: Array.from(frameData, (dataset, i) => set_dataset(dataset.Name, dataset.Data, i))
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

let nameIndexMatch = [];

function refresh_statistic_frame(frameData) {
    frame.data.labels = get_statisticDataLabels();

    frame.data.datasets.length = frameData.length;

    frameData.forEach((dataset, i) => {
        if (frame.data.datasets[i]) {
            let prevIndex = nameIndexMatch.find(val => val.Name == dataset.Name);

            prevIndex = prevIndex ? prevIndex.Index : i;

            if (nameIndexMatch.length == 0)
                nameIndexMatch.push({ Name: dataset.Name, Index: prevIndex });

            frame.data.datasets[i].label = dataset.Name;
            frame.data.datasets[i].data = dataset.Data;
            frame.data.datasets[i].borderColor = colors[prevIndex % colors.length];
            frame.data.datasets[i].backgroundColor = colors[prevIndex % colors.length] + '60';
        }
        else {
            let index = 0;
            while (nameIndexMatch.some(val => val.Index == index) && index <= colors.length) {
                index++;
            }

            frame.data.datasets[i] = set_dataset(dataset.Name, dataset.Data, index);
        }
    });

    nameIndexMatch = nameIndexMatch.filter(val => frameData.some(dataset => dataset.Name == val.Name));

    frame.update();
}

function get_statisticDataLabels() {
    let years = $('#years').val().map(year => parseInt(year)).sort();
    let monthFromIndex = parseInt($('#monthFrom').val()) - 1;
    let monthToIndex = parseInt($('#monthTo').val()) - 1;

    const months = [
        { name: 'Январь', days: 31 },
        { name: 'Февраль', days: 28 },
        { name: 'Март', days: 31 },
        { name: 'Апрель', days: 30 },
        { name: 'Май', days: 31 },
        { name: 'Июнь', days: 30 },
        { name: 'Июль', days: 31 },
        { name: 'Август', days: 31 },
        { name: 'Сентябрь', days: 30 },
        { name: 'Октябрь', days: 31 },
        { name: 'Ноябрь', days: 30 },
        { name: 'Декабрь', days: 31 }
    ];

    const statisticDataLabels = {};

    for (const year of years) {
        const yearData = [];
        for (const month of months) {
            var days = Array.from({ length: month.days }, (_, index) => index + 1 + ' ' + month.name);

            if (month.name == 'Февраль' && (year % 4 == 0))
                days.push(29);

            yearData[months.indexOf(month)] = days;
        }
        statisticDataLabels[year] = yearData;
    }

    let labels = [];
    let frameHeader = $('#dinamic-text');

    if (years.length == 1) {
        if (Number.isInteger(monthFromIndex) && Number.isInteger(monthToIndex)) {
            if (monthFromIndex == monthToIndex) {
                frameHeader.html(`${months[monthFromIndex].name.toLowerCase()} ${years[0]} года,`);
            }
            else {
                frameHeader.html(`${months[monthFromIndex].name.toLowerCase()} - ${months[monthToIndex].name.toLowerCase()} ${years[0]} года,`);
            }

            if (monthToIndex - monthFromIndex < 3) {
                var mergedArray = [];

                statisticDataLabels[years[0]].slice(monthFromIndex, monthToIndex + 1).forEach(array => mergedArray = mergedArray.concat(array));

                labels = mergedArray;
            }
            else {
                labels = months.slice(monthFromIndex, monthToIndex + 1).map(month => month.name);
            }
        }
        else if (Number.isInteger(monthFromIndex)) {
            frameHeader.html(`${months[monthFromIndex].name.toLowerCase()} ${years[0]} года,`);

            labels = statisticDataLabels[years[0]][monthFromIndex];
        }
        else {
            frameHeader.html(`${years[0]} год,`);

            labels = months.map(month => month.name);
        }
    }
    else if (years.length <= 3) {
        frameHeader.html(`[${years.join(", ")}] года,`);

        for (let i = 0; i < years.length; i++)
            labels = labels.concat(months.map((month, monthIndex) => month.name + ' ' + years[i]));
    }
    else {
        frameHeader.html(`${Math.min(...years)} - ${Math.max(...years)},`);

        labels = years;
    }

    return labels;
}

function set_dataset(label, data, index) {
    const dataset = {
        label: label,
        data: data,
        borderColor: colors[index % colors.length],
        backgroundColor: colors[index % colors.length] + '60',
        tension: .1
    };

    nameIndexMatch.push({ Name: label, Index: index });

    return dataset;
}

function years_event(yearsInput) {
    let years = $(yearsInput).val();
    let months = $('#months');

    if (years.length > 1) {
        months.fadeOut();

        months.find('#monthFrom').val(1);
        months.find('#monthTo').val(12);
    }
    else {
        months.fadeIn();
    }

    get_statisticDataLabels();
    search_statistic_frame();
}

let oldMonthFrom;
let oldMonthTo;

function months_event() {
    let monthFrom = $('#monthFrom'),
        monthTo = $('#monthTo');

    let monthFromVal = parseInt(monthFrom.val()),
        monthToVal = parseInt(monthTo.val());

    if (monthFromVal > monthToVal) {
        if (monthFromVal != oldMonthFrom) {
            monthTo.val(monthFromVal);
        }
        else {
            monthFrom.val(monthToVal);
            monthTo.val(monthFromVal);
        }
    }

    oldMonthFrom = monthFromVal;
    oldMonthTo = monthToVal;

    get_statisticDataLabels();
    search_statistic_frame();
}

let dynamicFilterArray = [];

function statisticGroups_event(statisticGroupVal, dynamicFilterValues = '', firstRun = false) {
    statisticGroupVal = parseInt(statisticGroupVal);

    let dynamicFilter = $('#dynamic-filter');

    if (statisticGroupVal === 1) {
        dynamicFilter.fadeOut(300);

        dynamicFilter.find('select').val('');
        dynamicFilter.find('select').html('');

        $('#uniqueProducts').fadeIn(300);
        $('#salesChannels').fadeIn(300);
        $('#typeProducts').fadeIn(300);
        $('#countries').fadeIn(300);
    }
    else {
        dynamicFilter.fadeIn(300);

        let select = dynamicFilter.find('select');
        dynamicFilterArray = [];

        var options;

        if (statisticGroupVal == 2) {
            dynamicFilter.find('label').html('Модели:');
            options = $('#uniqueProducts select').html();

            $('#uniqueProducts').fadeOut(300);
            $('#salesChannels').fadeIn(300);
            $('#typeProducts').fadeOut(300);
            $('#countries').fadeIn(300);
        }
        else if (statisticGroupVal == 3) {
            dynamicFilter.find('label').html('Каналы продаж:');
            options = $('#salesChannels select').html();

            $('#uniqueProducts').fadeIn(300);
            $('#salesChannels').fadeOut(300);
            $('#typeProducts').fadeIn(300);
            $('#countries').fadeIn(300);
        }
        else if (statisticGroupVal == 4) {
            dynamicFilter.find('label').html('Типы моделей:');
            options = $('#typeProducts select').html();

            $('#uniqueProducts').fadeOut(300);
            $('#salesChannels').fadeIn(300);
            $('#typeProducts').fadeOut(300);
            $('#countries').fadeIn(300);
        }
        else if (statisticGroupVal == 5) {
            dynamicFilter.find('label').html('Страны:');
            options = $('#countries select').html();

            $('#uniqueProducts').fadeIn(300);
            $('#salesChannels').fadeIn(300);
            $('#typeProducts').fadeIn(300);
            $('#countries').fadeOut(300);
        }

        select.html(options);

        if (dynamicFilterValues) {
            const values = dynamicFilterValues.split(',');
            select.val(values);

            dynamicFilterArray = values;
        }
        else {
            let firstVal = select.find('option').first().attr('value');
            select.val(firstVal);

            dynamicFilterArray.push(firstVal);
        }

        select.select2({
            closeOnSelect: false
        });

        select.on("select2:selecting", event => {
            if (dynamicFilterArray.every(val => val != event.params.args.data.id)) {
                dynamicFilterArray.push(event.params.args.data.id);
            }
        });
        select.on("select2:unselecting", event => dynamicFilterArray = dynamicFilterArray.filter(val => val != event.params.args.data.id));
    }

    if (!firstRun) {
        $('#countries select').select2('destroy').val("").select2();
        $('#typeProducts select').select2('destroy').val("").select2();
        $('#salesChannels select').select2('destroy').val("").select2();
        $('#uniqueProducts select').select2('destroy').val("").select2();

        nameIndexMatch = [];
        search_statistic_frame();
    }
}

let request;

function search_statistic_frame() {
    const postData = {
        Years: $('#years').val().join(),
        MonthFrom: $('#monthFrom').val(),
        MonthTo: $('#monthTo').val(),
        GroupBy: $('#statisticGroup').val(),
        DynamicFilter: dynamicFilterArray.join(),
        CountingType: $('#countingType').val(),
        UniqueProducts: $('#uniqueProducts select').val()?.join(),
        SalesChannels: $('#salesChannels select').val()?.join(),
        TypeProducts: $('#typeProducts select').val()?.join(),
        Countries: $('#countries select').val()?.join()
    };

    if (request)
        request.abort();

    if (!request || request.state() == 'resolved')
        $('#loader-statisticFrame').fadeIn(150);

    request = $.post(gRootUrl + 'Statistic/Search', postData, statisticData => {
        $('#loader-statisticFrame').fadeOut(150);

        if (frame)
            refresh_statistic_frame(statisticData.frameData);
        else
            set_statistic_frame(statisticData.frameData);

        if (salesTypeChart)
            refresh_statistic_salesTypeChart(statisticData.salesTypeChartData);
        else
            set_statistic_salesTypeChart(statisticData.salesTypeChartData);

        if (clientBarChart)
            refresh_statistic_topClientBar(statisticData.clientsBarData);
        else
            set_statistic_topClientBar(statisticData.clientsBarData);

        if (productBarChart)
            refresh_statistic_topProductBar(statisticData.productsBarData);
        else
            set_statistic_topProductBar(statisticData.productsBarData);

        if (typeProductBarChart)
            refresh_statistic_topTypeProductBar(statisticData.typeProductsBarData);
        else
            set_statistic_topTypeProductBar(statisticData.typeProductsBarData);

        if (countryBarChart)
            refresh_statistic_topCountryBar(statisticData.countriesBarData);
        else
            set_statistic_topCountryBar(statisticData.countriesBarData);

        if (salesChannelBarChart)
            refresh_statistic_topSalesChannelBar(statisticData.salesChannelsBarData);
        else
            set_statistic_topSalesChannelBar(statisticData.salesChannelsBarData);
    });

    request.fail(error => {
        $('#loader-statisticFrame').fadeOut(150);
        console.log(error);
    });
}

/*------------- pie chart salesType -------------*/

var salesTypeChart;
const salesTypeChartSettings = {
    labels: ['Продажи', 'Возврат'],
    datasets: [{
        backgroundColor: ['rgba(141, 255, 172, 0.4)', 'rgba(255, 99, 132, 0.4)'],
        borderColor: ['rgba(141, 255, 172, 0.6)','rgba(255, 99, 132, 0.6)'],
        hoverOffset: 4
    }]
};

function set_statistic_salesTypeChart(salesTypeChartData) {
    $('#totalCount').html('Типы продаж, всего: ' + (salesTypeChartData[0] + salesTypeChartData[1]));

    salesTypeChartSettings.datasets[0].data = salesTypeChartData;

    salesTypeChart = new Chart('pie-chart-type-sales', {
        type: 'pie',
        data: salesTypeChartSettings,
        options: {
            onHover: (evt, activeEls) => activeEls.length > 0 ? evt.chart.canvas.style.cursor = 'pointer' : evt.chart.canvas.style.cursor = 'default',
            layout: {
                padding: {
                    bottom: 5,
                    top: 10
                }
            },
            plugins: {
                legend: {
                    labels: {
                        color: '#000000BB',
                        font: {
                            size: 14,
                            family: 'Arial',
                            weight: '600'
                        },
                        padding: 8
                    }
                }
            },
            animation: {
                duration: 750
            }
        }
    });
}

function refresh_statistic_salesTypeChart(salesTypeChartData) {
    $('#totalCount').html('Типы продаж, всего: ' + (salesTypeChartData[0] + salesTypeChartData[1]));

    salesTypeChart.data.datasets[0].data = salesTypeChartData;
    salesTypeChart.update();
}

/*------------- bar chart -------------*/
const allDataset = {
    label: 'Все продажи',
    backgroundColor: ['rgba(255, 204, 0, 0.4)'],
    borderColor: ['rgb(255, 204, 0)'],
    borderWidth: 1
};

const salesDataset = {
    label: 'Продажи',
    backgroundColor: ['rgba(141, 255, 172, 0.4)'],
    borderColor: ['rgb(141, 255, 172)'],
    borderWidth: 1
};

const returnsDataset = {
    label: 'Возврат',
    backgroundColor: ['rgba(255, 99, 132, 0.4)'],
    borderColor: ['rgb(255, 99, 132)'],
    borderWidth: 1
};

const barChartConfig = {
    type: 'bar',
    data: {
        datasets: [salesDataset, returnsDataset]
    },
    options: {
        scales: {
            y: {
                beginAtZero: true
            }
        },
        plugins: {
            legend: {
                labels: {
                    color: '#000000BB',
                    font: {
                        size: 14,
                        family: 'Arial',
                        weight: '600'
                    },
                    padding: 8
                }
            }
        }
    }
};

function update_bar_data(chart, barData) {
    let labels = Array.from(barData, barElem => barElem.Name);
    let sales = Array.from(barData, barElem => barElem.Sales);
    let returns = Array.from(barData, barElem => barElem.Return);

    let salesDatasetLocal = JSON.parse(JSON.stringify(salesDataset));
    let returnsDatasetLocal = JSON.parse(JSON.stringify(returnsDataset));
    let allDatasetLocal = JSON.parse(JSON.stringify(allDataset));

    chart.data.labels = labels;

    if ($('#countingType').val() == 1 || $('#countingType').val() == 2) {
        if (Math.max(...sales) == 0 && Math.max(...returns) == 0) {
            chart.data.datasets.length = 0;
        }
        else if (Math.max(...sales) == 0) {
            chart.data.datasets.length = 1;

            if (chart.data.datasets[0].label == returnsDatasetLocal.label) {
                chart.data.datasets[0].data = returns;
            }
            else {
                returnsDatasetLocal.data = returns;
                chart.data.datasets[0] = returnsDatasetLocal;
            }
        }
        else if (Math.max(...returns) == 0) {
            chart.data.datasets.length = 1;

            if (chart.data.datasets[0].label == salesDatasetLocal.label) {
                chart.data.datasets[0].data = sales;
            }
            else {
                salesDatasetLocal.data = sales;
                chart.data.datasets[0] = salesDatasetLocal;
            }
        }
        else {
            if (chart.data.datasets[0] && chart.data.datasets[1]) {
                chart.data.datasets[0].data = sales;
                chart.data.datasets[1].data = returns;
            }
            else {
                salesDatasetLocal.data = sales;
                chart.data.datasets[0] = salesDatasetLocal;

                returnsDatasetLocal.data = returns;
                chart.data.datasets[1] = returnsDatasetLocal;
            }
        }
    }
    else {
        chart.data.datasets.length = 1;

        if (chart.data.datasets[0].label == allDatasetLocal.label) {
            chart.data.datasets[0].data = sales;
        }
        else {
            allDatasetLocal.data = sales;
            chart.data.datasets[0] = allDatasetLocal;
        }
    }

    return chart;
}

/*------------- bar chart topClient -------------*/
var clientBarChart;

function set_statistic_topClientBar(barData) {
    let barChartConfigLocal = JSON.parse(JSON.stringify(barChartConfig));
    update_bar_data(barChartConfigLocal, barData);

    clientBarChart = new Chart('bar-chart-top-client', barChartConfigLocal);
}

function refresh_statistic_topClientBar(barData) {
    update_bar_data(clientBarChart, barData);

    clientBarChart.update();
}

/*------------- bar chart topProduct -------------*/
var productBarChart;

function set_statistic_topProductBar(barData) {
    let barChartConfigLocal = JSON.parse(JSON.stringify(barChartConfig));
    update_bar_data(barChartConfigLocal, barData);

    productBarChart = new Chart('bar-chart-top-product', barChartConfigLocal);
}

function refresh_statistic_topProductBar(barData) {
    update_bar_data(productBarChart, barData);

    productBarChart.update();
}

/*------------- bar chart topTypeProduct -------------*/
var typeProductBarChart;

function set_statistic_topTypeProductBar(barData) {
    let barChartConfigLocal = JSON.parse(JSON.stringify(barChartConfig));
    update_bar_data(barChartConfigLocal, barData);

    typeProductBarChart = new Chart('bar-chart-top-type-product', barChartConfigLocal);
}

function refresh_statistic_topTypeProductBar(barData) {
    update_bar_data(typeProductBarChart, barData);

    typeProductBarChart.update();
}

/*------------- bar chart topCountry -------------*/
var countryBarChart;

function set_statistic_topCountryBar(barData) {
    let barChartConfigLocal = JSON.parse(JSON.stringify(barChartConfig));
    update_bar_data(barChartConfigLocal, barData);

    countryBarChart = new Chart('bar-chart-top-country', barChartConfigLocal);
}

function refresh_statistic_topCountryBar(barData) {
    update_bar_data(countryBarChart, barData);

    countryBarChart.update();
}

/*------------- bar chart topSalesChannel -------------*/
var salesChannelBarChart;

function set_statistic_topSalesChannelBar(barData) {
    let barChartConfigLocal = JSON.parse(JSON.stringify(barChartConfig));
    update_bar_data(barChartConfigLocal, barData);

    salesChannelBarChart = new Chart('bar-chart-top-sales-channel', barChartConfigLocal);
}

function refresh_statistic_topSalesChannelBar(barData) {
    update_bar_data(salesChannelBarChart, barData);

    salesChannelBarChart.update();
}