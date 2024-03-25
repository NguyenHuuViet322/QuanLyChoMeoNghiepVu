var DongVatNghiepVuConfig = {

    onEvents: function () {
        $(document).on("change", ".onChangeDongVat", function () {
            let el = $(this);
            let form = el.closest("form");
            if (el.val() == "" || el.val() == "0") {
                el.val(0);
                return false;
            }
            let data = {
                ID: el.val(),
                IsKhaiBaoMat: true
            };
            let method = "POST";
            let url = el.data("url");
            let target = el.data("target");


            jQuery.ajax({
                type: method,
                async: true,
                url: url,
                data: data,
                beforeSend: function () {
                    CommonJs.LazyLoadAjaxPro();
                },
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.SetMessage(rs);
                        return false;
                    };

                    $(target).html(rs);
                    CommonJs.UpdateFormState($(target));
                    CommonJs.UpdateInputMoney($(target));
                    CommonJs.UpdateIsNumber($(target));
                    CommonJs.DateTimeFormat();
                    CommonJs.Select2Init();
                    CommonJs.UpdateTreeGrid();
                }
            });
        });

        $(document).on("change", ".onChangeCbPhanLoai", function () {
            let el = $(this);
            let id = el.attr("id");
            let form = el.closest('form');
            form.find(`.onChangeCbPhanLoai:not(#${id})`).prop("checked", false);
            let target = $(el.data('target'))
            let isCheck = el.is(":checked");
            if (target) {
                if (isCheck)
                    target.removeClass('hidden');
                else
                    target.addClass('hidden');
            }
        });

        $(document).on("change", ".onChangeDdlPhanLoai", function () {
            let el = $(this);
            let val = el.val();
            let target = $(el.data('target'))
            let hiddenVal = el.attr("data-hidden-value");
            if (hiddenVal && hiddenVal.includes(val)) {
                target.addClass('hidden');
            }
            else {
                target.removeClass('hidden');
            }
        });

        jQuery(document).on("change", ".onChangeDV", function () {
            var obj = jQuery(this);
            var url = obj.attr("data-url");
            var target = obj.attr("data-target");
            var data = {
                SelectedID: obj.val(),
                DefaultText: obj.attr("data-default-text")
            }
            let ssCallBack = function (rs) {
                jQuery(target).html(rs.data);
                CommonJs.UpdateSelect2(jQuery(target));
            };
            let errCallback = function (rs) {

            };
            CommonJs.CustAjaxCall(data, "POST", url, null, ssCallBack, errCallback);
        });
    },
};

var InitDongVatNghiepVu = function () {
    DongVatNghiepVuConfig.onEvents();
} 