var RoleConfig = {
    Init: function () {
        $("#SearchRole").click(function () {
            RoleConfig.SearchRole(1);
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
    },
    SearchRole: function (pageIndex) {
        $("#hddPageIndex").val(pageIndex);
        let Keyword = $("#Keyword").val();

        let condition = {
            Keyword: Keyword,
            PageIndex: pageIndex,
            PageSize: $("#drdPageSize").val()
        };
        let method = "POST";
        let url = "/Role/SearchByCondition";
        let ssCallback = function (data) {
            $("#divRoles").html(data);
            CommonJs.Select2Init();
            $("#SearchRole").click(function () {
                RoleConfig.SearchRole(1);
            });
        }
        CommonJs.CustAjaxCall(condition, method, url, "", ssCallback, "");
        return false;
    }
};