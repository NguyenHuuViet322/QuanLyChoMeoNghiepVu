var TeamsConfig = {
    OpenPopup: function () {
        CommonJs.Select2Init();
        //$("#ckAllRoles").click(function () {
        //    if ($("#ckAllRoles").is(':checked')) {
        //        $("#IDRoleStrs > option").prop("selected", "selected");// Select All Options
        //        $("#IDRoleStrs").trigger("change");// Trigger change to select 2
        //    } else {
        //        $("#IDRoleStrs > option").prop("selected", false);// Unselect All Options
        //        $("#IDRoleStrs").trigger("change");// Trigger change to select 2
        //    }
        //});
        //$("#IDRoleStrs").on("change", function () {
        //    if ($(this).val() == null || $(this).val() == undefined || $(this).val().length == 0) {
        //        $("#ckAllRoles").prop("checked", false);
        //    }
        //})
        $("#ckAllGroupPers").click(function () {
            if ($("#ckAllGroupPers").is(':checked')) {
                $("#IDGroupPerStrs > option").prop("selected", "selected");// Select All Options
                $("#IDGroupPerStrs").trigger("change");// Trigger change to select 2
            } else {
                $("#IDGroupPerStrs > option").prop("selected", false);// Unselect All Options
                $("#IDGroupPerStrs").trigger("change");// Trigger change to select 2
            }
        });
        $("#IDGroupPerStrs").on("change", function () {
            if ($(this).val() == null || $(this).val() == undefined || $(this).val().length == 0) {
                $("#ckAllGroupPers").prop("checked", false);
            }
            else {
                if ($("#ckAllGroupPers").is(':checked') && ($("#IDGroupPerStrs option").length > $("#IDGroupPerStrs").val().length)) {
                    $("#ckAllGroupPers").prop("checked", false);
                }
            }
        })
    }
};
