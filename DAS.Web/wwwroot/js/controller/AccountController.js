var AccountConfig = {
    Init: function () {
        AccountConfig.onEvent();
        AccountConfig.OpenPopup();
        AccountConfig.OnLeaveTextbox();
        AccountConfig.ClickUploadAvar();
        AccountConfig.ClickUpdateProfile();
        AccountConfig.ClickAvatar();
    },
    onEvent: function () {
        jQuery(document).on("click", ".change-password", function () {
            let btn = $(this);
            let url = "/Account/CreateChangePasswordPopup";
            let target = btn.attr("data-target");
            var data = btn.getDataUppername();
            jQuery.ajax({
                type: "GET",
                async: true,
                url: url,
                data: data,
                success: function (rs) {
                    $(target).html(rs);
                    $(target).modal({
                        backdrop: 'static', //Click outside
                        //keyboard: true, //Esc
                        show: true
                    });
                }
            });
            return false;
        });
        jQuery(document).on("click", ".logout", function () {
            //var url = $(this).attr("href");
            var url = '/Account/Logout';
            var comfirmMessage = jQuery(this).attr("data-comfirm-message") || 'Bạn có chắc chắn muốn đăng xuất?';
            CommonJs.ShowConfirmMsg(
                comfirmMessage,
                '',
                'Đăng xuất',
                'Huỷ',
                function () {
                    $.ajax({
                        async: false,
                        method: "GET",
                        url: url
                    }).done(function (data) {
                        window.location.href = '/Account/Login';
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        // If fail
                    });
                });
            return false;
        });
    },
    OpenPopup: function () {
        jQuery(document).on("click", ".quickSubmitChangPwd", function () {
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

            let data = CommonJs.GetSerialize(form); //trimspace
            let url = self.attr("data-url");
            let dataType = "json";
            let ssCallBack = function (res) {
                if (res.type === "Success")
                    form.closest('.modal').modal('toggle');//Close model when success
                CommonJs.SetMessage(res, true);
                if (res.type === "success") {
                    setTimeout(function () {
                        window.location.href = '/Account/Login';
                    }, 1000);
                }
            }
            //todo : handle with errCallback
            let errCallback = function () {
                return false;
            }

            CommonJs.CustAjaxCall(data, "POST", url, dataType, ssCallBack, errCallback);
            return false;
        });

        if (document.getElementsByClassName("onlynumber").length) {
            $(".onlynumber").ForceNumericOnly();
        }
    },
    OnLeaveTextbox: function () {
        jQuery(document).on("change", ".textbox-for-login, .textbox-valid", function () {
            var target = $(this).attr("data-target");
            jQuery(".field-validation-error").html("");
            $(target).html("");
        });
    },
    ClickUploadAvar: function () {
        jQuery(document).on("change", "#File", function () {
            //var self = $(this);
            //var form = self.closest("form");

            var input = document.getElementById("File");
            //var files = input.files;
            //var formData = new FormData();
            var _validFileExtensions = [".jpg", ".jpeg", ".png"];

            //Change avatar
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                var oInput = input.files[0];
                var sFileName = oInput.name;
                var blnValid = false;
                if (sFileName.length > 0) {
                    for (var j = 0; j < _validFileExtensions.length; j++) {
                        var sCurExtension = _validFileExtensions[j];
                        if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                            blnValid = true;
                            break;
                        }
                    }

                    if (!blnValid) {
                        document.getElementById("File").value = "";
                        alert("Xin lỗi, " + sFileName + " không đúng định dạng. Định dạng hợp lệ: " + _validFileExtensions.join(", "));
                    }
                }
                if (blnValid) {
                    reader.onload = function (e) {
                        $("#imgProfile").attr("src", e.target.result);
                    }

                    reader.readAsDataURL(input.files[0]); // convert to base64 string
                }
            }

            //for (var i = 0; i != files.length; i++) {
            //    formData.append("File", files[i]);
            //}

            //let url = self.attr("data-url");
            //$.ajax(
            //    {
            //        url: url,
            //        data: formData,
            //        processData: false,
            //        contentType: false,
            //        type: "POST",
            //        success: function (res) {
            //            if (res != null) {
            //                jQuery(form.attr("data-target")).attr("src", res.data.data.physicalPath);
            //                CommonJs.SetMessageNotRefresh(res, true);
            //            }
            //        }
            //    }
            //);

            //CommonJs.CustAjaxCall(formData, "POST", url, dataType, ssCallBack, errCallback);
            //return false;
        });
    },
    ClickUpdateProfile: function () {
        jQuery(document).on("click", ".quickSubmitUpdateProfile", function () {
            var self = $(this);

            var input = document.getElementById("File");
            var AccountName = document.getElementById("AccountName").value;
            var Name = document.getElementById("Name").value;
            var Email = document.getElementById("Email").value;
            var Phone = document.getElementById("Phone").value;
            var Address = document.getElementById("Address").value;
            var IdentityNumber = document.getElementById("IdentityNumber").value
            var srcImage = document.getElementById("imgProfile").value;
            var File = input.files;
            var formData = new FormData();

            for (var i = 0; i != File.length; i++) {
                formData.append("File", File[i]);
            }

            formData.append("AccountName", AccountName);
            formData.append("Name", Name);
            formData.append("Email", Email);
            formData.append("Phone", Phone);
            formData.append("Address", Address);
            formData.append("IdentityNumber", IdentityNumber);

            let url = self.attr("data-url");
            $.ajax(
                {
                    url: url,
                    data: formData,
                    processData: false,
                    contentType: false,
                    type: "POST",
                    success: function (res) {
                        CommonJs.SetMessage(res, true);
                    }
                }
            );

            //CommonJs.CustAjaxCall(formData, "POST", url, dataType, ssCallBack, errCallback);
            return false;
        });
    },
    ClickAvatar: function () {
        jQuery(document).on("click", ".avatar", function () {
            $("#File").trigger("click");
        });
    },

    InitUploadLargeFile: function () {

    }
};