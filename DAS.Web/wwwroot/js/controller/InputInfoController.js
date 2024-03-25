var InputInfoConfig = {

    onEvents: function () {

        $(document).on("click", ".onSetKieuTach", function (e) {
            var btn = $(this);
            var form = btn.closest("form");
            var dataId = btn.data("id");
            form.find(`.onSetKieuTach:not([data-id='${dataId}'])`).removeClass("active");
            var targetInput = form.find(btn.attr("data-target-input"));
            btn.toggleClass("active");
            if (btn.hasClass("active")) {
                targetInput.val(dataId);
            }
            else {
                targetInput.val("");
            }
        });

        jQuery(document).on("change", ".onGetColumnMapper", function (e) {
            e.preventDefault();
            var obj = jQuery(this);
            if (obj.hasClass("disabled"))
                return false;
            var target = $(obj.data("target"));
            CommonJs.LazyLoadAjaxPro();
            InputInfoConfig.GetSyncData(obj, function (res) {
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
        jQuery(document).on("change", ".onChangeFieldIsIdentity", function (e) {
            e.preventDefault();
            var obj = jQuery(this);
            var form = obj.closest("form");
            var target = form.find(obj.data("target"));
            CommonJs.DestroyValidator(form);
            if (obj.is(":checked")) {
                target.prop('readonly', true);
                target.attr('data-bv-notempty', false);
            } else {
                target.prop('readonly', false);
                target.attr('data-bv-notempty', true);
            }
            CommonJs.BootstrapValidator(form);
        });
    },
    GetSyncData: function (obj, ssCallback) {
        var form = obj.closest("form");
        var formData = new FormData();
        var data = CommonJs.GetSerialize2(form);
        let table = obj.attr("data-table");
        let IDRecords = [];
        jQuery(table).find(".checkboxes").each(function () {
            if (jQuery(this).prop("checked")) {
                var id = jQuery(this).data("id");
                if (CommonJs.IsInteger(id)) {
                    IDRecords.push(id);
                }
            }
        });
        for (var key in data) {
            if (data.hasOwnProperty(key)) {
                formData.append(key, data[key]);
            }
        }
        for (var i = 0; i < IDRecords.length; i++) {
            formData.append("IDRecords", IDRecords[i]);
        }


        $.ajax({
            url: obj.attr("data-url"),
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

var InitInputInfo = function () {

    InputInfoConfig.onEvents();
}