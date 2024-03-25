window.onload = function () {
    let icons = $(".statistic-select-block .image-block");

    if (icons.length > 0) {
        icons.each((i, elem) => {
            elem.addEventListener("mouseenter", () => $(elem).closest('.statistic-select-block').find('.button').addClass("button-hover"), false);
            elem.addEventListener("mouseleave", () => $(elem).closest('.statistic-select-block').find('.button').removeClass("button-hover"), false);
        });
    }
}