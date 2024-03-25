var TableInfoConfig = {

    onEvents: function () {

        jQuery(document).on("change", ".onSwitchDiv", function () {
            var obj = jQuery(this);
            var tr = obj.closest("form");
            tr.find(obj.attr("data-target")).addClass("hidden").find("input").each(function () {
                if (jQuery(this).attr("type") == "checkbox" || jQuery(this).attr("type") == "radio")
                    jQuery(this).prop('checked', false);
                else if (jQuery(this).hasClass("isPositiveNumber"))
                    jQuery(this).val("0");
                else
                    jQuery(this).val("");
            }); //ẩn và set 0 (só), "" (chữ)
            tr.find(obj.attr("data-target") + "[data-selected-id]").each(function () {
                if (jQuery(this).attr("data-selected-id").split(",").includes(obj.val())) {
                    jQuery(this).removeClass("hidden"); //hiển thị 
                }
            });
        });

        jQuery(document).on("change", ".onChangeIsConfig", function () {
            var isChecked = jQuery(this).val();
            if (isChecked == 1)
                jQuery(jQuery(this).attr("data-target")).removeClass("hidden");
            else
                jQuery(jQuery(this).attr("data-target")).addClass("hidden");
        });

        jQuery(document).on("change", ".onChangeIsPrimaryKey", function () {
            let table = $(this).closest("table");
            table.find('.onChangeIsPrimaryKey:checked').not(this).prop('checked', false);
        });

        jQuery(document).on("change", ".onChangeIsIdentity", function () {
            let obj = $(this);
            let form = $(this).closest("form");
            let target = form.find(obj.attr("data-target"));
            var dataType = form.find(obj.attr("data-type-target"));

            if ($(this).is(":checked")) {
                form.find(obj.attr("data-target") + "[data-selected-id]").each(function () {
                    if ($(this).attr("data-selected-id").split(",").includes(dataType.val())) {
                        $(this).removeClass("hidden"); //hiển thị 
                    }
                });
            }
            else {
                target.addClass("hidden");
            }

        });

        jQuery(document).on("change", ".onIsCloneAllRecord", function () {
            let obj = $(this);
            let target = $(obj.data("target"));
            if (target.length > 0) {
                let isChecked = obj.is(":checked");
                if (isChecked) {
                    target.val('');
                }
                target.closest(".div-input").toggleClass("hidden");
                target.prop("disabled", isChecked);
                target.prop("readonly", isChecked);
                target.trigger("change");
            }
        });
    },

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
};

var InitTableInfo = function () {
    TableInfoConfig.onEvents();
}
var InitTableInfoStatistic = function () {
    TableInfoConfig.initChart();
}