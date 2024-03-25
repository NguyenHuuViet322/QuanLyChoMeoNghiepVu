var BackupAndRestoreConfigs = {
    ClickRestoreButton: function () {
        jQuery(document).on("click", ".restore-button", function () {
            $("#File").trigger("click");
        });
    },

    RestoreAction: function () {
        jQuery(document).on("change", "#File", function () {
            var self = $(this);
            var form = self.closest("form");

            var input = document.getElementById("File");
            var files = input.files;
            var formData = new FormData();

            for (var i = 0; i != files.length; i++) {
                formData.append("File", files[i]);
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
                        if (res != null) {
                            CommonJs.SetMessage(res, true);
                        }
                    }
                }
            );

            CommonJs.CustAjaxCall(formData, "POST", url, dataType, ssCallBack, errCallback);
            return false;
        });
    },
};