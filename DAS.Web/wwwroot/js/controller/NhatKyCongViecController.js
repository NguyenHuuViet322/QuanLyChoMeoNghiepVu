var NhatKyCongViec = {
    OnLeaveTextbox: function () {
        jQuery(document).on("change", ".textbox-for-login, .textbox-valid", function () {
            var target = $(this).attr("data-target");
            jQuery(".field-validation-error").html("");
            $(target).html("");
        });
    },
    GetCVTon: function (data,target,url) {
        let form = $(this);
        jQuery.ajax({
            type: 'get',
            async: true,
            url: url,
            data: data,
            beforeSend: function () {
                $(target).html('');
            },
            success: function (rs) {
                $(target).html(rs);
            }
        });
        return false;
    },
    OnChange: function () {
        jQuery(document).on("change", "#nkcvngay,#nkcvca", function () {
            var form = jQuery(this).closest('form');
            var data = CommonJs.GetSerialize2(form);
            var url = '/nhatkycongviec/getton';
            NhatKyCongViec.GetCVTon(data,'#data_tableton',url);
        });
    }
};