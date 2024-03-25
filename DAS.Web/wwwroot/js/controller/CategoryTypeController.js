var CategoryTypeConfig = {

    ChangeDataType: function () {

        jQuery(document).on("change", ".onSwitchDiv", function () {
            var obj = jQuery(this);
            var tr = obj.closest("tr");
            tr.find(obj.attr("data-target")).addClass("hidden").find("input").each(function () {
                if (jQuery(this).hasClass("isPositiveNumber"))
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
    },
    ChangeIsConfig: function () {

        jQuery(document).on("change", ".onChangeIsConfig", function () {
            var isChecked = jQuery(this).val();
            if (isChecked == 1)
                jQuery(jQuery(this).attr("data-target")).removeClass("hidden");
            else
                jQuery(jQuery(this).attr("data-target")).addClass("hidden");
        });
    },
};

var InitCategoryType = function () {
    CategoryTypeConfig.ChangeDataType();
    CategoryTypeConfig.ChangeIsConfig();
}