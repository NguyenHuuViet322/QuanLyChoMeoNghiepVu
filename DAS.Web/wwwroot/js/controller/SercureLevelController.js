var SercureLevelConfig = {
    SearchSercureLevel: function (pageIndex) {
        $("#hddPageIndex").val(pageIndex);
        let Keyword = $("#Keyword").val();

        let condition = {
            Keyword: Keyword,
            PageIndex: pageIndex,
            PageSize: $("#drdPageSize").val()
        };
        let method = "POST";
        let url = "/SercureLevel/SearchByConditionPagging";
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
        $("#SearchSercureLevel").click(function () {
            SercureLevelConfig.SearchSercureLevel(1);
        });
        $("#btnCreate").click(function () {
            let url = "/SercureLevel/CreatePopup";
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
            let url = "/SercureLevel/EditPopup/" + id;
            let ssCallback = function (data) {
                $(".popup-modal > .modal-dialog").empty();
                $("#planEditModal > .modal-dialog").html(data);
            }
            CommonJs.CustAjaxCall({}, "POST", url, "", ssCallback, "");
        });
        $(".btn-detail").click(function () {
            let id = $(this).attr("data-id");
            let url = "/SercureLevel/DetailPopup/" + id;
            let ssCallback = function (data) {
                $(".popup-modal > .modal-dialog").empty();
                $("#planDetailModal > .modal-dialog").html(data);
            }
            CommonJs.CustAjaxCall({}, "POST", url, "", ssCallback, "");
        })
        SercureLevelConfig.DeleteSercureLevel();
    },
    DeleteSercureLevel: function () {
        $(".DeleteSercureLevel").click(function () {
            let SercureLevelId = $(this).attr("data-id");
            CommonJs.ShowConfirmMsg('Bạn có chắc chắn muốn xóa cấp độ bảo mật này?'
                , 'Bạn không thể phục hồi dữ liệu đã xóa'
                , 'Xóa'
                , function () {
                    let someData = {};
                    let method = "POST";
                    let url = "/SercureLevel/Delete/" + SercureLevelId;
                    let ssCallBack = function (res) {
                        if (typeof res === "object" && typeof res.type !== undefined) {
                            if (res.type === "Success") {
                                CommonJs.ShowNotifyMsg(SwalMsgType.success, res.message);
                                SercureLevelConfig.SearchSercureLevel($("#hddPageIndex").val());
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
    DeleteMultiSercureLevels: function () {
        $(".DeleteMultiSercureLevels").click(function () {
            let SercureLevelIdStrs = "";
            $("tbody :checkbox").each(function () {
                if ($(this).is(":checked")) {
                    SercureLevelIdStrs += $(this).attr("id").replace("sercurelevel-", "") + ",";
                }
            });
            if (SercureLevelIdStrs == "")
                return false;
            else
                console.log(SercureLevelIdStrs);
            CommonJs.ShowConfirmMsg('Bạn có chắc chắn muốn xóa các cấp độ bảo mật này?'
                , 'Bạn không thể phục hồi dữ liệu đã xóa'
                , 'Xóa'
                , function () {
                    let someData = { idStr: SercureLevelIdStrs.substring(0, SercureLevelIdStrs.length - 1) };
                    let method = "POST";
                    let url = "/SercureLevel/DeleteMulti";
                    let ssCallBack = function (res) {
                        if (typeof res === "object" && typeof res.type !== undefined) {
                            if (res.type === "Success") {
                                CommonJs.ShowNotifyMsg(SwalMsgType.success, res.message);
                                SercureLevelConfig.SearchSercureLevel($("#hddPageIndex").val());
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
    }
};

var InitSercureLevelConfig = function () {
    SercureLevelConfig.DeleteMultiSercureLevels();
    SercureLevelConfig.SearchSercureLevel(1);
    SercureLevelConfig.ActionBeforeLoadListByAjax();
}