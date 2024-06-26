var popup;

document.addEventListener('mousemove', function (event) {
    let popupWidth = popup.width();
    let mapWidth = $('#map').width();
    let maxWidth = mapWidth - popupWidth;

    popup.css('left', Math.min(event.clientX, maxWidth - 40) + 10) + 'px';
    popup.css('top', (event.clientY + 15) + 'px');
});

window.onload = function () {
    $('#map-filter').on('mousemove', _ => {
        popup.css('display', 'none');
        $('#map').css('cursor', '');
    });

    popup = $('.popup-map');

    $('.ajax-loading-overlay').fadeIn(0);

    const mapCoordinateX = Number(localStorage.getItem('map-coordinate-X') ?? 40);
    const mapCoordinateY = Number(localStorage.getItem('map-coordinate-Y') ?? 20);
    const mapZoom = Number(localStorage.getItem('map-zoom') ?? 3);

    $.post(gRootUrl + 'Statistic/MapData', mapData => {
        $('.ajax-loading-overlay').fadeOut(300);

        var salesArray = Array.from(mapData, mapItem => mapItem.Sales);
        let maxSales = Math.max(...salesArray);

        $('.map-legend-text').html('0 - ' + maxSales);

        var geojsonSource = new ol.source.Vector({
            url: gRootUrl + 'Scripts/Bootstrap_Plugins/open-layers/world_countries.geo.json',
            format: new ol.format.GeoJSON()
        });

        var styleFunction = function (feature) {
            let sales = mapData.find(mapItem => mapItem.Name.toLowerCase() == feature.get('admin').toLowerCase())?.Sales,
                color = set_map_gradation(maxSales, sales);

            return new ol.style.Style({
                fill: new ol.style.Fill({
                    color: color ? color + ', 0.5)' : 'rgba(255, 255, 255, 0.5)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(150, 150, 150, 0.8)'
                })
            });
        };

        var map = new ol.Map({
            target: 'map',
            layers: [new ol.layer.Vector({
                source: geojsonSource,
                style: styleFunction
            })],
            view: new ol.View({
                center: ol.proj.fromLonLat([mapCoordinateX, mapCoordinateY]),
                zoom: mapZoom,
                maxZoom: 6
            })
        });

        const featureOverlay = new ol.layer.Vector({
            source: new ol.source.Vector(),
            map: map,
            style: {
                'stroke-color': 'rgba(255, 255, 255, 0.9)',
                'stroke-width': 2
            }
        });

        let highlight;
        const displayFeatureInfo = function (feature) {
            if (feature !== highlight) {
                if (highlight)
                    featureOverlay.getSource().removeFeature(highlight);
                if (feature)
                    featureOverlay.getSource().addFeature(feature);

                highlight = feature;
            }
        };

        map.on('moveend', _ => {
            const newZoom = map.getView().getZoom();

            const center = map.getView().getCenter();
            const coordinate = ol.proj.toLonLat(ol.extent.getBottomLeft(center));

            localStorage.setItem('map-zoom', newZoom);
            localStorage.setItem('map-coordinate-X', coordinate[0]);
            localStorage.setItem('map-coordinate-Y', coordinate[1]);
        });

        map.on('pointermove', function (evt) {
            if (evt.dragging)
                return;

            const pixel = map.getEventPixel(evt.originalEvent);
            const feature = map.forEachFeatureAtPixel(pixel, function (feature) {
                return feature;
            });

            displayFeatureInfo(feature);

            if (feature) {
                var name = feature.get('admin');
                var sales = mapData.find(mapItem => mapItem.Name.toLowerCase() == feature.get('admin').toLowerCase())?.Sales;

                var content = '<p>' + name + (sales ? ' (' + sales + ' шт)' : '') + '</p>';
                popup.html(content);

                popup.css('display', 'block');
                $('#map').css('cursor', 'pointer');
            } else {
                popup.css('display', 'none');
                $('#map').css('cursor', '')
            }
        });

        map.on('click', function (event) {
            map.forEachFeatureAtPixel(event.pixel, function (feature) {
                var mapItem = mapData.find(mapItem => mapItem.Name.toLowerCase() == feature.get('admin').toLowerCase());

                if (mapItem) {
                    //window.open(gRootUrl + 'Report/CountryStatisticList/Countries/JuliaAlert.Models.Objects/' + mapItem.id);
                }
            });
        });
    });
};

function set_map_gradation(gradationMax, value) {
    let maxIntenseColor = 255,
        maxIntenseColorPercent = maxIntenseColor / 100,
        valuePercent = gradationMax / 100;

    let intenseColor = maxIntenseColorPercent * value / valuePercent;

    let invertIntenseColor = maxIntenseColor - intenseColor;

    return value ? 'rgba(' + invertIntenseColor + ', 255, ' + invertIntenseColor : undefined;
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

    search_statistic_map();
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

    search_statistic_map();
}

function search_statistic_map() {

}