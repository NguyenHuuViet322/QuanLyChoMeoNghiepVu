var OrganConfig = {
    Init: function () {
        $("#SearchOrgan").click(function () {
            OrganConfig.SearchOrgan(1);
        });     
    },
    OpenPopup: function () {
        $("#planCreateForm .select2").select2({
            width: '.form-group',
            allowClear: true,
            placeholder: function () {
                $(this).data('placeholder');
            },
            language: "vi"
        });
        $(".onlynumber").ForceNumericOnly();
    }
};