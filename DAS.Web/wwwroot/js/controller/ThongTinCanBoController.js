var ThongTinCanBoConfig = {

    onEvents: function () {
        jQuery(document).on("click", ".quickSearchCus", function () {
            debugger
            let tag = $(this);
            let url = tag.attr("href");
            let target = tag.attr("data-target");
            let formTarget = tag.attr("data-formtarget");
            let fieldSearch = tag.attr("data-fieldsearch");
            let valueField = tag.attr("data-valuefield");
            let tagField = $(fieldSearch).val(valueField);
            let data = CommonJs.GetSerialize2($(formTarget));
            jQuery.ajax({
                type: "post",
                async: true,
                url: url,
                data: data,
                beforeSend: function () {
                    $(target).html('');
                },
                success: function (rs) {
                    try {
                        if (form.attr("data-state") != "0") {
                            window.history.pushState(null, "", CommonJs.FormBuilderQString(form));
                        }
                    } catch (e) {
                        console.log(e);
                    }
                    CommonJs.ToggleMultiTicks(jQuery(target));
                    $(target).html(rs);
                    CommonJs.Select2Init($(target));
                    CommonJs.UpdateTreeGrid();
                    CommonJs.UpEvent();
                }
            });
            return false;
        })
    },
};

var InitThongTinCanBo = function () {
    ThongTinCanBoConfig.onEvents();
} 