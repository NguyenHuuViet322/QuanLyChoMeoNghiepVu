/**************************************************************/
/***********************SCAN DOC*******************************/
/**************************************************************/
var Cdata = {
    Storage: {
        domain: "localhost:44387"
    }
};
var CDataInit = function () {
    try {
        var data = jQuery("#CDATA").val();
        var dataJson = jQuery.parseJSON(data);
        for (var k in dataJson) {
            if (dataJson.hasOwnProperty(k)) {
                Cdata[k] = dataJson[k];
            }
        }
    } catch (e) {
    }
}

var ScanDoc = {
    onEvent: function () {
        var ScanSettings = {
            url: "http://localhost:82"
        };

        var total_page = 1;
        function ScanFile_Page() {
            if ($(document).find(".items_Scan").is(":visible")) {
                total_page = $(document).find(".items_Scan .itemScan").length;
                $(document).find("#num_Pages").find("b").text(total_page);
                $(document).find("#page_Number").attr("max", total_page);
                $(document).find(".items_Scan").find(".itemScan").each(function () {
                    var page_index = jQuery(this).index() + 1;
                    jQuery(this).attr("page_index", page_index);
                    if (page_index == 1) {
                        jQuery(this).addClass("over_top");
                        jQuery(this).addClass("first_over_top");
                    }
                });
                var current_page = 1;
                $(document).find(".ScanResult").find(".items_Scan").scroll(function () {
                    $(document).find(".items_Scan").find(".itemScan:not(.first_over_top)").each(function () {
                        var pos = jQuery(this).position();
                        var PosScroll = jQuery(this).scrollTop();
                        if ((parseInt(PosScroll) + 10) > parseInt(pos.top)) {
                            jQuery(this).addClass("over_top");
                        } else {
                            jQuery(this).removeClass("over_top");
                        }
                    });
                    current_page = $(document).find(".items_Scan").find(".itemScan.over_top").length;
                    $(document).find("#page_Number").val(current_page);
                });
                $(document).find('#page_Number').on('change', function () {
                    goto_page = $(document).find('#page_Number').val();
                    var container = $(document).find('.ScanResult').find('.items_Scan'),
                        scrollTo = $(document).find('.ScanResult').find('.items_Scan').find('.itemScan[page_index=' + goto_page + ']');
                    container.scrollTop(
                        scrollTo.offset().top - container.offset().top + container.scrollTop()
                    );
                });
                $(document).find('.other_tools').find('.goto_firstpage ').on('click', function () {
                    var container = $(document).find('.ScanResult').find('.items_Scan'),
                        scrollTo = $(document).find('.ScanResult').find('.items_Scan').find('.itemScan[page_index="1"]');
                    container.scrollTop(
                        scrollTo.offset().top - container.offset().top + container.scrollTop()
                    );
                });
                $(document).find('.other_tools').find('.goto_lastpage ').on('click', function () {
                    var container = $(document).find('.ScanResult').find('.items_Scan'),
                        scrollTo = $(document).find('.ScanResult').find('.items_Scan').find('.itemScan[page_index=' + total_page + ']');
                    container.scrollTop(
                        scrollTo.offset().top - container.offset().top + container.scrollTop()
                    );
                });
                $(document).find('.prev_next_page').find('.prev_page').on('click', function () {
                    var tem_page = current_page;
                    if (tem_page > 1) {
                        tem_page = parseInt(tem_page) - 1;
                        current_page = tem_page;
                    }
                    var container = $('.ScanResult').find('.items_Scan'),
                        scrollTo = $(document).find('.ScanResult').find('.items_Scan').find('.itemScan[page_index=' + current_page + ']');
                    container.scrollTop(
                        scrollTo.offset().top - container.offset().top + container.scrollTop()
                    );
                });
                $(document).find('.prev_next_page').find('.next_page').on('click', function () {
                    var tem_page = current_page;
                    if (tem_page < total_page) {
                        tem_page = parseInt(tem_page) + 1;
                        current_page = tem_page;
                    }
                    var container = $(document).find('.ScanResult').find('.items_Scan'),
                        scrollTo = $(document).find('.ScanResult').find('.items_Scan').find('.itemScan[page_index=' + current_page + ']');
                    container.scrollTop(
                        scrollTo.offset().top - container.offset().top + container.scrollTop()
                    );
                });
            }
        }
        function ScanFile_Atc() {
            //Xóa ảnh
            if ($(document).find(".items_Scan").is(":visible")) {
                $(document).find(".items_Scan").find(".itemScan").each(function () {
                    var obj = jQuery(this);
                    obj.find("img").wrapAll('<div class="itemScan_img"></div>');
                    obj.find(".itemScan_img").append('<a href="#" title="Xóa ảnh" class="Del_itemScan"><i class="ion-ios-close-empty"></i></a>');
                    obj.find('.Del_itemScan').on('click', function () {
                        if (confirm('Bạn có muốn xóa ảnh này không ?')) {
                            //var img_id = jQuery(this).parents('.itemScan').attr("data-value");
                            jQuery(this).parents('.itemScan').remove();
                            //console.log("Remove img id " +img_id+ " done");
                            ScanFile_Page();
                        }
                    });
                });
            }
        }
        var szoom_stt = 1;
        function ScanFile_FixWH() {
            $(document).find(".items_Scan").find(".itemScan").each(function () {
                var obj = jQuery(this);
                var sf_w = 0;
                var sf_h = 0;
                if (szoom_stt == 1) { //Auto zoom
                    sf_w = 680;
                } else if (szoom_stt == 2) { //Full width
                    sf_w = obj.width();
                } else if (szoom_stt == 3) { //50%
                    sf_w = 340;
                } else if (szoom_stt == 4) { //75%
                    sf_w = 510;
                } else if (szoom_stt == 5) { //100%
                    sf_w = 680;
                } else if (szoom_stt == 6) { //125%
                    sf_w = 850;
                } else if (szoom_stt == 7) { //150%
                    sf_w = 1020;
                } else if (szoom_stt == 8) { //175%
                    sf_w = 1190;
                } else if (szoom_stt == 9) { //200%
                    sf_w = 1360;
                }
                else {
                    sf_w = 680;
                }
                obj.find(".itemScan_img").css("width", sf_w);
                obj.find(".itemScan_img img").css("width", sf_w);
                sf_h = obj.find(".itemScan_img").find("img").height();
                console.log("sf_h:" + sf_h);
                var ratio = 0;
                if (parseFloat(sf_w) > parseFloat(sf_h)) {
                    ratio = parseFloat(sf_w) / parseFloat(sf_h);
                } else if (parseFloat(sf_w) < parseFloat(sf_h)) {
                    ratio = parseFloat(sf_h) / parseFloat(sf_w);
                } else { //auto zoom
                    ratio = 1;
                }
                console.log("ratio:" + ratio);

                obj.find(".itemScan_img").css("width", 0);
                obj.find(".itemScan_img").css("height", 0);
                obj.find(".itemScan_img").css({ "padding-top": sf_h / 2, "padding-right": sf_w / 2, "padding-bottom": sf_h / 2, "padding-left": sf_w / 2 });
                obj.find(".itemScan_img").attr("padding-lr", sf_w / 2);
                obj.find(".itemScan_img").attr("padding-bt", sf_h / 2);
                obj.find(".itemScan_img").attr("ratio", ratio);
                obj.find(".itemScan_img img").css({ "position": "absolute", "top": "0", "left": "0", "max-width": "none" });
                console.log("ScanFile_FixWH - ScanFile_FixWH");
            });
        }
        var rorated = false;
        var rorate_deg = 0;
        function ScanFile_Rorate() {
            $(document).find(".other_tools").find(".rorate_left").on('click', function () {
                ScanFile_FixWH();
                if (!rorated) {
                    rorated = true;
                } else {
                    rorated = false;
                }
                rorate_deg = rorate_deg - 90;
                rorate_css = 'rotate(' + rorate_deg + 'deg)';
                if ($(document).find(".items_Scan").find(".itemScan").is(":visible")) {
                    $(document).find(".items_Scan").find(".itemScan").each(function () {
                        var obj = jQuery(this);
                        obj.find("img").css({ "transform": rorate_css });
                        var padding_lr = obj.find(".itemScan_img").attr("padding-lr");
                        var padding_bt = obj.find(".itemScan_img").attr("padding-bt");
                        var img_ratio = obj.find(".itemScan_img").attr("ratio");
                        if (rorated) {
                            if (szoom_stt == 2) {
                                if (parseFloat(padding_lr) > parseFloat(padding_bt)) {
                                    obj.find(".itemScan_img").css({ "padding": (padding_lr * img_ratio) + "px " + padding_lr + "px" });
                                    obj.find(".itemScan_img img").css("width", (padding_lr * 2) * img_ratio);
                                    obj.find(".itemScan_img img").css("margin-left", -((padding_lr * img_ratio) - padding_lr));
                                    obj.find(".itemScan_img img").css("margin-top", (padding_lr * img_ratio) - padding_lr);
                                } else {
                                    obj.find(".itemScan_img").css({ "padding": (padding_lr / img_ratio) + "px " + padding_lr + "px" });
                                    obj.find(".itemScan_img img").css("width", (padding_lr * 2) / img_ratio);
                                    obj.find(".itemScan_img img").css("margin-left", (padding_lr - (padding_lr / img_ratio)));
                                    obj.find(".itemScan_img img").css("margin-top", -(padding_lr - (padding_lr / img_ratio)));
                                };
                            } else {
                                obj.find(".itemScan_img").css({ "padding": padding_lr + "px " + padding_bt + "px" });
                                obj.find(".itemScan_img img").css("width", padding_lr * 2);
                                if (parseFloat(padding_lr) > parseFloat(padding_bt)) {
                                    obj.find(".itemScan_img img").css("margin-left", -(padding_lr - padding_bt));
                                    obj.find(".itemScan_img img").css("margin-top", padding_lr - padding_bt);
                                } else {
                                    obj.find(".itemScan_img img").css("margin-left", -(padding_lr - padding_bt));
                                    obj.find(".itemScan_img img").css("margin-top", padding_lr - padding_bt);
                                };
                            }
                        } else {
                            if (szoom_stt == 2) {
                                if (parseFloat(padding_lr) > parseFloat(padding_bt)) {
                                    obj.find(".itemScan_img").css({ "padding": padding_bt + "px " + padding_lr + "px" });
                                    obj.find(".itemScan_img img").css("width", (padding_lr * 2));
                                    obj.find(".itemScan_img img").css("margin-left", 0);
                                    obj.find(".itemScan_img img").css("margin-top", 0);
                                } else {
                                    obj.find(".itemScan_img").css({ "padding": padding_bt + "px " + padding_lr + "px" });
                                    obj.find(".itemScan_img img").css("width", (padding_lr * 2));
                                    obj.find(".itemScan_img img").css("margin-left", 0);
                                    obj.find(".itemScan_img img").css("margin-top", 0);
                                };
                            } else {
                                obj.find(".itemScan_img").css({ "padding": padding_bt + "px " + padding_lr + "px" });
                                obj.find(".itemScan_img img").css("width", padding_lr * 2);
                                if (parseFloat(padding_lr) > parseFloat(padding_bt)) {
                                    obj.find(".itemScan_img img").css("margin-left", 0);
                                    obj.find(".itemScan_img img").css("margin-top", 0);
                                }
                                else {
                                    obj.find(".itemScan_img img").css("margin-left", 0);
                                    obj.find(".itemScan_img img").css("margin-top", 0);
                                }
                            }
                        }
                    });
                } else {
                    alert("Không có ảnh để hiển thị");
                }
            });
            $(document).find(".other_tools").find(".rorate_right").on('click', function () {
                ScanFile_FixWH();
                if (!rorated) {
                    rorated = true;
                } else {
                    rorated = false;
                }
                rorate_deg = rorate_deg + 90;
                rorate_css = 'rotate(' + rorate_deg + 'deg)';
                if ($(document).find(".items_Scan").find(".itemScan").is(":visible")) {
                    $(document).find(".items_Scan").find(".itemScan").each(function () {
                        var obj = jQuery(this);
                        obj.find("img").css({ "transform": rorate_css });
                        var padding_lr = obj.find(".itemScan_img").attr("padding-lr");
                        var padding_bt = obj.find(".itemScan_img").attr("padding-bt");
                        var img_ratio = obj.find(".itemScan_img").attr("ratio");
                        if (rorated) {
                            if (szoom_stt == 2) {
                                if (parseFloat(padding_lr) > parseFloat(padding_bt)) {
                                    obj.find(".itemScan_img").css({ "padding": (padding_lr * img_ratio) + "px " + padding_lr + "px" });
                                    obj.find(".itemScan_img img").css("width", (padding_lr * 2) * img_ratio);
                                    obj.find(".itemScan_img img").css("margin-left", -((padding_lr * img_ratio) - padding_lr));
                                    obj.find(".itemScan_img img").css("margin-top", (padding_lr * img_ratio) - padding_lr);
                                } else {
                                    obj.find(".itemScan_img").css({ "padding": (padding_lr / img_ratio) + "px " + padding_lr + "px" });
                                    obj.find(".itemScan_img img").css("width", (padding_lr * 2) / img_ratio);
                                    obj.find(".itemScan_img img").css("margin-left", (padding_lr - (padding_lr / img_ratio)));
                                    obj.find(".itemScan_img img").css("margin-top", -(padding_lr - (padding_lr / img_ratio)));
                                };
                            } else {
                                obj.find(".itemScan_img").css({ "padding": padding_lr + "px " + padding_bt + "px" });
                                obj.find(".itemScan_img img").css("width", padding_lr * 2);
                                if (parseFloat(padding_lr) > parseFloat(padding_bt)) {
                                    obj.find(".itemScan_img img").css("margin-left", -(padding_lr - padding_bt));
                                    obj.find(".itemScan_img img").css("margin-top", padding_lr - padding_bt);
                                } else {
                                    obj.find(".itemScan_img img").css("margin-left", -(padding_lr - padding_bt));
                                    obj.find(".itemScan_img img").css("margin-top", padding_lr - padding_bt);
                                };
                            }
                        } else {
                            if (szoom_stt == 2) {
                                if (parseFloat(padding_lr) > parseFloat(padding_bt)) {
                                    obj.find(".itemScan_img").css({ "padding": padding_bt + "px " + padding_lr + "px" });
                                    obj.find(".itemScan_img img").css("width", (padding_lr * 2));
                                    obj.find(".itemScan_img img").css("margin-left", 0);
                                    obj.find(".itemScan_img img").css("margin-top", 0);
                                } else {
                                    obj.find(".itemScan_img").css({ "padding": padding_bt + "px " + padding_lr + "px" });
                                    obj.find(".itemScan_img img").css("width", (padding_lr * 2));
                                    obj.find(".itemScan_img img").css("margin-left", 0);
                                    obj.find(".itemScan_img img").css("margin-top", 0);
                                };
                            } else {
                                obj.find(".itemScan_img").css({ "padding": padding_bt + "px " + padding_lr + "px" });
                                obj.find(".itemScan_img img").css("width", padding_lr * 2);
                                if (parseFloat(padding_lr) > parseFloat(padding_bt)) {
                                    obj.find(".itemScan_img img").css("margin-left", 0);
                                    obj.find(".itemScan_img img").css("margin-top", 0);
                                }
                                else {
                                    obj.find(".itemScan_img img").css("margin-left", 0);
                                    obj.find(".itemScan_img img").css("margin-top", 0);
                                }
                            }
                        }
                    });
                } else {
                    alert("Không có ảnh để hiển thị");
                }
            });
        }

        function Getscanr(k) {

            var guid = k;
            jQuery.ajax({
                type: "GET",
                async: false,
                url: ScanSettings.url + "/plugin/getscanr?k=" + k + "&ProductKey=" + window.Cdata.ProductKey,
                success: function (response) {
                    if (response == "Wait") {
                        setTimeout(function () {
                            Getscanr(guid);
                        }, 1000);
                    }
                    else if (response == "Exit") {
                        jQuery("#Overlay").fadeOut();
                        // ScanFile_Page();
                        // ScanFile_Atc();
                        // ScanFile_FixWH();
                        // ScanFile_Rorate();
                    }
                    else {
                        jQuery("#Overlay").fadeOut();
                        try {
                            var html = "";
                            var css = "";
                            var page = 0;
                            var keys = jQuery.parseJSON(response);
                            if (keys.length > 0) {
                                for (var i in keys) {
                                    if (keys.hasOwnProperty(i)) {
                                        var k = keys[i];
                                        jQuery.ajax({
                                            type: "GET",
                                            async: false,
                                            url: ScanSettings.url + "/plugin/getimage?k=" + k + "&ProductKey=" + window.Cdata.ProductKey,
                                            success: function (response) {
                                                html += "<div data-value='" + k + "' class='itemScan " + css + "'>" +
                                                    "<img src='data:image/png;base64," + jQuery(response).text() + "' />" +
                                                    "</div>";
                                            }
                                        });
                                        //css = "hidden";
                                    }
                                }
                                jQuery(".itemScan").addClass("hidden");
                                jQuery("#ScanResult").append(html);
                                jQuery("#ScanResult").find(".itemScan").each(function () {

                                    page++;
                                    jQuery("#ScanTotalPage").val(page);
                                    if (jQuery(this).is(":visible")) {
                                        jQuery("#ScanCurrentPage").val(page);
                                    }
                                });
                                if (page > 0) {
                                    jQuery("#ScanUpfile").removeClass("hidden");
                                }

                                console.log("success");
                            }

                        } catch (e) {
                            console.log(e);
                        }
                    }
                }
            });

        }

        jQuery(document).on("change", ".scanExts", function () {
            if (jQuery("#ScanExtBMP").prop("checked")) {
                jQuery("#ScanMultiPageTIFF").attr("disabled", true).prop("checked", false);
                jQuery("#ScanMultiPagePDF").attr("disabled", true).prop("checked", false);
            }
            else if (jQuery("#ScanExtJPEG").prop("checked")) {
                jQuery("#ScanMultiPageTIFF").attr("disabled", true).prop("checked", false);
                jQuery("#ScanMultiPagePDF").attr("disabled", true).prop("checked", false);
            }
            else if (jQuery("#ScanExtTIFF").prop("checked")) {
                jQuery("#ScanMultiPageTIFF").attr("disabled", false);
                jQuery("#ScanMultiPagePDF").attr("disabled", true).prop("checked", false);
            }
            else if (jQuery("#ScanExtPNG").prop("checked")) {
                jQuery("#ScanMultiPageTIFF").attr("disabled", true).prop("checked", false);
                jQuery("#ScanMultiPagePDF").attr("disabled", true.prop("checked", false));
            }
            else if (jQuery("#ScanExtPDF").prop("checked")) {
                jQuery("#ScanMultiPageTIFF").attr("disabled", true).prop("checked", false);
                jQuery("#ScanMultiPagePDF").attr("disabled", false);
            }
        });
        jQuery(document).on("change", ".groupradios .radio", function () {

            if (jQuery(this).prop("checked")) {
                jQuery(this).closest(".groupradios").find(".radio").prop("checked", false);
                jQuery(this).prop("checked", true);
            }
        });
        jQuery(document).on("click", "#ScanReset", function () {
            jQuery("#ScanCurrentPage").val("");
            jQuery("#ScanTotalPage").val("");
            jQuery("#ScanResult").html("");
            jQuery("#ScanUpfile").addClass("hidden");
        });
        jQuery(document).on("click", ".rotateScan", function () {

            var itemScan = jQuery("#ScanResult")
                .find(".itemScan")
                .filter(":visible").first();
            var k = itemScan.attr("data-value");
            var r = jQuery(this).attr("data-degree");

            jQuery.ajax({
                type: "GET",
                url: ScanSettings.url + "/plugin/rotateimage?r=" + r + "&k=" + k,
                beforeSend: function () {
                },
                complete: function () {
                },
                error: function () {
                },
                success: function (response) {
                    var source = jQuery(response).text();
                    if (!CommonJs.IsEmpty(source)) {
                        jQuery("#ScanResult").find(".itemScan").each(function () {
                            if (jQuery(this).attr("data-value") == k) {
                                jQuery(this).find("img").attr("src", "data:image/png;base64," + source);
                            }
                        });
                    }
                }
            });
        });
        jQuery(document).on("click", "#ScanSourceRefresh", function () {

            jQuery.ajax({
                type: "GET",
                url: ScanSettings.url + "/plugin/getscanners?u=" + Cdata.CUser.Uid + "&ProductKey=" + window.Cdata.ProductKey,
                beforeSend: function () {
                    jQuery("#ScanPlugin").addClass("hidden");
                    jQuery("#Scanning").addClass("hidden");
                    jQuery("#ScanReset").addClass("hidden");
                },
                complete: function () {
                },
                error: function () {
                    jQuery("#ScanPlugin").removeClass("hidden");
                    jQuery("#Scanning").addClass("hidden");
                    jQuery("#ScanReset").addClass("hidden");
                },
                success: function (response) {

                    try {
                        response = jQuery.parseJSON(response);

                        var options = "";
                        var version = response.version;
                        if (version != "1.0.0.9")
                            throw new "cài đặt bản nâng cấp";

                        var scanners = response.scanners;
                        for (var i in scanners) {
                            var scanner = scanners[i];
                            options += "<option value='" + scanner.ProductName + "'>" + scanner.ProductName + "</option>";
                        }
                        jQuery("#ScanSource").html(options);
                        CommonJs.UpdateSelect2(jQuery(".select2"));
                        // jQuery("#ScanSource").selectpicker('refresh');
                        jQuery("#ScanPlugin").addClass("hidden");
                        jQuery("#Scanning").removeClass("hidden");
                        jQuery("#ScanReset").removeClass("hidden");

                    } catch (e) {

                        console.log(e);
                        alert("Bạn cần cài đặt bản nâng cấp trước khi quét tài liệu");

                        jQuery("#ScanPlugin").removeClass("hidden");
                        jQuery("#Scanning").addClass("hidden");
                        jQuery("#ScanReset").addClass("hidden");
                    }
                }
            });
        });

        jQuery(document).on("submit", "#FrmScan", function () {

            var form = jQuery(this);
            if (CommonJs.ValidateDataForm(form)) {
                jQuery.ajax({
                    type: "POST",
                    url: ScanSettings.url + "/plugin/acquire",
                    data: JSON.stringify({
                        Source: jQuery("#ScanSource").val(),
                        ShowUi: jQuery("#ScanShowUi").prop("checked") ? 1 : 0,
                        AutoFeeder: jQuery("#ScanAutoFeeder").prop("checked") ? 1 : 0,
                        Duplex: jQuery("#ScanDuplex").prop("checked") ? 1 : 0,
                        Resolution: parseInt(jQuery("#ScanResolution").val()),
                        BlackAndWhite: jQuery("#ScanBnW").prop("checked") ? 1 : 0,
                        Gray: jQuery("#ScanGray").prop("checked") ? 1 : 0,
                        Color: jQuery("#ScanColor").prop("checked") ? 1 : 0,
                        ProductKey: window.Cdata.ProductKey,
                    }),
                    contentType: "application/json; charset=utf-8",
                    beforeSend: function () {
                        jQuery("#Overlay").fadeIn();
                    },
                    complete: function () {
                        jQuery("#Overlay").fadeOut();
                    },
                    error: function () {
                        jQuery("#Overlay").fadeOut();
                    },
                    success: function (response) {

                        if (response != "") {
                            setTimeout(function () {
                                Getscanr(response);
                            }, 1000);
                        }
                        else {
                            jQuery("#Overlay").fadeOut();
                        }
                    }
                });
            }
            return false;
        });


        jQuery(document).on("click", ".tree-f .item", function () {
            jQuery(this).closest(".tree-f").find(".item").removeClass("selected");
            jQuery(this).addClass("selected");
            return false;
        });
        jQuery(document).on("click", ".tree-f .expanded", function () {
            jQuery(this).removeClass("expanded").addClass("collapses");
            jQuery(this).closest("li.item").find("> ul").addClass("hidden");
            return false;
        });
        jQuery(document).on("click", ".tree-f .collapses", function () {

            var perfecter = jQuery(this);
            perfecter.removeClass("collapses").addClass("loading");

            var data = perfecter.getData();
            var success = function (response) {
                perfecter
                    .removeClass("loading")
                    .addClass("expanded");
                response.Perfecter = perfecter;
                CommonJs.sectionBuilder(response);

                var liItem = perfecter.closest("li.item");
                liItem.find("ul").remove();
                liItem.append(response.htCust);
            };
            try {
                Realtime.processer.server.pageGet(data).done(success);
                return false;
            } catch (e) {
                CommonJs.formPost({
                    url: '/',
                    data: data,
                    success: success
                });
            }
            return false;
        });

        jQuery(document).on("click", "#ScanSaveLocal", function () {

            try {
                var keys = new Array();
                jQuery("#ScanResult").find(".itemScan").each(function () {
                    var k = jQuery(this).attr("data-value");
                    keys.push(k);
                });
                var data = {
                    ProductKey: window.Cdata.ProductKey,
                    Keys: keys,
                    Filename: "",
                    Extension: jQuery(".scanExts").filter(":checked").val(),
                    Multipage: jQuery("#ScanMultiPageTIFF").prop("checked")
                        || jQuery("#ScanMultiPagePDF").prop("checked") ? 1 : 0
                };

                jQuery.ajax({
                    type: "POST",
                    url: ScanSettings.url + "/plugin/savefile",
                    data: JSON.stringify(data),
                    contentType: "application/json; charset=utf-8",
                    beforeSend: function () {
                    },
                    complete: function () {
                    },
                    error: function () {
                    },
                    success: function (response) {
                    }
                });
            } catch (e) {
                console.log(e);
            }

            return false;
        });
        jQuery(document).on("submit", ".formSaveScan", function (e) {
            try {
                var form = jQuery(this);
                if (!form.hasClass("submitting")) {

                    form.addClass("submitting");

                    if (!form.hasClass("bootstrapValidator")) {
                        form.addClass("bootstrapValidator").bootstrapValidator();
                    }
                    var bootstrapValidator = form.data('bootstrapValidator');
                    bootstrapValidator.validate();
                    if (!bootstrapValidator.isValid(true)) {
                        form.removeClass("submitting");
                        return false;
                    }

                    var keys = new Array();
                    jQuery("#ScanResult").find(".itemScan").each(function () {
                        var k = jQuery(this).attr("data-value");
                        keys.push(k);
                    });
                    var data = CommonJs.GetSerialize(form);
                    data.ProductKey = window.Cdata.ProductKey
                    data.Keys = keys;
                    data.UrlSave = CommonJs.GetDomain() + form.attr("action");
                    data.UrlStorage = window.Cdata.Storage.domain;
                    data.Token = window.Cdata.Token;
                    data.Extension = jQuery(".scanExts").filter(":checked").val();
                    data.Multipage = jQuery("#ScanMultiPageTIFF").prop("checked") || jQuery("#ScanMultiPagePDF").prop("checked") ? 1 : 0;

                    if (CommonJs.IsEmpty(data.Name)) {
                        return false;
                    }
                    if (data.Keys.length == 0) {
                        return false;
                    }
                    //default
                    if (CommonJs.IsEmpty(data.Parent)) {
                        data.Parent = 0;
                    }

                    //Utils.closeOverlay(true);
                    jQuery.ajax({
                        type: "POST",
                        url: ScanSettings.url + "/plugin/upfile",
                        data: JSON.stringify(data),
                        contentType: "application/json; charset=utf-8",
                        beforeSend: function () {
                            //Utils.openOverlay();
                            form.find("[type='submit']").prop("disabled", true);
                        },
                        complete: function () {
                            //Utils.closeOverlay(true);
                            form.removeClass("submitting");
                            form.find("[type='submit']").prop("disabled", false);
                        },
                        error: function () {
                            //Utils.closeOverlay(true);
                            form.removeClass("submitting");
                            form.find("[type='submit']").prop("disabled", false);
                        },
                        success: function (response) {
                            form.removeClass("submitting");
                            try {
                                var res = JSON.parse(response).data;
                                CommonJs.ShowNotifyMsg(res.type, res.message);
                                if (res.data && res.type && res.type.toLowerCase() == "success") {
                                    jQuery("#DEFAULT_URL").val(res.data.urlFile);
                                    jQuery("input[name='IDFile']").val(res.data.idFile);
                                    jQuery(".onAutoOCR").attr("href", CommonJs.UpdateQueryStringParameter(jQuery(".onAutoOCR").attr("href"), "IDFile", res.data.idFile));
                                    //jQuery("input[name='ID']").val(res.data.idDoc);
                                    //jQuery("input[name='IDDoc']").val(res.data.idDoc);
                                    webViewerLoad();
                                    PDFViewerApplication.setScale(DEFAULT_SCALE, true);
                                    jQuery('#scanModal').modal('toggle');//Close model when success
                                    jQuery("#ScanReset").trigger("click");
                                }
                                //Utils.closeOverlay(true);
                                form.find("[type='submit']").prop("disabled", false);

                            } catch (e) {

                            }


                        }
                    });
                }

            } catch (e) {
                console.log(e);
            }

            return false;
        });

    },
    Init: function () {

        ScanDoc.onEvent();

        CDataInit();
        jQuery("#ScanSourceRefresh").trigger("click");

        $(window).bind("load", function () {
            // ScanFile_Page();
            // ScanFile_Atc();
            // ScanFile_FixWH();
            // ScanFile_Rorate();
            $(document).find('#page_zoom_select').on('change', function () {
                szoom_stt = $(document).find('#page_zoom_select :selected').val();
                ScanFile_FixWH();
                if (rorated) {
                    $(document).find(".other_tools").find(".rorate_left").click();

                }
            });
            $(document).find('.page_zoomer').find('.minus_zoom').on('click', function () {
                var tem_val = szoom_stt;
                if (tem_val > 1) {
                    tem_val = parseInt(tem_val) - 1;
                    szoom_stt = tem_val;
                }
                $(document).find('#page_zoom_select').val(szoom_stt);
                ScanFile_FixWH();
                if (rorated) {
                    $(document).find(".other_tools").find(".rorate_left").click();

                }
            });
            $(document).find('.page_zoomer').find('.plus_zoom').on('click', function () {
                var tem_val = szoom_stt;
                if (tem_val < 9) {
                    tem_val = parseInt(tem_val) + 1;
                    szoom_stt = tem_val;
                }
                $(document).find('#page_zoom_select').val(szoom_stt);
                ScanFile_FixWH();
                if (rorated) {
                    $(document).find(".other_tools").find(".rorate_left").click();

                }
            });

        });
    }
}