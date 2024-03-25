var AgencyConfig = {
    OpenPopup: function () {
        $("#planCreateForm .select2").select2({
            width: '.form-group',
            allowClear: true,
            placeholder: function () {
                $(this).data('placeholder');
            },
            language: "vi"
        });
    },
    GetAgencysByOrganId: function (listOrganId, listId, isMultiple, idName, idWrapper) {
        var OrganIds = [];
        if (listOrganId != null && listOrganId != undefined && listOrganId != "" && (listOrganId.length > 0 || listOrganId > 0)) {
            if (Array.isArray(listOrganId))
                $.each(listOrganId, function (index, value) {
                    OrganIds.push(parseInt(value));
                });
            else
                OrganIds.push(parseInt(listOrganId));
        }
        var ids = [];
        if (listId != null && listId != undefined && listId != "" && (listId.length > 0 || listId > 0)) {
            if (Array.isArray(listId))
                $.each(listId, function (index, value) {
                    ids.push(parseInt(value));
                });
            else
                ids.push(parseInt(listId));
        }

        let method = "POST";
        let url = "/Agency/GetByOrganId";
        let someData = { OrganIds: OrganIds, ids: ids, idName: idName, idWrapper: idWrapper };
        let ssCallback = function (data) {
            //bind data
            $("#" + idWrapper).html(data);
            //check isMultiple before init event select2
            if (isMultiple)
                $("#" + idName).prop("multiple", "multiple");
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
            if (ids.length == 0)
                $("#" + idName).val('').trigger('change');
        }
        CommonJs.CustAjaxCall(someData, method, url, "", ssCallback, "");
    },
    ExpandTable: function () {
        $(".expandable-table").click(function () {
            var expandableBody = $(this).next();
            if (expandableBody.hasClass('expandable-body') && $.trim(expandableBody.html()) != '') {
                return;
            } else {
                let tableId = $(this).attr("id");
                let id = tableId.replace("expandable-", "");
                let url = "/Agency/GetHierachyAgency?id=" + id;
                let ssCallBack = function (data) {
                    expandableBody.append(data);
                    AgencyConfig.ExpandTable();
                };
                let errCallback = function () {
                    return false;
                }

                CommonJs.CustAjaxCall({}, "POST", url, "", ssCallBack, errCallback);
            }
        });
    }
};