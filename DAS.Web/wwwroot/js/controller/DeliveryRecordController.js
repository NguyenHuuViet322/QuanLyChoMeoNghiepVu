var DeliveryRecordConfigs = {
    Init: function () {
        jQuery(document).on("click", ".quickDownload", function () {
            var self = $(this);
            var form = self.closest("form");

            if (form.hasClass("validateForm")) {
                var bootstrapValidator = form.data('bootstrapValidator');
                bootstrapValidator.validate();
                if (!bootstrapValidator.isValid(true)) {
                    return false;
                }
            }

            //Clear old message
            jQuery(".field-validation-valid").html("");
            var idValue = self.attr("data-id");
            let urlDownload = self.attr("data-download");
            let url = self.attr("data-url");
            let resUrl = self.attr("data-resurl");
            $.ajax(
                {
                    url: url,
                    data: { id: idValue },
                    processData: false,
                    contentType: false,
                    type: "POST",
                    success: function (res) {
                        if (res.type === "Success") {
                            window.location.href = urlDownload;
                        }
                        else {
                            CommonJs.SetMessage(res, true, resUrl);
                        }
                    }
                }
            );
            //CommonJs.CustAjaxCall(data, "POST", url, dataType, ssCallBack, errCallback);
            return false;
        });
    },
    OnLeaveTextbox: function () {
        jQuery(document).on("change", ".textbox-for-login, .textbox-valid", function () {
            var target = $(this).attr("data-target");
            jQuery(".field-validation-error").html("");
            $(target).html("");
        });
    },
    OpenPopup: function () {
        CommonJs.Select2Init();
        CommonJs.DateTimeFormat();

        //Khi thay đổi kế hoạch
        jQuery(document).on("click", "#ckAllGroupPers", function () {
            if ($(this).is(':checked')) {
                $("#IDPlanProfileStrs > option").prop("selected", "selected");// Select All Options
                $("#IDPlanProfileStrs").trigger("change");// Trigger change to select 2
            } else {
                $("#IDPlanProfileStrs > option").prop("selected", false);// Unselect All Options
                $("#IDPlanProfileStrs").trigger("change");// Trigger change to select 2
            }
        });

        //Khi thay đổi kế hoạch
        jQuery(document).on("change", "#crIDPlan", function () {
            $("#IDPlanProfileStrs > option").prop("selected", false);// Unselect All Options
            $("#IDPlanProfileStrs").trigger("change");// Trigger change to select 2
            $("#ckAllGroupPers").prop("checked", false);
        });

        jQuery(document).on("change", "#IDPlanProfileStrs", function () {
            var maxLength = $("#IDPlanProfileStrs option").length;
            var len = $("#IDPlanProfileStrs").val().length;
            if (len < maxLength) {
                $("#ckAllGroupPers").prop("checked", false);
            } else {
                $("#ckAllGroupPers").prop("checked", true);
            };
        });

        //Khi thay đổi Đơn vị
        jQuery(document).on("change", "#crIDAgency", function () {
            let self = $(this);
            let form = self.closest("form");
            let data = form.serializeArray();
            var idAgency = jQuery(this).val();
            var idPlan = jQuery("#crIDPlan").val();
            $("#ckAllGroupPers").prop("checked", false);
            if (idAgency == 0) {
                //planProfile.val(0);
                $("#IDPlanProfileStrs > option").prop("selected", false);// Unselect All Options
                $("#IDPlanProfileStrs").trigger("change");// Trigger change to select 2
            }
            else {
                if (idPlan == 0) {
                    $("#IDPlanProfileStrs > option").prop("selected", false);// Unselect All Options
                    $("#IDPlanProfileStrs").trigger("change");// Trigger change to select 2
                    $("#ckAllGroupPers").prop("checked", false);
                } else {
                    url = "DeliveryRecord/ChangeListPlanProfile";
                    jQuery.ajax({
                        type: "POST",
                        async: true,
                        url: url,
                        data: data,
                        success: function (rs) {
                            $("#IDPlanProfileStrs").html(rs.data);
                            var maxLength = $("#IDPlanProfileStrs option").length;
                            if (maxLength > 0) {
                                $("#IDPlanProfileStrs > option").prop("selected", "selected");// Select All Options
                                $("#IDPlanProfileStrs").trigger("change");// Trigger change to select 2
                                $("#ckAllGroupPers").prop("checked", true);
                            }
                        }
                    });
                }
            };
        });
    },
    UpdateTeam: function (idName, idWrapper, idRemoved) {
        let IDGroupPers = $("#IDPlanProfileStrs").val();
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
            $("#" + idName).on("select2:select", function () {
                let length = $("#IDPlanProfileStrs").val().length;
                if ($("#IDTeamStrs option").length == length)
                    $("#ckAllTeams").prop("checked", true);
                UsersConfig.UpdateGroupPer("IDPlanProfileStrs", "editUserGroupPer", "selected");
            }).on("select2:unselect", function (e) {
                $("#ckAllTeams").prop("checked", false);
                var idRemoved = e.params.data.id;
                UsersConfig.UpdateGroupPer("IDPlanProfileStrs", "editUserGroupPer", "unselected", idRemoved);
            });
        }
        CommonJs.CustAjaxCall(someData, method, url, "", ssCallback, "");
    },
    UpdatesPlanProfiles: function (idName, idWrapper, type, idRemoved) {
        let IDGroupPers = $("#IDPlanProfileStrs").val();
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
            }).on("select2:select", function () {
                let length = $("#IDPlanProfileStrs").val().length;
                if ($("#IDPlanProfileStrs option").length == length)
                    $("#ckAllGroupPers").prop("checked", true);
            }).on("select2:unselect", function () {
                $("#ckAllGroupPers").prop("checked", false);
                var idRemoved = e.params.data.id;
                UsersConfig.UpdateTeam("IDTeamStrs", "editUserTeam", idRemoved);
            });
        }
        CommonJs.CustAjaxCall(someData, method, url, "", ssCallback, "");
    },
}