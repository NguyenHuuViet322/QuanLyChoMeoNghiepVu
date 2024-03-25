var ElectronicSignConfig = {

    SignFileCallBack: function (rv) {
        window.location.reload();
        var received_msg = JSON.parse(rv);
        console.log(received_msg);
        if (received_msg.Status == 0) {
            //console.log(received_msg);
            document.getElementById("_signature").value = received_msg.FileName + ":" + received_msg.FileServer + ":" + received_msg.DocumentNumber + ":" + received_msg.DocumentDate;
            document.getElementById("file1").value = received_msg.FileServer;
            document.getElementById("file2").value = received_msg.FileServer;
        } else {
            document.getElementById("_signature").value = received_msg.Message;
        }
    },
    Signature: function () {
        //Lãnh đạo ký phê duyệt
        jQuery(document).on("click", "#_lanhdaoPheduyet", function () {
            var self = $(this);
            let rel = "/ReceiveArchive/ElectronicSign";
            var id = self.data("id");
            jQuery.ajax({
                type: "GET",
                async: true,
                url: rel,
                data: { id: id },
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.SetMessage(rs);
                        return false;
                    };

                    prms = {};
                    prms["FileUploadHandler"] = document.location.origin + "/ReceiveArchive/Upload";
                    prms["SessionId"] = "";
                    prms["FileName"] = document.location.origin + "/ReceiveArchive/ElectronicSign/" + id;

                    var json_prms = JSON.stringify(prms);
                    vgca_sign_approved(json_prms, ElectronicSignConfig.SignFileCallBack);
                }
            });
            return false;
        });

        //Đóng dấu phát hành
        jQuery(document).on("click", "#_vanthuphathanh", function () {
            var self = $(this);
            let rel = "/ReceiveArchive/ElectronicSign";
            var id = self.data("id");
            jQuery.ajax({
                type: "POST",
                async: true,
                url: rel,
                data: { id: id },
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.SetMessage(rs);
                        return false;
                    };

                    var prms = {};

                    prms["FileUploadHandler"] = "";
                    prms["SessionId"] = "";
                    prms["FileName"] = document.getElementById("file1").value; //"http://localhost:16227/files/test1.pdf";
                    prms["DocNumber"] = "123";
                    prms["IssuedDate"] = "2019-03-12T12:00:00+07:00";

                    var json_prms = JSON.stringify(prms);
                    vgca_sign_issued(json_prms, SignFileCallBack1);
                }
            });
            return false;
        });

        //Ký số công văn đến
        jQuery(document).on("click", "#_vanthuphathanh", function () {
            var self = $(this);
            let rel = "/ReceiveArchive/ElectronicSign";
            var id = self.data("id");
            jQuery.ajax({
                type: "POST",
                async: true,
                url: rel,
                data: { id: id },
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.SetMessage(rs);
                        return false;
                    };

                    var prms = {};
                    var scv = [{ "Key": "abc", "Value": "abc" }];

                    prms["FileUploadHandler"] = "";
                    prms["SessionId"] = "";
                    prms["FileName"] = url;
                    prms["MetaData"] = scv;

                    var json_prms = JSON.stringify(prms);
                    vgca_sign_income(json_prms, SignFileCallBack1);
                }
            });
            return false;

        });

        //Ký comment vào văn bản
        jQuery(document).on("click", "#_comment", function () {
            var self = $(this);
            let rel = "/ReceiveArchive/ElectronicSign";
            var id = self.data("id");
            jQuery.ajax({
                type: "POST",
                async: true,
                url: rel,
                data: { id: id },
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.SetMessage(rs);
                        return false;
                    };

                    var prms = {};
                    var scv = [{ "Key": "abc", "Value": "abc" }];

                    prms["FileUploadHandler"] = "";
                    prms["SessionId"] = "";
                    prms["FileName"] = url;
                    prms["MetaData"] = scv;

                    var json_prms = JSON.stringify(prms);
                    vgca_comment(json_prms, SignFileCallBack1);
                }
            });
            return false;

        });

        //Ký tài liệu đính kèm
        jQuery(document).on("click", "#_commentAttachment", function () {
            var self = $(this);
            let rel = "/ReceiveArchive/ElectronicSign";
            var id = self.data("id");
            jQuery.ajax({
                type: "POST",
                async: true,
                url: rel,
                data: { id: id },
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.SetMessage(rs);
                        return false;
                    };

                    var prms = {};
                    var scv = [{ "Key": "abc", "Value": "abc" }];

                    prms["FileUploadHandler"] = "";
                    prms["SessionId"] = "";
                    prms["FileName"] = url;
                    prms["DocNumber"] = "123/BCY-CTSBMTT";
                    prms["MetaData"] = scv;

                    var json_prms = JSON.stringify(prms);
                    vgca_sign_appendix(json_prms, SignFileCallBack1);
                }
            });
            return false;

        });

        //Ký số Bản sao điện tử
        jQuery(document).on("click", "#_Sacomment", function () {
            var self = $(this);
            let rel = "/ReceiveArchive/ElectronicSign";
            var id = self.data("id");
            jQuery.ajax({
                type: "POST",
                async: true,
                url: rel,
                data: { id: id },
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.SetMessage(rs);
                        return false;
                    };

                    var prms = {};
                    var scv = [{ "Key": "abc", "Value": "abc" }];

                    prms["FileUploadHandler"] = "";
                    prms["SessionId"] = "";
                    prms["FileName"] = url;
                    prms["DocNumber"] = "123/BCY-CTSBMTT";
                    prms["MetaData"] = scv;

                    var json_prms = JSON.stringify(prms);
                    vgca_sign_copy(json_prms, SignFileCallBack1);
                }
            });
            return false;
        });
    },

};