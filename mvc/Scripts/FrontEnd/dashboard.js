document.addEventListener('DOMContentLoaded', function () {
    if (Notification.permission !== "granted")
        Notification.requestPermission();

    init_dashboard();
});

function init_dashboard() {
    let icons = $(".dashboard-icon"),
        buttonIcons = $(".dashboard-icon-title.button"),
        dashboardContainer = $('.dashboard-container');

    if (icons.length > 0) {
        icons.each((i, elem) => {
            elem.addEventListener("mouseenter", () => buttonIcons[i].classList.add("button-hover"), false);
            elem.addEventListener("mouseleave", () => buttonIcons[i].classList.remove("button-hover"), false);
        });
    }

    if (dashboardContainer.length > 0) {
        let dataGridContainer = dashboardContainer.find('.data-grid-container');

        dataGridContainer.css('height', 0);
        dataGridContainer.css('height', $('.data-grid').height());
    }

    set_gradation();
}

function set_gradation() {
    let maxIntenseColor = 255 * 2,
        maxIntenseColorPercent = maxIntenseColor / 100;

    $('[gradation]').each((i, gradationElem) => {
        gradationElem = $(gradationElem);

        let gradationMin = gradationElem.attr('gradation-min'),
            gradationMax = gradationElem.attr('gradation-max'),
            gradationPercent = Math.abs(gradationMax - gradationMin) / 100;

        gradationElem.find('[gradation-val]').each((i, valueBlock) => {
            let gradationVal = Number($(valueBlock).attr('gradation-val'));

            let intenseColor = maxIntenseColorPercent * (gradationVal - gradationMin) / gradationPercent;

            let red = maxIntenseColor - intenseColor,
                green = intenseColor;

            gradationElem.css('background-color', 'rgb(' + red + ', ' + green + ', 0, 0.5)');
        });
    });
}

function chart_click(id) {
    let typeLink = Number($('.chart').attr('link-type'));

    if (typeLink == 1) {
        window.open(gRootUrl + 'Report/OrderList/OrderState/JuliaAlert.Models.Objects/' + id);
    }
    if (typeLink == 2) {
        window.open(gRootUrl + 'Report/OrderList_LocalSales/OrderState/JuliaAlert.Models.Objects/' + id);
    }
}

function change_dashboard_item(widgetBlock, dashboardItemIndex) {
    widgetBlock = $(widgetBlock);

    if (widgetBlock.hasClass('widget-caption-active'))
        return;

    let dataGridContainer = $(".data-grid-container"),
        chartBlock = $('.chart');

    $('.widget-caption').removeClass('widget-caption-active');
    widgetBlock.addClass('widget-caption-active');

    dataGridContainer.css('height', 0);

    if (chartBlock.length > 0)
        chartBlock.addClass('unvisible');

    let timeDuration = new Date().getTime() + 400;

    $.post(gRootUrl + "DashBoard/ChangeDashboardItem", { DashboardItemIndex: dashboardItemIndex }, dashboardItem => {
        setTimeout(() => {
            let dataGrid = $('.dashboard-list .data-grid');

            dataGrid.html('');
            dataGrid.html(dashboardItem.ListView);

            set_gradation();
            dataGridContainer.css('height', $('.data-grid').height());

            if (dashboardItem.ChartData) {
                set_dashboard_chart(dashboardItem.ChartData);
                chartBlock.removeClass('unvisible');
                chartBlock.attr('link-type', dashboardItem.ChartLinkType);
            }
            else if (frame) {
                frame.destroy();
            }

        }, timeDuration - new Date().getTime());
    });
}

function change_dashboard(breadcrumbBlock, dashboardIndex) {
    let breadcrumbs = $('.header-breadcrumbs > .button');

    if (!$(breadcrumbBlock).hasClass("hover-button")) {
        let timeDuration = new Date().getTime() + 250;

        let content = $('.content');
        content.css('opacity', 0);

        $.post(gRootUrl + "DashBoard/ChangeDashboard", { DashboardIndex: dashboardIndex }, dashboard => {
            setTimeout(() => {
                content.html(dashboard);
                init_dashboard();

                breadcrumbs.removeClass('hover-button');

                breadcrumbs.each((i, breadcrumb) => {
                    breadcrumb = $(breadcrumb);
                    if (Number(breadcrumb.attr('value')) === dashboardIndex)
                        breadcrumb.addClass('hover-button');
                })

                content.css('opacity', 1);
            }, timeDuration - new Date().getTime());
        });
    }
}

function set_hover_on_breadcrumb(breadcrumbId) {
    $('.header-breadcrumbs [value=' + breadcrumbId + ']').addClass('hover-button');
}

var frame = undefined;

function set_dashboard_chart(chartData) {
    ids = Array.from(chartData, itemData => itemData.Id);

    if (frame)
        frame.destroy();

    frame = new Chart('dashboardPieChart', {
        type: 'doughnut',
        data: {
            labels: Array.from(chartData, itemData => itemData.Name),
            datasets: [{
                data: Array.from(chartData, itemData => itemData.Count),
                backgroundColor: Array.from(chartData, itemData => itemData.Color),
                hoverOffset: 4
            }]
        },
        options: {
            onClick: (event, array) => {
                if (array[0] && ids && ids[array[0].index])
                    chart_click(ids[array[0].index]);
            },
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