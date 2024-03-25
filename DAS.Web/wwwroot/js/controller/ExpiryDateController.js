var ExpiryDateConfig = {
    SearchExpiryDate: function (pageIndex) {
        $("#hddPageIndex").val(pageIndex);
        let Keyword = $("#Keyword").val();

        let condition = {
            Keyword: Keyword,
            PageIndex: pageIndex,
            PageSize: $("#drdPageSize").val()
        };
        let method = "POST";
        let url = "/ExpiryDate/SearchByConditionPagging";
        let ssCallback = function (data) {
            $("#ContentAreaTable").html(data);
            $('#drdPageSize').select2({
                width: 60,
                minimumResultsForSearch: 'Infinity',
                language: "vi"
            });
        }
        CommonJs.CustAjaxCall(condition, method, url, "", ssCallback, "");
        return false;
    },
    ActionBeforeLoadListByAjax: function () {
        $("#SearchExpiryDate").click(function () {
            ExpiryDateConfig.SearchExpiryDate(1);
        });
        $("#btnCreate").click(function () {
            let url = "/ExpiryDate/CreatePopup";
            let ssCallback = function (data) {
                $(".popup-modal > .modal-dialog").empty();
                $("#planCreateModal > .modal-dialog").html(data);
            }
            CommonJs.CustAjaxCall({}, "POST", url, "", ssCallback, "");
        });
    },
    ActionAfterLoadListByAjax: function () {
        $(".btn-edit").click(function () {
            let id = $(this).attr("data-id");
            let url = "/ExpiryDate/EditPopup/" + id;
            let ssCallback = function (data) {
                $(".popup-modal > .modal-dialog").empty();
                $("#planEditModal > .modal-dialog").html(data);
            }
            CommonJs.CustAjaxCall({}, "POST", url, "", ssCallback, "");
        });
        $(".btn-detail").click(function () {
            let id = $(this).attr("data-id");
            let url = "/ExpiryDate/DetailPopup/" + id;
            let ssCallback = function (data) {
                $(".popup-modal > .modal-dialog").empty();
                $("#planDetailModal > .modal-dialog").html(data);
            }
            CommonJs.CustAjaxCall({}, "POST", url, "", ssCallback, "");
        })
        ExpiryDateConfig.DeleteExpiryDate();
    },
    DeleteExpiryDate: function () {
        $(".DeleteExpiryDate").click(function () {
            let ExpiryDateId = $(this).attr("data-id");
            CommonJs.ShowConfirmMsg('Bạn có chắc chắn muốn xóa thời hạn bảo quản này?'
                , 'Bạn không thể phục hồi dữ liệu đã xóa'
                , 'Xóa'
                , function () {
                    let someData = {};
                    let method = "POST";
                    let url = "/ExpiryDate/Delete/" + ExpiryDateId;
                    let ssCallBack = function (res) {
                        if (typeof res === "object" && typeof res.type !== undefined) {
                            if (res.type === "Success") {
                                CommonJs.ShowNotifyMsg(SwalMsgType.success, res.message);
                                ExpiryDateConfig.SearchExpiryDate($("#hddPageIndex").val());
                            }
                            else if (res.Type === "Error")
                                CommonJs.ShowNotifyMsg(SwalMsgType.error, res.message);
                            else if (res.Type === "Warning")
                                CommonJs.ShowNotifyMsg(SwalMsgType.warning, res.message);
                        }
                    };
                    CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                });
            return false;
        })
    },
    DeleteMultiExpiryDates: function () {
        $(".DeleteMultiExpiryDates").click(function () {
            let ExpiryDateIdStrs = "";
            $("tbody :checkbox").each(function () {
                if ($(this).is(":checked")) {
                    ExpiryDateIdStrs += $(this).attr("id").replace("expirydate-", "") + ",";
                }
            });
            if (ExpiryDateIdStrs == "")
                return false;
            else
                console.log(ExpiryDateIdStrs);
            CommonJs.ShowConfirmMsg('Bạn có chắc chắn muốn xóa các thời hạn bảo quản này?'
                , 'Bạn không thể phục hồi dữ liệu đã xóa'
                , 'Xóa'
                , function () {
                    let someData = { idStr: ExpiryDateIdStrs.substring(0, ExpiryDateIdStrs.length - 1) };
                    let method = "POST";
                    let url = "/ExpiryDate/DeleteMulti";
                    let ssCallBack = function (res) {
                        if (typeof res === "object" && typeof res.type !== undefined) {
                            if (res.type === "Success") {
                                CommonJs.ShowNotifyMsg(SwalMsgType.success, res.message);
                                ExpiryDateConfig.SearchExpiryDate($("#hddPageIndex").val());
                            }
                            else if (res.Type === "Error")
                                CommonJs.ShowNotifyMsg(SwalMsgType.error, res.message);
                            else if (res.Type === "Warning")
                                CommonJs.ShowNotifyMsg(SwalMsgType.warning, res.message);
                        }
                    };
                    CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                });
            return false;
        })
    },
    OpenPopup: function () {
        $(".onlynumber").ForceNumericOnly();
        $("#Value").on("change", function () {
            let value = parseInt($("#Value").val());
            let max = parseInt($("#MaxValueExpiryDate").val());
            if (value <= 0)
                $("#Name").val("");
            else if (value >= max)
                $("#Name").val("Vĩnh viễn");
            else
                $("#Name").val(value + " năm");
        });
    }
};

var InitExpiryDateConfig = function () {
    ExpiryDateConfig.DeleteMultiExpiryDates();
    ExpiryDateConfig.SearchExpiryDate(1);
    ExpiryDateConfig.ActionBeforeLoadListByAjax();
}