var SystemLogConfig = {
    Init: function () {
        jQuery(document).on("click", ".jsonContain", function () {
            var obj = jQuery(this);
            var target = jQuery(obj.attr("data-target"));
            var renderer = jQuery(obj.attr("data-renderer"));
            var data = obj.text();
            var dataJson = JSON.parse(data);
            var options = {
                collapsed: false,
                rootCollapsable: true,
                withQuotes: false,
                withLinks: true
            };
            jQuery(renderer).jsonViewer(dataJson, options);
            $(target).modal({
                backdrop: 'static', //Click outside
                //keyboard: true, //Esc
                show: true
            });
        });
    },
    initChart: function () {
        let dict = JSON.parse($("#tableStatisticData").val());

        var colors = ["#dc3912", "#3366cc", "#ff9900", "#109618", "#990099", "#0099c6", "#dd4477", "#66aa00", "#b82e2e", "#316395", "#3366cc", "#994499", "#22aa99", "#aaaa11", "#6633cc", "#e67300", "#8b0707", "#651067", "#329262", "#5574a6", "#3b3eac", "#b77322", "#16d620", "#b91383", "#f4359e", "#9c5935", "#a9c413", "#2a778d", "#668d1c", "#bea413", "#0c5922", "#743411"];

        while (colors.length < dict.labels.length) {
            colors = colors.concat(colors);
        }

        var ctx = document.getElementById('archive_chart');
        const data = {
            labels: dict.labels,
            datasets: [{
                label: '',
                data: dict.data,
                backgroundColor: colors,
                hoverOffset: 4
            }]
        };
        var option = {
            //maintainAspectRatio: false,
            //responsive: true,
            legend: {
                display: false,
                position: "bottom",
            },
            ticks: {
                autoSkip: true,
                maxTicksLimit: 20
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            }
            //scales: {
            //    yAxes: [{
            //        ticks: {
            //            min: 0
            //        }
            //    }]
            //}
            //backgroundColor: '#2286ef'
        };
        new Chart(ctx, {
            type: 'bar',
            data: data,
            options: option
        });
    },
    getRandomColorHex: function () {
        var hex = "0123456789ABCDEF",
            color = "#";
        for (var i = 1; i <= 6; i++) {
            color += hex[Math.floor(Math.random() * 16)];
        }
        return color;
    }
};
var InitSystemLogConfigStatistic = function () {
    SystemLogConfig.initChart();
}