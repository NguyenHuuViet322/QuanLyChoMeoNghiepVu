var ProfileStoringConfig = {
    autoIndex: function (target) {
        var stt = 0;
        target.find(".spIndex").each(function () {
            stt++;
            jQuery(this).html(stt);
        });
    },
    onEvents: function () {
        jQuery(document).on("click", ".onSaveProfileStoring", function () {
            var self = $(this);
            var form = jQuery(self.attr("data-form"));
            //Clear old message
            jQuery(".field-validation-valid").html("");
            let data = CommonJs.GetSerialize2(form); //trimspace
            let url = self.attr("data-url");
            let ssCallBack = function (res) {
                CommonJs.SetMessage(res, true, true);
                if (res.type === "success") {
                    jQuery(".temp").removeClass("temp");
                }
            }
            jQuery.ajax({
                type: "POST",
                async: true,
                url: url,
                data: data,
                success: ssCallBack
            });
        });

        jQuery(document).on("change", ".onAutoSearch", function () {
            var obj = $(this);
            var form = obj.closest("form");
            var willDisableSave = $('#slBox').val() == 0 && $(".temp").length == 0;
            var storage = function () {
                var str = '';
                str += $('#slStorage').val() > 0 ? $('#slStorage option:selected').text() + ' / ' : '';
                str += $('#slShelve').val() > 0 ? $('#slShelve option:selected').text() + ' / ' : '';
                str += $('#slBox').val() > 0 ? $('#slBox option:selected').text() : '';
                return str;
            }
            CommonJs.QuickActions();
            form.trigger("submit");
            $('#dropdownMenuButton').dropdown('hide');
            $('.storage').removeClass('text-danger').addClass('text-primary').text(storage);
            //Set disable for save btn
            jQuery(".onSaveProfileStoring").attr("disabled", willDisableSave);
        });

        jQuery(document).on("click", ".quickSubmitProfileStoring", function () {
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
            let dataType = "json"
            let ssCallBack = function (res) {
                if (res.type === "Success") {
                    form.closest('.modal').modal('toggle');//Close model when success
                    jQuery("#slBox").append('<option value="' + res.data.id + '" >' + res.data.name + '</option>');
                    jQuery("#slBox").val(res.data.id);
                    CommonJs.UpdateSelect2(jQuery("#slBox"));
                }
                CommonJs.SetMessage(res, true, true);
            }
            //todo : handle with errCallback
            let errCallback = function () {
                return false;
            }

            CommonJs.CustAjaxCall(data, "POST", url, dataType, ssCallBack, errCallback);
            return false;
        });

        jQuery(document).on("click", ".onMoveTr", function () {
            var obj = $(this);
            var tbFrom = jQuery(obj.attr("data-tb-from"));
            var tbTo = jQuery(obj.attr("data-tb-to"));
            var idTarget = tbTo.attr("data-id");
            var willDisableSave = $('#slBox').val() == 0 && $(".temp").length == 0;

            tbFrom.find(".checkboxes:checked").each(function () {
                var tr = jQuery(this).closest("tr");
                tr.find(".idItem").attr("name", tr.find(".idItem").attr("data-org-name") + idTarget);
                tbTo.find("tbody").prepend(tr);
                tr.toggleClass("temp");
                tr.hide().fadeIn("slow");
            });

            tbFrom.find(".checkboxes,.group-checkable").prop("checked", false);
            tbTo.find(".checkboxes,.group-checkable").prop("checked", false);

            ProfileStoringConfig.autoIndex($(tbFrom));
            ProfileStoringConfig.autoIndex($(tbTo));

            //Set disable for save btn
            jQuery(".onSaveProfileStoring").attr("disabled", willDisableSave);
            return false;
        });

        jQuery(document).on("submit", ".quickSearchProfileStoring", function () {
            let form = $(this);
            let url = form.attr("action");
            let method = form.attr("method");
            let target = form.attr("data-target");
            let target2 = form.attr("data-target2");
            let data = CommonJs.GetSerialize2(form);
            var temps = $(target).find(".temp");

            var ids = [];
            jQuery(target2).find(".temp").each(function () {
                var id = jQuery(this).data("id");
                ids.push(id);
            });
            if (ids.length > 0)
                data.IDs = JSON.stringify(ids);

            jQuery.ajax({
                type: method,
                async: true,
                url: url,
                data: data,
                beforeSend: function () {
                    $(target).html('');
                },
                success: function (rs) {
                    try {
                        if (form.attr("data-state") != "0") {
                            window.history.pushState(null, "", CommonJs.FormBuilderQString(form));
                            //jQuery(document).prop("title", rs.title);
                        }
                    } catch (e) {
                        console.log(e);
                    }
                    CommonJs.ToggleMultiTicks(jQuery(target));


                    $(target).html(rs);
                    $(target).find("tbody").prepend(temps);

                    ProfileStoringConfig.autoIndex($(target));

                    CommonJs.Select2Init($(target));
                    CommonJs.UpdateTreeGrid();
                }
            });
            return false;
        });

        jQuery(document).on("click", ".quickUpdateProfileStoring", function () {
            let btn = $(this);
            let url = btn.attr("data-href");
            if (CommonJs.IsEmpty(url))
                url = btn.attr("href");
            let target = btn.attr("data-target");;
            var data = btn.getDataUppername();
            var method = btn.attr("data-method") || "GET";

            data.IDStorage = jQuery("#slStorage").val();
            data.IDShelve = jQuery("#slShelve").val();

            jQuery.ajax({
                type: method,
                async: true,
                url: url,
                data: data,
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.SetMessage(rs);
                        return false;
                    };
                    $(target).html(rs);
                    CommonJs.UpdateFormState($(target));
                    CommonJs.UpdateInputMoney($(target));
                    CommonJs.UpdateIsNumber($(target));
                    CommonJs.DateTimeFormat();
                    CommonJs.Select2Init();
                    $(target).modal({
                        backdrop: 'static', //Click outside
                        //keyboard: true, //Esc
                        show: true
                    });
                }
            });
            return false;
        });

    }
};

var InitProfileStoring = function () {
    ProfileStoringConfig.onEvents();
}