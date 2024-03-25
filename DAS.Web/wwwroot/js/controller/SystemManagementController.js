var SystemManagementConfig = {
    OpenPopup: function () {
        CommonJs.Select2Init();
        CommonJs.DateTimeFormat();
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
        }).on("select2:unselect", function (e) {
            var idRemoved = e.params.data.id;
            SystemManagementConfig.UpdateTeam("IDTeamStrs", "editUserTeam", idRemoved);
        });

        $("#ckAllTeams").click(function () {
            if ($("#ckAllTeams").is(':checked')) {
                $("#IDTeamStrs > option").prop("selected", "selected");// Select All Options
                $("#IDTeamStrs").trigger("change");// Trigger change to select 2
            } else {
                $("#IDTeamStrs > option").prop("selected", false);// Unselect All Options
                $("#IDTeamStrs").trigger("change");// Trigger change to select 2
            }
        });
        $("#IDTeamStrs").on("change", function () {
            if ($(this).val() == null || $(this).val() == undefined || $(this).val().length == 0) {
                $("#ckAllTeams").prop("checked", false);
            }
        }).on("select2:select", function () {
            SystemManagementConfig.UpdateGroupPer("IDGroupPerStrs", "editUserGroupPer", "selected");
        }).on("select2:unselect", function (e) {
            var idRemoved = e.params.data.id;
            SystemManagementConfig.UpdateGroupPer("IDGroupPerStrs", "editUserGroupPer", "unselected", idRemoved);
        });
        $(".onlynumber").ForceNumericOnly();
    },
    UpdateTeam: function (idName, idWrapper, idRemoved) {
        let IDGroupPers = $("#IDGroupPerStrs").val();
        let IDGroups = $("#IDTeamStrs").val();
        let groupPerIds = [];
        if (IDGroupPers != null && IDGroupPers != undefined && IDGroupPers != "" && IDGroupPers.length > 0) {
            $.each(IDGroupPers, function (index, value) {
                groupPerIds.push(parseInt(value));
            })
        }
        let groupIds = [];
        if (IDGroups != null && IDGroups != undefined && IDGroups != "" && IDGroups.length > 0) {
            $.each(IDGroups, function (index, value) {
                groupIds.push(parseInt(value));
            })
        }
        let method = "POST";
        let url = "/Team/UpdateTeamsByGroupPers";
        let someData = { groupPerIds: groupPerIds, groupIds: groupIds, idName: idName, idRemoved: idRemoved };
        let ssCallback = function (data) {
            //bind data
            $("#" + idWrapper).html(data);
            //if (isMultiple)
            //    $("#" + idName).prop("multiple", "multiple");
            //init event select2
            $("#" + idName).select2({
                placeholder: function () {
                    $(this).data('placeholder');
                },
                width: function () {
                    $(this).data('width');
                },
                language: "vi"
            });
            $("#" + idName).on("change", function () {
                if ($(this).val() == null || $(this).val() == undefined || $(this).val().length == 0) {
                    $("#ckAllTeams").prop("checked", false);
                }
            }).on("select2:select", function () {
                SystemManagementConfig.UpdateGroupPer("IDGroupPerStrs", "editUserGroupPer", "selected");
            }).on("select2:unselect", function (e) {
                var idRemoved = e.params.data.id;
                SystemManagementConfig.UpdateGroupPer("IDGroupPerStrs", "editUserGroupPer", "unselected", idRemoved);
            });
        }
        CommonJs.CustAjaxCall(someData, method, url, "", ssCallback, "");
    },
    UpdateGroupPer: function (idName, idWrapper, type, idRemoved) {
        let IDGroupPers = $("#IDGroupPerStrs").val();
        let IDGroups = $("#IDTeamStrs").val();
        let groupPerIds = [];
        if (IDGroupPers != null && IDGroupPers != undefined && IDGroupPers != "" && IDGroupPers.length > 0) {
            $.each(IDGroupPers, function (index, value) {
                groupPerIds.push(parseInt(value));
            })
        }
        let groupIds = [];
        if (IDGroups != null && IDGroups != undefined && IDGroups != "" && IDGroups.length > 0) {
            $.each(IDGroups, function (index, value) {
                groupIds.push(parseInt(value));
            })
        }
        let method = "POST";
        let url = "/GroupPermission/UpdateGroupPersByTeams";
        let someData = { groupPerIds: groupPerIds, groupIds: groupIds, idName: idName, type: type, idRemoved: idRemoved };
        let ssCallback = function (data) {
            //bind data
            $("#" + idWrapper).html(data);
            //if (isMultiple)
            //    $("#" + idName).prop("multiple", "multiple");
            //init event select2
            $("#" + idName).select2({
                placeholder: function () {
                    $(this).data('placeholder');
                },
                width: function () {
                    $(this).data('width');
                },
                language: "vi"
            });
            if ($("#" + idName + " option:selected").length == 0)
                $("#" + idName).val('').trigger('change');
            $("#" + idName).on("change", function () {
                if ($(this).val() == null || $(this).val() == undefined || $(this).val().length == 0) {
                    $("#ckAllGroupPers").prop("checked", false);
                }
            }).on("select2:unselect", function (e) {
                var idRemoved = e.params.data.id;
                SystemManagementConfig.UpdateTeam("IDTeamStrs", "editUserTeam", idRemoved);
            });
        }
        CommonJs.CustAjaxCall(someData, method, url, "", ssCallback, "");
    }
};
