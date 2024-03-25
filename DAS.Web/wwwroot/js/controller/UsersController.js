var UsersConfig = {
    Init: function () {
        AgencyConfig.GetAgencysByOrganId($("#Agencies").val(), $("#Agencys").val(), true, "Agencys", "elmBindingAgency");
        $("#Agencies").change(function () {
            AgencyConfig.GetAgencysByOrganId($(this).val(), $("#Agencys").val(), true, "Agencys", "elmBindingAgency");
        });
    },
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
        $("#IDGroupPerStrs").on("select2:select", function () {
            let length = $("#IDGroupPerStrs").val().length;
            if ($("#IDGroupPerStrs option").length == length)
                $("#ckAllGroupPers").prop("checked", true);
        }).on("select2:unselect", function (e) {
            $("#ckAllGroupPers").prop("checked", false);
            var idRemoved = e.params.data.id;
            UsersConfig.UpdateTeam("IDTeamStrs", "editUserTeam", idRemoved);
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
        $("#IDTeamStrs").on("select2:select", function () {
            let length = $("#IDGroupPerStrs").val().length;
            if ($("#IDTeamStrs option").length == length)
                $("#ckAllTeams").prop("checked", true);
            UsersConfig.UpdateGroupPer("IDGroupPerStrs", "editUserGroupPer", "selected");
        }).on("select2:unselect", function (e) {
            $("#ckAllTeams").prop("checked", false);
            var idRemoved = e.params.data.id;
            UsersConfig.UpdateGroupPer("IDGroupPerStrs", "editUserGroupPer", "unselected", idRemoved);
        });
        $("#HasOrganPermissionStr").click(function () {
            if ($("#HasOrganPermissionStr").is(':checked')) {
                $("#HasOrganPermission").val(true);
            } else {
                $("#HasOrganPermission").val(false);
            }
        });
        $(".onlynumber").ForceNumericOnly();
        var OrganIds = [];
        var AgencyIds = [];
        AgencyIds.push($("#editUserIDAgency").val());
        OrganIds.push($("#IDOrgan").val());
        AgencyConfig.GetAgencysByOrganId(OrganIds, AgencyIds, false, "IDAgency", "elmBindingIDAgency");
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
            $("#" + idName).on("select2:select", function () {
                let length = $("#IDGroupPerStrs").val().length;
                if ($("#IDTeamStrs option").length == length)
                    $("#ckAllTeams").prop("checked", true);
                UsersConfig.UpdateGroupPer("IDGroupPerStrs", "editUserGroupPer", "selected");
            }).on("select2:unselect", function (e) {
                $("#ckAllTeams").prop("checked", false);
                var idRemoved = e.params.data.id;
                UsersConfig.UpdateGroupPer("IDGroupPerStrs", "editUserGroupPer", "unselected", idRemoved);
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
            }).on("select2:select", function () {
                let length = $("#IDGroupPerStrs").val().length;
                if ($("#IDGroupPerStrs option").length == length)
                    $("#ckAllGroupPers").prop("checked", true);
            }).on("select2:unselect", function () {
                $("#ckAllGroupPers").prop("checked", false);
                var idRemoved = e.params.data.id;
                UsersConfig.UpdateTeam("IDTeamStrs", "editUserTeam", idRemoved);
            });
        }
        CommonJs.CustAjaxCall(someData, method, url, "", ssCallback, "");
    },
    ClickUploadTemplate: function () {
        jQuery(document).on("change", "#File", function () {
            var input = document.getElementById("File");
            var _validFileExtensions = [".xlsx"];

            //Check Input
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                var oInput = input.files[0];
                var sFileName = oInput.name;
                var blnValid = false;
                document.getElementById("TextFileName").value = sFileName;
                if (sFileName.length > 0) {
                    for (var j = 0; j < _validFileExtensions.length; j++) {
                        var sCurExtension = _validFileExtensions[j];
                        if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                            blnValid = true;
                            break;
                        }
                    }

                    if (!blnValid) {
                        document.getElementById("TextFileName").value = "";
                        document.getElementById("File").value = "";
                        alert("Xin lỗi, " + sFileName + " không đúng định dạng. Định dạng hợp lệ: " + _validFileExtensions.join(", "));
                    }
                    if (blnValid) {
                        reader.onload = function (e) {
                            $("#File").attr("src", e.target.result);
                        }

                        reader.readAsDataURL(input.files[0]); // convert to base64 string
                    }
                }
            }
        });
    },
    ClickButtonUpload: function () {
        jQuery(document).on("click", ".fileinput-button", function () {
            $("#File").trigger("click");
        });
    },
    ClickSubmitImport: function () {
        jQuery(document).on("click", ".quickSubmitImport", function () {
            var self = $(this);
            var form = self.closest("form");
            function Base64ToBytes(base64) {
                var s = window.atob(base64);
                var bytes = new Uint8Array(s.length);
                for (var i = 0; i < s.length; i++) {
                    bytes[i] = s.charCodeAt(i);
                }
                return bytes;
            };
            $(".quickSubmitImport").prop({ disabled: true });
            if (form.hasClass("validateForm")) {
                var bootstrapValidator = form.data('bootstrapValidator');
                bootstrapValidator.validate();
                if (!bootstrapValidator.isValid(true)) {
                    $(".quickSubmitImport").prop({ disabled: false });
                    if (bootstrapValidator.$invalidFields.length > 0) {
                        $(bootstrapValidator.$invalidFields[0]).focus();
                    }
                    return false;
                }
            }

            //Clear old message
            jQuery(".field-validation-valid").html("");
            var formData = new FormData();
            let url = self.attr("data-url");
            var Password = document.getElementById("Password").value;
            var ConfirmPassword = document.getElementById("ConfirmPassword").value;
            var IDOrgan = document.getElementById("IDOrgan").value;
            var IDAgency = document.getElementById("IDAgency").value;
            var IDPosition = document.getElementById("IDPosition").value;
            var IDUserGroupPerStrs = document.getElementById("IDUserGroupPerStrs").value;
            var IDTeamStrs = document.getElementById("IDTeamStrs").value;
            var Status = document.getElementById("ActiveStatus").value;
            //var Priority = document.getElementById("Priority").value;
            //Fiel
            var input = document.getElementById("File");
            var File = input.files;

            //Post data
            formData.append("Password", Password);
            formData.append("ConfirmPassword", ConfirmPassword);
            formData.append("IDOrgan", IDOrgan);
            formData.append("IDAgency", IDAgency);
            formData.append("IDPosition", IDPosition);
            formData.append("IDUserGroupPerStrs", IDUserGroupPerStrs);
            formData.append("IDTeamStrs", IDTeamStrs);
            formData.append("Status", Status);

            if (input.files && input.files[0] && input.files[0].size > CommonJs.LimitFileSize) {

                CommonJs.ResumableUpload(input, 1, function (data) {
                    var strJs = JSON.parse(data);
                    if (strJs.status != "Success") {
                        strJs["type"] = strJs.status;
                        $(".quickSubmitImport").prop({ disabled: false });
                        CommonJs.SetMessage(strJs);
                    } else {
                        if (strJs.data == 0) {
                            strJs["type"] = "error";
                            strJs["message"] = "Upload file thất bại";
                            $(".quickSubmitImport").prop({ disabled: false });
                            CommonJs.SetMessage(strJs);
                        } else {
                            formData.append("IDFile", strJs.data);
                            //Do something
                            $.ajax({
                                url: url,
                                data: formData,
                                processData: false,
                                contentType: false,
                                type: "POST",
                                success: function (res) {
                                    $(".quickSubmitImport").prop({ disabled: false });
                                    CommonJs.SetMessage(res, true);
                                }
                            });
                        }
                    }
                });
            }
            else {
                for (var i = 0; i != File.length; i++) {
                    formData.append("File", File[i]);
                }

                $.ajax({
                    url: url,
                    data: formData,
                    processData: false,
                    contentType: false,
                    type: "POST",
                    success: function (rs) {
                        $(".quickSubmitImport").prop({ disabled: false });
                        CommonJs.SetMessage(rs, true);
                        var filename = rs.fileName;
                        var type = rs.mimeType;
                        var bytes = Base64ToBytes(rs.fileContents);
                        var blob = new Blob([bytes], { type: type });
                        var downloadUrl = URL.createObjectURL(blob);
                        //window.location = downloadUrl;
                        var a = document.createElement("a");
                        a.href = downloadUrl;
                        a.download = filename;
                        document.body.appendChild(a);
                        a.click();
                        setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100);
                    }
                });
            };

            //let ssCallBack = function (res) {
            //    if (res.type === "Success") {
            //        if (form.attr("data-is-append-select") == "true") {
            //            form.closest('.modal').modal('toggle');//Close model when success
            //            var slTarget = jQuery(form.attr("data-select-target"));
            //            slTarget.append('<option value="' + res.data.id + '" >' + res.data.name + '</option>');
            //            slTarget.val(res.data.id);
            //            CommonJs.UpdateSelect2(slTarget);
            //            CommonJs.SetMessage(res, true, true);
            //            return false;
            //        }
            //
            //
            //        if (res.data && res.data.url) {
            //            window.open(res.data.url);//Close model when success
            //        }
            //        else {
            //            form.closest('.modal').modal('toggle');//Close model when success
            //            if (resUrl != '' && resUrl != undefined)
            //                setTimeout(function () {
            //                    window.location = resUrl;
            //                }, 1000);
            //            CommonJs.SetMessage(res, true, resUrl);
            //        }
            //
            //    } else if (res.type === "Redirect") {
            //        window.location.href = res.message;
            //    } else {
            //        $(".quickSubmit").prop({ disabled: false });
            //        CommonJs.SetMessage(res, true, resUrl);
            //    }
            //}
            ////todo : handle with errCallback
            //let errCallback = function () {
            //    $(".quickSubmit").prop({ disabled: false });
            //    return false;
            //}
            //
            //CommonJs.CustAjaxCall(data, "POST", url, dataType, ssCallBack, errCallback);
            return false;
        });
    },
};
