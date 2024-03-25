var ImportDataConfig = {

    onEvents: function () {

        jQuery(document).on("change", ".onReloadHeader", function (e) {

            $(".onGetHeader").trigger("click");
        });

        jQuery(document).on("change", ".onChangeHeaderRow", function (e) {

            $("#txtDataRow").val(parseInt($(this).val()) + 1);
        });

        jQuery(document).on("click", ".onGetHeader,.onMapColumn", function (e) {
            e.preventDefault();
            var obj = jQuery(this);
            if (obj.hasClass("disabled"))
                return false;
            var target = $(obj.data("target"));
            CommonJs.LazyLoadAjaxPro();
            ImportDataConfig.GetImportData(obj, function (res) {
                if (res && res.type && res.message) {
                    if (res.type != undefined)
                        res.type = res.type.toLowerCase();
                    CommonJs.ShowNotifyMsg(res.type, res.message);

                    target.html("");
                    return false;
                }

                if (res) {
                    target.html(res);
                    CommonJs.UpdateSelect2(target);
                }
            });
        });

        jQuery(document).on("click", ".onImportData", function (e) {
            e.preventDefault();
            var obj = jQuery(this);
            if (obj.hasClass("disabled"))
                return false;
            CommonJs.LazyLoadAjaxPro();
            ImportDataConfig.GetImportData(obj, function (res) {
                console.log(res);
                if (res) {
                    if (res.type != undefined)
                        res.type = res.type.toLowerCase();
                    CommonJs.SetMessageNotRefresh(res, true);
                    if (res.type == "success" && obj.hasClass("successOnReload")) {

                        obj.closest('.modal').modal('hide')
                        window.location.reload();
                    }
                }
            });
        });

        jQuery(document).on("click", ".onSelectImportFile", function (e) {
            e.preventDefault();
            var obj = jQuery(this);
            var rel = obj.attr("data-rel");
            jQuery(rel).trigger("click");
            jQuery(rel).attr('data-modal', obj.attr('data-modal'));

        });
        jQuery(document).on("change", ".onFileImportChange", function (e) {
            e.preventDefault();
            var obj = jQuery(this);
            let modal = obj.attr("data-modal");
            CommonJs.OpenModal(jQuery(modal));
            $(".onGetHeader").trigger("click");
        });
    },
    GetImportData: function (obj, ssCallback) {
        var form = obj.closest("form");
        var files = form.find('input[type="file"]');
        var formData = new FormData();
        var data = CommonJs.GetSerialize2(form);
        for (var key in data) {
            if (data.hasOwnProperty(key)) {
                formData.append(key, data[key]);
            }
        }
        if (files.length > 0) {
            files.each(function () {
                var name = jQuery(this).attr("name");
                if (name) {
                    formData.append(name, $(this)[0].files[0]);
                }
            });
        } else {
            return;
        }
        $.ajax({
            url: obj.attr("href"),
            type: 'POST',
            data: formData,
            processData: false,  // tell jQuery not to process the data
            contentType: false,  // tell jQuery not to set contentType 
            beforeSend: function () {
                obj.addClass('disabled', true);
            },
            success: function (result) {
            },
            error: function (jqXHR) {
            },
            complete: function (jqXHR, status) {
                obj.removeClass('disabled', false);
            },
            success: function (res) {
                obj.removeClass('disabled', false);
                ssCallback(res);
            },
        });
    }
};

var InitImportData = function () {

    ImportDataConfig.onEvents();
}