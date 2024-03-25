var ProfileListConfig = {
    Init: function () {
        ProfileListConfig.GetProfileTemplateByIDStorage($('#Storages').val(), $('#ProfileTemplates').val(), true, "ProfileTemplates", "elmBindingProfileTemplates");
        $("#Storages").change(function () {
            ProfileListConfig.GetProfileTemplateByIDStorage($(this).val(), $("#ProfileTemplates").val(), true, "ProfileTemplates", "elmBindingProfileTemplates");
        });
    },
    OpenPopup: function () {
        CommonJs.Select2Init();
        $("#IDStorage").change(function () {
            var storageIds = [];
            storageIds.push($(this).val());
            ProfileListConfig.GetProfileTemplateByIDStorage(storageIds, "", false, "IDProfileTemplate", "elmBindingIDProfileTemplate");
        });
    },
    GetProfileTemplateByIDStorage: function (listStorageId, listId, isMultiple, idName, idWrapper) {
        var storageIds = [];
        if (listStorageId != null && listStorageId != undefined && listStorageId != "" && listStorageId.length > 0) {
            if (Array.isArray(listStorageId)) {
                $.each(listStorageId, function (index, value) {
                    storageIds.push(parseInt(value))
                })

            } else {
                storageIds.push(parseInt(listStorageId));
            }
        }

        var ids = [];
        if (listId != null && listId != undefined && listId != "" && listId.length > 0) {
            $.each(listId, function (index, value) {
                ids.push(parseInt(value));
            })
        }

        let method = 'POST';
        let url = "/ProfileList/GetProfileTemplateByIdStorage";
        let someData = { storageIDs: storageIds, ids: ids, idName: idName, idWrapper };
        let ssCallback = function (data) {
            //bind data
            $("#" + idWrapper).html(data);
            if (isMultiple)
                $("#" + idName).prop("multiple", "multiple");
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
    }
};