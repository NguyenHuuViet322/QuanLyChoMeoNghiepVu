const options = {
    fontName: 'Roboto',
    title: '',
    titleTextStyle: {
        fontSize: 14
    },
    tooltip: { ignoreBounds: true },
    backgroundColor: 'transparent',
    legend: { maxLines: 3, alignment: 'center' },
    height: 350,
};

var CollectionReport = {
    initChart: function () {
        try {
            let dict = JSON.parse($("#tableStatisticData").val());

            var colors = ["#3366cc", "#dc3912", "#ff9900", "#109618", "#990099", "#0099c6", "#dd4477", "#66aa00", "#b82e2e", "#316395", "#3366cc", "#994499", "#22aa99", "#aaaa11", "#6633cc", "#e67300", "#8b0707", "#651067", "#329262", "#5574a6", "#3b3eac", "#b77322", "#16d620", "#b91383", "#f4359e", "#9c5935", "#a9c413", "#2a778d", "#668d1c", "#bea413", "#0c5922", "#743411"];

            while (colors.length < dict.labels.length) {
                colors = colors.concat(colors);
            }

            var ctx = document.getElementById('archive_chart');
            //var data = {
            //    labels: labelDict,
            //    datasets: [{
            //        data: dataDict,
            //        backgroundColor: backgroundColorDict
            //    }]
            //};
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
                chart: {
                    height: 100
                },
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
                    yAxes: [{
                        ticks: {
                            min: 0
                        }
                    }],
                    y: {
                        beginAtZero: true
                    }
                }
                //backgroundColor: '#2286ef'
            };
            new Chart(ctx, {
                type: 'bar',
                data: data,
                options: option
            });
        } catch (e) {
            console.log(e);
        }
    },


    getRandomColorHex: function () {
        var hex = "0123456789ABCDEF",
            color = "#";
        for (var i = 1; i <= 6; i++) {
            color += hex[Math.floor(Math.random() * 16)];
        }
        return color;
    }
}
var InitCollectionReport = function () {
    CollectionReport.initChart();
}