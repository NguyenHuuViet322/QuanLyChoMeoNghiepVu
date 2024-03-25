var TableInfoApiConfig = {


    onEvents: function () {
        jQuery(document).on("change", ".onGroupViewColumnChange", function () {
            let obj = $(this);
            let childs = obj.closest("table").find(obj.attr("data-child"));
            if (obj.is(":checked")) {
                childs.prop('checked', true);
            }
            else {
                childs.prop('checked', false);
            }
        });
        jQuery(document).on("change", ".onToggleAllApp", function () {
            let obj = $(this);
            let target = $(obj.data("target"));
            if (target.length>0) {
                let isChecked = obj.is(":checked");
                if (isChecked) {
                    target.val('');
                }
                target.prop("disabled", isChecked);
                target.trigger("change");
            }
        });
    },
};

var InitTableInfoApiConfig = function () {
    TableInfoApiConfig.onEvents();
}