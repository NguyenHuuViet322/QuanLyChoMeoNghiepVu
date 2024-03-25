var BorrowConfig = {
    Init: function () {
        jQuery(document).on("click", ".quickRequestBorrow", function () {
            var self = $(this);
            var form = self.closest("form");
            //Clear old message
            jQuery(".field-validation-valid").html("");

            let data = CommonJs.GetSerialize(form); //trimspace
            var index = 0;
            jQuery(this)
                .closest(".dataTables_wrapper").find(".cbDocs").each(function () {
                    if (jQuery(this).prop("checked")) {
                        var id = jQuery(this).attr("data-id");
                        if (CommonJs.IsInteger(id)) {
                            var name = jQuery(this).attr("name").replace("[n]", "[" + index + "]");
                            data[name] = id;
                            jQuery(this).parent().find("input[type='hidden']").each(function () {
                                name = jQuery(this).attr("name").replace("[n]", "[" + index + "]");
                                data[name] = jQuery(this).val();
                            });
                        }
                        index++;
                    }
                });
            let url = self.attr("data-url");
            let resUrl = self.attr("data-resurl");
            let dataType = "json"
            let ssCallBack = function (res) {
                if (res.type === "Success") {
                    if (res.data && res.data.url) {
                        window.open(res.data.url);//Close model when success
                    }
                    else {

                        form.closest('.modal').modal('toggle');//Close model when success
                        if (resUrl != '' && resUrl != undefined)
                            setTimeout(function () {
                                window.location = resUrl;
                            }, 1000);
                        CommonJs.SetMessage(res, true, resUrl);

                    }
                } else {
                    CommonJs.SetMessage(res, true, resUrl);
                }


            }
            //todo : handle with errCallback
            let errCallback = function () {
                return false;
            }

            CommonJs.CustAjaxCall(data, "POST", url, dataType, ssCallBack, errCallback);
            return false;
        });

        jQuery(document).on("click", ".quickBorrow", function () { 
            let btn = $(this);
            let url = btn.attr("data-href");
            if (CommonJs.IsEmpty(url))
                url = btn.attr("href");
            let target = btn.attr("data-target");;
            var data = btn.getDataUppername();
            var method = btn.attr("data-method") || "GET";
            if (!btn.hasClass("clicked")) {
                btn.addClass("clicked");
                jQuery.ajax({
                    type: method,
                    async: true,
                    url: url,
                    data: data,
                    error: function () {
                        btn.removeClass("clicked");
                    },
                    complete: function () {
                        btn.removeClass("clicked");
                    },
                    success: function (rs) {
                        btn.removeClass("clicked");
                        if (rs.data != undefined && rs.data.isComfirm) {
                            var confirmStr = rs.message;
                            let confirmHeader = confirmStr.substring(0, confirmStr.indexOf(':') + 1).trim();
                            confirmMessage = confirmStr.substring(confirmStr.indexOf(':') + 1).trim();
                            if (confirmHeader === '') {
                                confirmHeader = confirmMessage;
                                confirmMessage = '';
                            }
                            CommonJs.ShowConfirmMsg2(
                                confirmHeader,
                                confirmMessage,
                                'Đồng ý',
                                'Hủy',
                                function () {
                                    window.location = rs.data.redirectUrl;
                                });

                            return false;
                        }
                        if (rs.type && rs.type.toLowerCase() == "error") {
                            CommonJs.SetMessage(rs, false, true);
                            return false;
                        }
                        if (rs.type != undefined && rs.data != undefined && rs.data) {
                            var idDocs = rs.data.cartValue;
                            for (var i = 0; i < idDocs.length; i++) {
                                jQuery(".quickBorrow.isDoc[data-id='" + idDocs[i] + "']").remove();
                            }
                            if (jQuery(".quickBorrow.isDoc").length == 0) {
                                jQuery(".quickBorrow.isProfile").remove();
                            }
                            var cart = jQuery(".cart-count").attr("data-value", idDocs.length).text(idDocs.length).parent().addClass("shake");
                            setTimeout(function () {
                                cart.removeClass('shake');
                            }, 300);
                            CommonJs.SetMessage(rs, false, true);
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
            }
            return false;
        });

        jQuery(document).on("click", ".onDeleteCartItem", function () {
            var obj = jQuery(this);
            var url = obj.attr("data-url");
            var isReindex = obj.attr("data-reindex") === "1";
            var tr = obj.closest(".item");
            var tbody = obj.closest("tbody");
            var cIndex = tr.index();
            var friends = tr.siblings("tr");
            var data = { id: obj.attr("data-id") };
            var parent = obj.attr("data-parent");
            var group = obj.attr("data-group");
            var trGroup = jQuery(obj.attr("data-tr-group"));
            jQuery.ajax({
                type: "POST",
                async: true,
                url: url,
                data: data,
                success: function (rs) {
                    if (rs.type != undefined && rs.data != undefined && rs.data) {

                        if (tr.length > 0)
                            tr.remove();

                        CommonJs.SetMessage(rs, false, true);
                        var idDocs = rs.data.cartValue;
                        jQuery(".cart-count").attr("data-value", idDocs.length).text(idDocs.length);

                        if (idDocs.length == 0)
                            jQuery(".spTotal").text(jQuery(".spTotal").attr("data-msg-empty"));
                        else
                            jQuery(".totalByCart").text(idDocs.length);

                        var totalByGroup = tbody.find("tr.item[data-group='" + group + "']").length;

                        if (totalByGroup > 0) trGroup.find(".totalByGroup").text(totalByGroup);
                        else {
                            trGroup.next(".expandable-body").remove();
                            trGroup.remove();
                        }

                        if (parent) //xoa dong hien tai, hien dong dau tien cung cha (Fix truong hop xoa tr khi rowspan)
                        {
                            var frs = tbody.find("tr[data-parent='" + parent + "']"); //tr cung cha

                            if (frs.length > 0)
                                tbody.find("tr[data-parent='" + parent + "']").each(function (i, tr) {

                                    var rowSpan = frs.length;
                                    if (rowSpan < 0)
                                        rowSpan = 1;

                                    if (i == 0) {
                                        jQuery(tr).find(".colProfile").removeClass("hidden").attr("rowspan", rowSpan);
                                    } else {
                                        jQuery(tr).find(".colProfile").addClass("hidden").attr("rowspan", rowSpan);
                                    }
                                });
                        }


                        if (isReindex && friends.length > 0) {
                            //Đánh lại index
                            const regex = /\[[0-9]+\]/gm;
                            jQuery(tbody).find("select.select2-hidden-accessible").select2('destroy');
                            friends.each(function (i, row) {
                                if (i >= cIndex) {
                                    jQuery(row).find("input,select").each(function () {
                                        var type = jQuery(this).attr("type");
                                        var tagName = jQuery(this).prop("tagName").toLowerCase();
                                        if (type == "checkbox" || type == "radio") {
                                            jQuery(this).prop("checked", jQuery(this).prop("checked"));
                                            jQuery(this).attr("checked", jQuery(this).prop("checked"))
                                        }
                                        else if (tagName == "select") {
                                            jQuery(this).find("option[value='" + jQuery(this).val() + "']").attr("selected", "true");
                                        }
                                        else {
                                            jQuery(this).attr("value", jQuery(this).val());
                                        }
                                    });

                                    jQuery(row).html(jQuery(row).html().replace(regex, "[" + i + "]"));
                                }
                            });
                            CommonJs.UpdateSelect2(jQuery(tbody));
                        }
                    };
                }
            });



            return false;
        });

        jQuery(document).on("change", ".cbDocs", function () {
            var cb = jQuery(this);
            BorrowConfig.ToggleParentCheckboxes(cb);
            BorrowConfig.ToggleParentCheckboxes(jQuery(cb.attr("data-cb-parent")));
        });
        jQuery(document).on("change", ".cbOrgans", function () {
            var cb = jQuery(this);
            BorrowConfig.ToggleChildCheckboxes(cb);
            BorrowConfig.ToggleParentCheckboxes(cb);
        });
        jQuery(document).on("change", ".cbAll", function () {
            var cb = jQuery(this);
            BorrowConfig.ToggleChildCheckboxes(cb);
        });
        jQuery(document).on("change", ".onChangeBorrowType", function () {
            var obj = jQuery(this);
            var name = obj.attr("name");
            var url = location.protocol + '//' + location.host + location.pathname;
            window.location = CommonJs.UpdateQueryStringParameter(url, name, obj.val());
            return false;
        });

        jQuery(document).on("change", ".onSwitchDiv", function () {
            var obj = jQuery(this);
            var form = obj.closest("form");
            form.find(obj.attr("data-target")).addClass("hidden");
            form.find(obj.attr("data-target") + "[data-selected-id]").each(function () {
                if (jQuery(this).attr("data-selected-id").split(",").includes(obj.val())) {
                    jQuery(this).removeClass("hidden"); //hiển thị 
                }
            });
        });
    },
    ToggleParentCheckboxes: function (cb) {
        var parent = jQuery(cb.attr("data-cb-parent"));
        if (parent) {
            var flag = true;
            cb.closest("tbody").find(".cbItems").each(function () {
                if (!jQuery(this).prop("checked")) {
                    flag = false;
                    return false;
                }
            });
            parent.prop("checked", flag);
        }
    },
    ToggleChildCheckboxes: function (cb) {
        var isCheck = cb.prop("checked");
        var chids = jQuery("input[type='checkbox'][data-cb-parent='#" + cb.attr("id") + "']");
        if (chids.length > 0)
            chids.each(function () {
                jQuery(this).prop("checked", isCheck);
                BorrowConfig.ToggleChildCheckboxes(jQuery(this));
            });
    }
};