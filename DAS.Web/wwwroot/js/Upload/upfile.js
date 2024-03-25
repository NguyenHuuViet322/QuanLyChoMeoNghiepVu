var Uploader = {
    init: function () {
        Uploader.onEvent();
        Uploader.upEvent();
    },
    onEvent: function () {
        jQuery(document).on("click", ".attachFile", function () {
            let el = $(this);
            let rel = $(el.attr("data-rel"));
            rel.val("").trigger("click");

        });

        jQuery(document).on("click", ".onRemoveFile", function (e) {
            e.preventDefault();
            $(this).closest(".file-item").remove();
        });
    },
    upEvent: function (container) {
        if (!container)
            container = jQuery(document);

        container.find(".inputUpFile").each(function (e) {
            let el = jQuery(this);

            if (!el.hasClass("setUpFiled")) {
                el.hasClass("setUpFiled");
                el.ajaxUploader({
                    name: "file",
                    postUrl: "/Uploader/UploadFile",
                    onBegin: function (e, t) {
                        return true;
                    },
                    onClientLoadStart: function (e, file, t) {
                    },
                    onClientProgress: function (e, file, t) {
                    },
                    onServerProgress: function (e, file, t) {
                    },
                    onClientAbort: function (e, file, t) {
                    },
                    onClientError: function (e, file, t) {
                        jQuery(el.attr("data-target")).removeClass("loading");
                    },
                    onServerAbort: function (e, file, t) {
                        jQuery(el.attr("data-target")).removeClass("loading");
                    },
                    onServerError: function (e, file, t) {
                        jQuery(el.attr("data-target")).removeClass("loading");
                    },
                    onSuccess: function (e, file, t, res) {
                        CommonJs.ShowNotifyMsg(res.type, res.message);
                        if (res.type === "Success" && res.data) {
                            let target = $(el.attr("data-target"));
                            let isMultiple = el.attr("multiple") != undefined;
                            let html;

                            let temp = el.attr("data-template");
                            temp = temp ? temp : "#tempFileResultDf"; //Mẫu truyền vào hoặc mặc định
                            temp = $(temp)
                            if (temp && temp.length > 0) {
                                let data = res.data;
                                html = $(temp).html();
                                html = html.replaceAll("{name}", data.fileName);
                                html = html.replaceAll("{path}", data.physicalPath);
                            }
                            html = $(html);
                            html.addClass("file-item"); //add class phục vụ cho xóa
                            if (isMultiple) {
                                target.append(html);
                            } else {
                                target.html(html);
                            }
                        }
                    }
                });
            }
        });
    },
};
jQuery(document).ready(function () {
    Uploader.init();
})