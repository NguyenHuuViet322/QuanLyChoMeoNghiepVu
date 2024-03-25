var DocTypeConfig = {
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
    ChangeType: function () {

        jQuery(document).on("change", ".onChangeType", function () {
            var obj = jQuery(this);
            var url = obj.attr("data-url");
            var target = obj.attr("data-target");
            var data = {
                type: obj.val(),
                id: obj.attr("data-id-doctype")
            }

            let ssCallBack = function (rs) {
                jQuery(target).html(rs);
                CommonJs.UpdateSelect2(jQuery(target));
            };
            let errCallback = function (rs) {

            };
            CommonJs.CustAjaxCall(data, "POST", url, null, ssCallBack, errCallback);
        });
    },
};

var InitDocType = function () {
    DocTypeConfig.ChangeDataType();
    DocTypeConfig.ChangeType();
}