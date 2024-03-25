var HomeConfig = {
    Init: function () {
        jQuery(function () {
            $('body').removeClass('sidebar-mini').addClass('sidebar-collapse');
            $('.chart_container, .progress-groups, .shortcut').overlayScrollbars({
                className: 'os-theme-dark',
                sizeAutoCapable: true,
                scrollbars: {
                    clickScrolling: true
                }
            });
        });
        jQuery(function () {
            var ctx = document.getElementById('file_chart');
            var year = new Date().getFullYear();
            var dict = JSON.parse($("#hddstorages").val());
            var dataDict = [];
            var yearDict = [];
            var total = 0;
            $.each(dict, function (index, value) {
                dataDict.push(value.TotalProfile);
                yearDict.push(value.Year);
                total += value.TotalProfile;
            });
            $("#txtNumberProfile").text(total);
            var data = {
                labels: yearDict,
                datasets: [{
                    label: 'Số lượng hồ sơ',
                    borderColor: '#2286ef',
                    borderWidth: 2,
                    fill: false,
                    data: dataDict,
                }]
            };
            var option = {
                legend: {
                    display: false
                },
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                }
            };
            var plan_chart = new Chart(ctx, {
                type: 'line',
                data: data,
                options: option
            });
        });
        jQuery(function () {
            let dict = JSON.parse($("#hddprofiles").val());
            let valueProfileDict = [];
            let labelProfileDict = [];
            let backgroundColorProfileDict = [];
            let total = 0;
            $.each(dict, function (index, value) {
                total += parseInt(value.TotalProfile);
            });
            $.each(dict, function (index, value) {
                labelProfileDict.push(value.Name + " (" + value.TotalProfile + ")");
                valueProfileDict.push((value.TotalProfile * 100 / total).toFixed(2));
                switch (value.Name) {
                    case "Duyệt nộp lưu":
                        backgroundColorProfileDict.push("#488f31");
                        break;
                    case "Chờ duyệt nộp lưu":
                        backgroundColorProfileDict.push("#93a74c");
                        break;
                    case "Từ chối nộp lưu":
                        backgroundColorProfileDict.push("#ffd9a8");
                        break;
                    case "Từ chối thu thập":
                        backgroundColorProfileDict.push("#de425b");
                        break;
                    case "Chờ duyệt":
                        backgroundColorProfileDict.push("#ed7b63");
                        break;
                    case "Hoàn thành":
                        backgroundColorProfileDict.push("#cfbe75");
                        break;
                    case "Đang thu thập":
                        backgroundColorProfileDict.push("#f7ac7d");
                        break;
                    default:
                        backgroundColorProfileDict.push("#488f31");
                        break;
                }
            });
            var ctx = document.getElementById('plan_chart');
            var data = {
                labels: labelProfileDict,
                datasets: [{
                    data: valueProfileDict,
                    backgroundColor: backgroundColorProfileDict
                }]
            };
            var option = {
                legend: {
                    position: 'right'
                },
                cutoutPercentage: 40,
            };
            var plan_chart = new Chart(ctx, {
                type: 'pie',
                data: data,
                options: option
            });
        });
        jQuery(function () {
            let dict = JSON.parse($("#hddexpiriedates").val());
            let labelDict = [];
            let dataDict = [];
            let backgroundColorDict = [];
            $.each(dict, function (index, value) {
                labelDict.push(value.Name);
                dataDict.push(value.Total);
                backgroundColorDict.push('#2286ef');
            });
            var ctx = document.getElementById('archive_chart');
            var data = {
                labels: labelDict,
                datasets: [{
                    data: dataDict,
                    backgroundColor: backgroundColorDict
                }]
            };
            //var data = {
            //    labels: ['5 năm', '10 năm', '20 năm', '50 năm', '70 năm', 'Vĩnh viễn'],
            //    datasets: [{
            //        data: [8175000, 3792000, 2695000, 2099000, 1526000, 3792000],
            //        backgroundColor: ['#2286ef', '#2286ef', '#2286ef', '#2286ef', '#2286ef', '#2286ef']
            //    }]                
            //};
            var option = {
                legend: {
                    display: false
                },
                scales: {
                    yAxes: [{
                        ticks: {
                            min: 0
                        }
                    }]
                }
                //backgroundColor: '#2286ef'
            };
            var plan_chart = new Chart(ctx, {
                type: 'bar',
                data: data,
                options: option
            });
        });
        jQuery(function () {
            const shorcut = $("#shortcut .row");
            const shortcutUpdateCallback = function () {
                let sorted = $(this).sortable("toArray", { attribute: "data-id" });
                let url = '/Home/UpdateBookMark';
                let data = { ids: sorted };
                jQuery.ajax({
                    type: 'POST',
                    async: true,
                    url: url,
                    data: data,
                    traditional: true,
                    success: function (rs) {
                        if (rs.type != undefined) {
                            CommonJs.ShowNotifyMsg(rs.type, rs.message);
                            return false;
                        };
                    }
                });
            };
            const shortcutRemoveCallback = function () {
                let colremove = $(this).closest('.col');
                let idModule = colremove.attr("data-id");
                let url = '/Home/RemoveBookMark';
                let data = { id: idModule };
                jQuery.ajax({
                    type: 'POST',
                    async: true,
                    url: url,
                    data: data,
                    success: function (rs) {
                        if (rs.type != undefined) {
                            CommonJs.ShowNotifyMsg(rs.type, rs.message);
                            if (rs.type == "Success") {
                                colremove.remove();
                            }
                            return false;
                        };
                    }
                });
                //$(this).closest('.col').remove();
            };

            shorcut.sortable({
                placeholder: 'col col-auto sort-highlight',
                handle: '.handle',
                cursor: 'move',
                axis: 'x',
                revert: 300,
                opacity: 0.75,
                update: shortcutUpdateCallback
            }).disableSelection();

            $('.btn-close').on('click', shortcutRemoveCallback);
        });
        jQuery(function () {
            if ($("#TypeDataDashBoard").length > 0) {
                $("#TypeDataDashBoard").on("change", function () {
                    var form = $(this).closest("form");
                    window.history.pushState(null, "", CommonJs.FormBuilderQString(form));
                    location.reload();
                });
            }
        });

    },
};