var PlanConfig = {
    Init: function () {
        $("#SearchPlan").click(function () {
            PlanConfig.SearchPlan(1);
        });
        PlanConfig.ExpandTable();        
    },
    OpenPopup: function () {
        $("#planCreateForm .select2").select2({
            width: '.form-group',
            allowClear: true,
            placeholder: function () {
                $(this).data('placeholder');
            },
            language: "vi"
        });
        $(".onlynumber").ForceNumericOnly();
    },
    SubmitForm: function () {
        $("#IsArchive").val($("#IsArchiveStr").is(":checked"));
        $(".quickSubmit").click();
    },
    ExpandTable: function () {
        $(".expandable-table").click(function () {
            var expandableBody = $(this).next();
            if (expandableBody.hasClass('expandable-body') && $.trim(expandableBody.html()) != '') {
                return;
            } else {
                let tableId = $(this).attr("id");
                let id = tableId.replace("expandable-", "");
                let url = "/Plan/GetHierachyPlan?id=" + id;
                let ssCallBack = function (data) {
                    expandableBody.append(data);
                    PlanConfig.ExpandTable();
                };
                let errCallback = function () {
                    return false;
                }

                CommonJs.CustAjaxCall({}, "POST", url, "", ssCallBack, errCallback);
            }
        });
    }
};