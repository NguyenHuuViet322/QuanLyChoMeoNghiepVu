var codeOrders = ["DM_Kho", "DM_Phong", "DM_MucLuc", "DM_HopSo", "DM_HoSo"]; //Thứ tự cha con các danh mục
var codeStorageOrders = ["DM_KHO", "DM_GIA", "DM_HOPSO"]; //Thứ tự cha con các danh mục

var AutocompleteConfig = {

    AutoUpdateChildData: function () {
        $(document).on('change', '.onAutoUpdateChildData', function (e) {
            try {

                function getChildCode(index) {
                    try {
                        return codeStorageOrders[index + 1];
                    } catch (e) {
                        return "";
                    }
                }

                var obj = jQuery(this);

                var container = obj.closest(".modal");
                if (container == undefined || container.length == 0)
                    container = jQuery(document);

                var cCodeType = obj.attr("data-code-type").toUpperCase();
                if (CommonJs.IsEmpty(cCodeType))
                    return false;
                var cIndex = codeStorageOrders.findIndex(x => x === cCodeType);
                if (cIndex < 0)
                    cIndex = 0;
                var target = container.find("select[data-code-type='" + getChildCode(cIndex) + "']");
                if (target == undefined)
                    return false;
                //var target = jQuery(obj.attr("data-target"));
                let ssCallBack = function (res) {
                    container.find(".onAutoUpdateChildData").each(function () {
                        var i = codeStorageOrders.findIndex(x => x === jQuery(this).attr("data-code-type").toUpperCase());
                        if (i > cIndex) { //Xoá toàn bộ data các dm con
                            jQuery(this).html("");
                            target.addClass("rended"); //add class  để ko gọi lại sự kiện onOpenCate
                            CommonJs.UpdateSelect2(target);
                        }
                    });
                    target.addClass("rended");//add class  để ko gọi lại sự kiện onOpenCate
                    target.html(res.data);
                   CommonJs.UpdateSelect2(target);

                    if (target.val() > 0) {
                        target.trigger("change");
                    }
                }
                //var defautlTargetOption = target.find("option:first"); //Get default option of select target
                var data = {
                    CodeType: cCodeType,
                    SelectedID: obj.val(),
                    TargetSelectedID: target.val(),
                    DefaultText: target.attr("data-placeholder"),
                    //DefaultValue: "",
                };
                //todo : handle with errCallback
                let errCallback = function () {
                    return false;
                }
                CommonJs.CustAjaxCall(data, "POST", "/Autocomplete/GetStorageOptions", null, ssCallBack, errCallback);
            } catch (e) {

            }
            return false;
        });

        //Render dữ liệu khi mở lần đầu
        $(document).on('select2:opening', '.onOpenCate', function (e) {
            // e.preventDefault();

            var obj = jQuery(this);
            if (!obj.hasClass("rended")) {
                let ssCallBack = function (res) {
                    obj.html(res.data);
                    obj.addClass("rended");
                    setTimeout(function () { obj.select2("open"); }, 10); //Fix bug click 2nd time > open

                }
                var data = {
                    CategoryTypeID: obj.attr("data-categorytype-id"),
                    CategoryID: obj.attr("data-category-id"),
                    CategoryParents: obj.attr("data-category-parents"),
                    CodeType: obj.attr("data-code-type"),
                    SelectedID: obj.attr("data-selected-id"),
                    DefaultText: obj.attr("data-default-text"),
                    DefaultValue: obj.attr("data-default-value"),
                    InputType: obj.attr("data-input-type"),
                };
                //todo : handle with errCallback
                let errCallback = function () {
                    return false;
                }
                CommonJs.CustAjaxCall(data, "POST", obj.attr("data-source-url"), null, ssCallBack, errCallback);
            }
        });
        //Render sẵn dữ liệu
        $(document).ready(function () {
            var listSelect2 = jQuery('.onLoadCate');
            listSelect2.each(function () {
                var obj = jQuery(this);
                if (!obj.hasClass("rended")) {
                    let ssCallBack = function (res) {
                        obj.html(res.data);
                        obj.addClass("rended");
                    }
                    var data = {
                        CategoryTypeID: obj.attr("data-categorytype-id"),
                        CodeType: obj.attr("data-code-type"),
                        SelectedID: obj.attr("data-selected-id"),
                        DefaultText: obj.attr("data-default-text"),
                        DefaultValue: obj.attr("data-default-value"),
                        InputType: obj.attr("data-input-type"),
                    };
                    //todo : handle with errCallback
                    let errCallback = function () {
                        return false;
                    }
                    CommonJs.CustAjaxCall(data, "POST", obj.attr("data-source-url"), null, ssCallBack, errCallback);
                }
            });
        });
        //$(document).on('select2:opening', '.onOpenCate', function (e) {
        //    // e.preventDefault();

        //    var obj = jQuery(this);
        //    if (!obj.hasClass("rended")) {
        //        let ssCallBack = function (res) {
        //            obj.html(res.data);
        //            obj.addClass("rended");                   
        //        }
        //        var data = {
        //            CategoryTypeID: obj.attr("data-categorytype-id"),
        //            CodeType: obj.attr("data-code-type"),
        //            SelectedID: obj.attr("data-selected-id"),
        //            DefaultText: obj.attr("data-default-text"),
        //            DefaultValue: obj.attr("data-default-value"),
        //            InputType: obj.attr("data-input-type"),
        //        };
        //        //todo : handle with errCallback
        //        let errCallback = function () {
        //            return false;
        //        }
        //        CommonJs.CustAjaxCall(data, "POST", obj.attr("data-source-url"), null, ssCallBack, errCallback);
        //    }
        //});

    },
};

var InitAutocomplete = function () {
    AutocompleteConfig.AutoUpdateChildData();
}