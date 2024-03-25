var TemplateConfig = {
    OnLeaveTextbox: function () {
        jQuery(document).on("change", ".textbox-for-login, .textbox-valid", function () {
            var target = $(this).attr("data-target");
            jQuery(".field-validation-error").html("");
            $(target).html("");
        });
    },
    ClickUpdateTemplate: function () {
        jQuery(document).on("click", ".quickSubmitTemplate", function () {
            var self = $(this);
            var form = self.closest("form");

            if (form.hasClass("validateForm")) {
                var bootstrapValidator = form.data('bootstrapValidator');
                bootstrapValidator.validate();
                if (!bootstrapValidator.isValid(true)) {
                    return false;
                }
            }
            jQuery(".field-validation-valid").html("");

            let data = CommonJs.GetSerialize(form); //trimspace

            var input = document.getElementById("File");
            //var Code = document.getElementById("Code").value;
            //var id = document.getElementById("ID");
            //var Name = document.getElementById("Name").value;
            //var Description = document.getElementById("Description").value;
            var File = input.files;

            for (var i = 0; i != File.length; i++) {
                data["File"] = File[i];
            }

            //if (id != undefined) {
            //    formData.append("ID", id.value);
            //}

            //formData.append("Code", Code);
            //formData.append("Name", Name);
            //formData.append("Description", Description);

            var formData = new FormData();
            for (var key in data) {
                formData.append(key, data[key]);
            }
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
    ClickUploadTemplate: function () {
        jQuery(document).on("change", "#File", function () {

            var input = document.getElementById("File");
            var _validFileExtensions = [".docx", ".DOCX"];

            //Check Input
            if (input.files && input.files[0]) {
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
            }
        });
    },
    ClickDownloadTemplate: function () {
        jQuery(document).on("click", ".download-template", function () {
            var self = $(this);
            let id = self.attr("data-id");
            let url = self.attr("data-url");
            var formData = new FormData();
            formData.append("id", id);
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
};