const SwalMsgType = {
    success: 'success',
    error: 'error',
    warning: 'warning',
    info: 'info',
    question: 'question',
};

var CommonJs = {
    CustAjaxCall: function (someData, method, url, datatype, ssCallBack, errCallback) {
        $.ajax({
            async: false,
            data: someData,
            method: method,
            dataType: datatype,//'json'
            url: url//'/controller/action/'
        }).done(function (data) {
            // If successful
            if (typeof (ssCallBack) === "function")
                ssCallBack(data);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // If fail
            alert(textStatus + ': ' + errorThrown);
            if (typeof (errCallback) === "function") {
                errCallback();
            }
        });
    },
    ShowModalExtra: function () {
        $("#modal-xl").modal('show');
    },
    ShowModalSm: function () {
        $("#modal-sm").modal('show');
    },
    //https://sweetalert2.github.io/
    ShowNotifyMsg: function (msgType, msgContent) {
        if (msgType != undefined)
            msgType = msgType.toLowerCase();
        let bgClass = "";
        if (msgType === "success")
            bgClass = 'bg-success';
        else if (msgType === "error")
            bgClass = 'bg-danger';
        else if (msgType === "warning")
            bgClass = 'bg-warning';
        else if (msgType === "info")
            bgClass = 'bg-info';

        if (!CommonJs.IsEmpty(bgClass)) {
            $(document).Toasts('create', {
                class: bgClass,
                autohide: true,//res.type === "Success",
                delay: 5000,
                position: 'bottomRight',
                title: "Thông báo",
                body: msgContent
            });
        }
        return false;
    },
    /**
     * Showing Sweetalert
     * @param {string} title Title of message box
     * @param {string} text Content of message box
     * @param {string} confirmText Text show on Confirm button
     * @param {string} cancelText Text show on Cancel button
     * @param {function} callbackFunc Callback funtion
     */
    ShowConfirmMsg: function (title, text, confirmText, cancelText, callbackFunc) {
        Swal.fire({
            titleText: title,
            text: text,
            icon: 'warning',
            showCancelButton: true,
            buttonsStyling: false,
            reverseButtons: true,
            focusConfirm: false,
            focusCancel: true,
            showCloseButton: true,
            confirmButtonText: '' + (confirmText || '<i class="fas fa-fw fa-trash-alt"></i> Xóa'),
            cancelButtonText: '' + (cancelText || '<i class="fas fa-fw fa-times"></i> Đóng'),
            customClass: {
                confirmButton: 'btn btn-outline-danger m-1',
                cancelButton: 'btn btn-outline-secondary m-1'
            },

        }).then((res) => {
            if (res.value) {
                if (typeof (callbackFunc) === "function") {
                    callbackFunc();
                }
            }
        })
    },
    ShowConfirmMsg2: function (title, text, confirmText, cancelText, callbackFunc, url) {
        Swal.fire({
            titleText: title,
            text: text,
            icon: 'warning',
            showCancelButton: true,
            buttonsStyling: false,
            reverseButtons: true,
            focusConfirm: false,
            focusCancel: true,
            showCloseButton: true,
            confirmButtonText: '<i class="fas fa-fw fa-check"></i>' + (confirmText || 'Đồng ý'),
            cancelButtonText: '<i class="fas fa-fw fa-times"></i>' + (cancelText || 'Đóng'),
            customClass: {
                confirmButton: 'btn btn-outline-primary m-1',
                cancelButton: 'btn btn-outline-secondary m-1'
            },
        }).then((res) => {
            if (res.value) {
                if (typeof (callbackFunc) === "function") {
                    callbackFunc();
                }
            }
        })
    },
    ShowConfirmMsgNote: function (title, text, confirmText, cancelText, callbackFunc) {
        Swal.fire({
            titleText: title,
            text: text,
            icon: 'warning',
            showCancelButton: true,
            buttonsStyling: false,
            reverseButtons: true,
            focusConfirm: false,
            focusCancel: true,
            showCloseButton: true,
            confirmButtonText: '<i class="fas fa-fw fa-check"></i>' + (confirmText || 'Đồng ý'),
            cancelButtonText: '<i class="fas fa-fw fa-times"></i>' + (cancelText || 'Đóng'),
            customClass: {
                confirmButton: 'btn btn-outline-primary m-1',
                cancelButton: 'btn btn-outline-secondary m-1'
            },
            input: 'textarea',
            inputAttributes: {
                autocapitalize: 'off',
                placeholder: 'Nhập lý do',
                rows: 3
            },
            preConfirm: (Reason) => {
                if (CommonJs.IsEmpty(Reason)) {
                    Swal.showValidationMessage('Vui lòng nhập lý do')
                }
                return Reason;
            },
        }).then((res) => {
            if (res.value) {
                if (typeof (callbackFunc) === "function") {
                    callbackFunc(res.value);
                }
            }
        })
    },

    QuickSubmitJS: function () {
        $(".QuickSubmitJS").click(function () {
            let self = $(this);
            let form = self.closest("form");
            let data = CommonJs.GetSerialize(form);
            let url = self.attr("data-url");
            let dataType = "json";
            let isReload = self.attr("isReloadAfterSubmit") !== "false";
            let ssCallBack = function (res) {
                if (typeof res === "object" && typeof res.type !== undefined) {
                    if (res.type === "Success") {
                        if (isReload) {
                            setTimeout(function () {
                                window.location.reload();
                            }, 3000);
                        }
                        CommonJs.ShowNotifyMsg(res.type, res.message);
                    }
                    else if (res.type === "Error" || res.type === "Warning") {
                        CommonJs.ShowNotifyMsg(res.type, res.message);
                    }
                }
            }

            //todo : handle with errCallback
            let errCallback = function () {
                return false;
            }

            CommonJs.CustAjaxCall(data, "POST", url, dataType, ssCallBack, errCallback);
            return false;
        });
    },
    QuickSubmitHTML: function () {
        $(".QuickSubmitHtml").click(function () {
            let self = $(this);
            let form = self.closest("form");
            let data = form.serializeArray();
            let nOb = {};
            for (let i in data) {
                if (!nOb.hasOwnProperty(data[i].name))
                    nOb[data[i].name] = "";
                nOb[data[i].name].push(data[i].value);
            }

            let url = self.attr("data-url");
            let dataType = "html"
            let dataTarget = self.attr("data-target");
            let ssCallBack = function (res) {
                $(dataTarget).html(res);
            }

            //todo : handle with errCallback
            let errCallback = function () {
                return;
            }

            CommonJs.CustAjaxCall(data, url, dataType, ssCallBack, errCallback);
        });
    },
    GetSerialize: function (form) {
        //remove disable to get data
        var disabled = form.find(':input:disabled').removeAttr('disabled');

        let data = form.serializeArray();

        disabled.attr('disabled', 'disabled'); //add disabled input

        let rs = {};
        for (let i in data) {
            if (!rs.hasOwnProperty(data[i].name))
                rs[data[i].name] = [];
            rs[data[i].name].push(data[i].value.trim());// Trim D
        }
        for (let i in rs) {
            if (rs[i].length === 1) {
                rs[i] = rs[i].join(",");
            }
            else {
                rs[i] = JSON.stringify(rs[i]);
            }
        }

        return rs;
    },
    GetSerialize2: function (form) {
        var keys = {};
        form.find("input, select, textarea,button").each(function () {
            var el = jQuery(this);
            var name = el.prop("name");
            if (!CommonJs.IsEmpty(name)) {
                var tagName = el.prop("tagName").toLowerCase();
                if (tagName == "input") {
                    var type = el.prop("type").toLowerCase();
                    if (type == "text" || type == "password" || type == "hidden" || type == "number" || type == "search") {
                        if (!keys.hasOwnProperty(name)) {
                            keys[name] = [];
                        }
                        if (!CommonJs.IsEmpty(el.val()))
                            el.val(el.val().trim());
                        keys[name].push(el.val());
                    } else if (type == "checkbox") {
                        if (!keys.hasOwnProperty(name)) {
                            keys[name] = [];
                        }
                        if (el.prop("checked")) {
                            keys[name].push(el.val());
                        }
                        else {
                            keys[name].push(0);
                        }
                        //if (!checkboxs.hasOwnProperty(name)) {
                        //    checkboxs[name] = 0;
                        //}
                        //checkboxs[name]++;
                    } else if (type == "radio") {
                        if (!keys.hasOwnProperty(name)) {
                            keys[name] = [];
                        }
                        if (el.prop("checked")) {
                            keys[name].push(el.val());
                        }
                    }
                } else if (tagName != "button") {
                    if (!keys.hasOwnProperty(name)) {
                        keys[name] = [];
                    }
                    keys[name].push(el.val());
                }
            }
        });
        for (var k in keys) {
            var vals = keys[k];
            if (vals.length == 1) { //|| !checkboxs.hasOwnProperty(k)
                keys[k] = vals.join(",");
            } else {
                keys[k] = JSON.stringify(vals);
            }
        }
        return keys;
    },
    GetSerialize3: function (form, outq) {
        var keys = {};
        form.find("input, select, textarea,button").each(function () {
            var el = jQuery(this);
            var name = el.prop("name");
            if (!CommonJs.IsEmpty(name)) {
                var tagName = el.prop("tagName").toLowerCase();
                if (tagName == "input") {
                    var type = el.prop("type").toLowerCase();
                    if (type == "text" || type == "password" || type == "search" || type == "hidden" || type == "number") {
                        if (!keys.hasOwnProperty(name)) {
                            outq[name] = [];
                        }
                        if (!CommonJs.IsEmpty(el.val()))
                            el.val(el.val().trim());
                        outq[name].push(el.val());

                    } else if (type == "checkbox") {
                        if (!keys.hasOwnProperty(name)) {
                            keys[name] = [];
                        }
                        if (el.prop("checked")) {
                            keys[name].push(el.val());
                        }
                        else {
                            keys[name].push(0);
                        }
                        //if (!checkboxs.hasOwnProperty(name)) {
                        //    checkboxs[name] = 0;
                        //}
                        //checkboxs[name]++;
                    } else if (type == "radio") {
                        if (!keys.hasOwnProperty(name)) {
                            keys[name] = [];
                        }
                        if (el.prop("checked")) {
                            keys[name].push(el.val());
                        }
                    }
                } else if (tagName != "button") {
                    if (!keys.hasOwnProperty(name)) {
                        keys[name] = [];
                    }
                    keys[name].push(el.val());
                }
            }
        });

        for (var k in outq) {
            var vals = outq[k];
            if (vals.length == 1) {
                outq[k] = vals.join(",");
            } else {
                outq[k] = JSON.stringify(vals);
            }
        }

        for (var k in keys) {
            var vals = keys[k];
            if (vals.length == 1) { //|| !checkboxs.hasOwnProperty(k)
                keys[k] = vals.join(",");
            } else {
                keys[k] = JSON.stringify(vals);
            }
        }
        return keys;
    },

    ClearFormSimple(form) {
        $(form).find("input, select, textarea,button").each(function () {
            let el = $(this);
        });
    },
    Select2Init: function (container) {
        try {
            if (container == undefined)
                container = jQuery(document);

            function formatResult(node) {
                if (typeof (node.element) == undefined) {
                    return node.text;
                }
                var level = 0;
                if (node.element !== undefined) {
                    if (!node.element.hasAttribute("level")) return node.text;
                    var levelvalue = (node.element.getAttribute("level"));
                    if (levelvalue == null) level = 0;
                    else level = levelvalue;
                }
                var $result = $('<span style="padding-left:' + (20 * level) + 'px;">' + node.text + '</span>');
                if (level == 0) $result = $('<span style="font-weight: bold;">' + node.text + '</span>');
                return $result;
            };
            container.find('select.select2').select2({
                placeholder: function () {
                    $(this).data('placeholder');
                },
                width: function () {
                    $(this).data('width');
                },
                language: "vi",
            });
            $('body').on('shown.bs.modal', '.modal', function () {
                $(this).find('.modal-body .select2').each(function () {
                    var dropdownParent = $(document.body);
                    var modalShown = $(this).parents('.modal.show:first');
                    if (modalShown.length !== 0) {
                        dropdownParent = modalShown;
                    }
                    let isMultiple = $(this).attr("multiple");
                    $(this).select2({
                        closeOnSelect: !isMultiple,
                        dropdownParent: dropdownParent,
                        language: "vi",
                        templateResult: formatResult,
                    });
                });

            });
            $('#drdPageSize').select2({
                width: 70,
                minimumResultsForSearch: Infinity,
                dropdownParent: $('#drdPageSize').parent(),
                language: "vi"
            });
        } catch (e) {
        }
    },
    Select2InitAjax: function (container) {
        try {
            if (container == undefined)
                container = jQuery(document);

            container.find('.select2Ajax').each(function () {
                CommonJs.UpdateSelect2Ajax($(this));
            });

        } catch (e) {
        }
    },
    UpdateSelect2: function (container) {
        if (container != undefined && container.hasClass("select2")) {
            let isMultiple = container.attr("multiple");
            container.select2({
                closeOnSelect: !isMultiple,
                placeholder: function () {
                    $(this).data('placeholder');
                },
                width: function () {
                    $(this).data('width');
                },
                language: "vi"
            });
        }
        else {
            if (container == undefined)
                container = jQuery(document);

            container.find('.select2').each(function () {
                let isMultiple = $(this).attr("multiple");
                $(this).select2({
                    closeOnSelect: !isMultiple,
                    placeholder: function () {
                        $(this).data('placeholder');
                    },
                    width: function () {
                        $(this).data('width');
                    },
                    language: "vi"
                })
            });
        }

    },
    UpdateSelect2Ajax: function (self) {
        if (!self.hasClass("innited")) {
            self.addClass("innited");

            //Lấy container cấp gần nhất
            var container = self.closest('tr'); //Trong hàng của bảng
            if (!container || container.length == 0)
                container = self.closest('.modal'); //Trong popup
            if (!container || container.length == 0)
                container = $(document); //Toàn trang

            var isMultiple = self.hasClass("select2-cs");

            var openOnLoad = self.attr("data-open-on-load") == "true";

            var url = self.attr("data-url");
            var title = self.attr("title");
            var dfOption = self.attr("data-default-option");
            var selectedID = self.attr("data-selected-value");
            var optionMapperValues = [];

            if (url) {
                var ajaxConfig = {
                    delay: 250,
                    url: url,
                    method: "POST",
                    dataType: 'json',
                    data: function (params) {
                        //khởi tạo data để gửi lên api
                        var query = {
                            Term: params.term,
                            DefaultOption: dfOption || title
                        }
                        var datax = self.getDataUppername();

                        //Lấy giá trị để truyền vào param IDTable
                        if (self.attr("data-table-ref")) {
                            //Lấy theo container
                            var inputTable = container.find(self.attr("data-table-ref"));
                            if (!inputTable || inputTable.length == 0) {
                                //Ko có thì lấy toàn trang
                                inputTable = $(self.attr("data-table-ref"));
                            }
                            query["IDTable"] = inputTable.val();
                        }
                        if (Object.keys(datax).length > 0) {
                            return Object.assign(query, datax);
                        }
                        return query;
                    },
                    processResults: function (data) {
                        if (data && data.data && data.data.categories) {
                            var sources = data.data.categories;
                            if (sources[0]) {
                                // Nếu chưa có định dạng như mặc định của select2 {id:1,text:"asd"} thì map sang
                                if (sources.length > 0 && !Object.keys(sources[0]).includes("id")) {
                                    sources = sources.map(s => {
                                        var attrs = s.hasOwnProperty("Attribute") ? s.Attribute : {};
                                        let entries = Object.entries(attrs);
                                        let capsEntries = entries.map((entry) => [entry[0].toLowerCase(), entry[1]]);
                                        let dataObj = Object.fromEntries(capsEntries);
                                        var res = { "id": s.value, "text": s.text };
                                        if (optionMapperValues.length > 0) {
                                            optionMapperValues.forEach((t, i) => {
                                                var item = dataObj[t.toLowerCase()];
                                                res[t.toLowerCase()] = item ? item : "";
                                            });
                                        }
                                        return res;
                                    });
                                }
                                return {
                                    results: sources
                                };
                            }
                        }
                        return { results: [] };
                    },
                };
                var configS = {
                    ajax: ajaxConfig,
                    placeholder: $(this).data('placeholder') || title || dfOption,
                    width: function () {
                        $(this).data('width');
                    },
                    language: "vi",
                    closeOnSelect: !isMultiple,
                    allowClear: isMultiple,
                    minimumInputLength: openOnLoad ? 0 : 1,
                    templateResult: function (dataRow) {
                        if (dataRow.loading) return dataRow.text;
                        const textFormat = Object.entries(dataRow)
                            .map(([key, value]) => key + '=' + value)
                            .join(' ');
                        var option = $(`<option ${textFormat}>${dataRow.text}</option>`)
                        return option;
                    },
                };

                self.select2(configS);

                if (parseInt(selectedID) > 0) {
                    //Call api để lấy text của item đã chọn 
                    var datax = self.getDataUppername();

                    //Lấy giá trị để truyền vào param IDTable
                    if (self.attr("data-table-ref")) {
                        var inputTable = container.find(self.attr("data-table-ref"));
                        if (!inputTable || inputTable.length == 0) {
                            //Ko có thì lấy toàn trang
                            inputTable = $(self.attr("data-table-ref"));
                        }
                        datax["IDTable"] = inputTable.val();
                    }

                    datax["ID"] = selectedID;
                    $.ajax({
                        type: 'POST',
                        data: datax,
                        url: url
                    }).then(function (data) {
                        if (data.data && data.data.category && data.data.category.text) {
                            var option = new Option(data.data.category.text, selectedID, true, true);

                            self.append(option).trigger('change');
                            //self.trigger({
                            //    type: 'select2:select',
                            //    params: {
                            //        data: data
                            //    }
                            //});
                        }
                    });

                }

            }


        }
    },
    IsInteger: function (val) {
        return !isNaN(val) && !CommonJs.IsEmpty(val);
    },
    IsEmpty: function (val) {

        if (typeof val == "object")
            return false;
        if (typeof val == "function")
            return false;

        return val === undefined || jQuery.trim(val).length === 0;
    },
    SetMessage: function (res, onlyFieldError, isNoReload) {
        if (res.type != undefined)
            res.type = res.type.toLowerCase();
        if (res.type === "success") {
            CommonJs.ShowNotifyMsg(res.type, res.message);
            if (!isNoReload)
                setTimeout(function () {
                    window.location.reload();
                }, 1000);
        }
        else if (res.type === "error") {
            if (onlyFieldError) {
                if (res.data != undefined && res.data.length > 0) {
                    //Show error by line
                    var totalErr = 0;
                    for (var i = 0; i < res.data.length; i++) {
                        let errField = jQuery(".field-validation-valid[data-valmsg-for='" + res.data[i].field + "']");
                        if (errField.length > 0) {
                            errField.html(res.data[i].mss);
                            totalErr++;
                        }
                    }
                    if (totalErr == 0) {
                        //Có lỗi nhưng ko có field nào set đc message lỗi
                        CommonJs.ShowNotifyMsg(SwalMsgType.error, res.message);
                    } else {
                        //Scroll đến field bị lôi
                        var firstErrField = $(".field-validation-valid:not(:empty):visible:first");
                        if (firstErrField.length > 0) {
                            let errInpuit = firstErrField.parent().find(":input");
                            if (errInpuit.length > 0)
                                errInpuit.focus();
                        }
                    }
                } else {
                    CommonJs.ShowNotifyMsg(SwalMsgType.error, res.message);
                }
            } else {
                CommonJs.ShowNotifyMsg(res.type, res.message);
            }
        }
        else if (res.type === "warning")
            CommonJs.ShowNotifyMsg(res.type, res.message);
    },
    SetMessageNotRefresh: function (res, onlyFieldError) {
        if (res.type != undefined)
            res.type = res.type.toLowerCase();
        if (res.type === "success") {
            CommonJs.ShowNotifyMsg(res.type, res.message);
        }
        else if (res.type === "error") {
            if (onlyFieldError) {
                if (res.data != undefined && res.data.length > 0) {
                    //Show error by line
                    for (var i = 0; i < res.data.length; i++) {
                        jQuery(".field-validation-valid[data-valmsg-for='" + res.data[i].field + "']").html(res.data[i].mss);
                    }
                } else {
                    CommonJs.ShowNotifyMsg(SwalMsgType.error, res.message);
                }
            } else {
                CommonJs.ShowNotifyMsg(res.type, res.message);
            }
        }
        else if (res.type === "warning")
            CommonJs.ShowNotifyMsg(res.type, res.message);
    },
    QuickActions: function () {
        jQuery(document).on("submit", ".quickSearch", function () {
            let form = $(this);
            let url = form.attr("action");
            let method = form.attr("method");
            let target = form.attr("data-target");
            let callback = form.attr("data-callback");
            let data = CommonJs.GetSerialize2(form);

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
                        }
                    } catch (e) {
                        console.log(e);
                    }
                    CommonJs.ToggleMultiTicks(jQuery(target));
                    $(target).html(rs);
                    if (callback)
                        eval(callback);
                    CommonJs.Select2Init($(target));
                    CommonJs.UpdateTreeGrid();
                    CommonJs.UpEvent();
                }
            });
            return false;
        })

        jQuery(document).on("click", ".quickUpdate", function () {

            let btn = $(this);
            let url = btn.attr("data-href");
            if (CommonJs.IsEmpty(url))
                url = btn.attr("href");
            let target = btn.attr("data-target");;
            var data = btn.getDataUppername();
            var method = btn.attr("data-method") || "GET";
            if (btn.attr("data-has-checkboxes") == "true") {
                let ids = [];
                let table = jQuery(this)
                    .closest(".dataTables_wrapper")
                    .find("table");
                table.find(".checkboxes").each(function () {
                    if (jQuery(this).prop("checked")) {
                        var id = jQuery(this).attr("data-id");
                        if (CommonJs.IsInteger(id)) {
                            ids.push(id);
                        }
                    }
                });
                if (ids.length > 0)
                    data.ids = ids;
            }

            jQuery.ajax({
                type: method,
                async: true,
                url: url,
                data: data,
                beforeSend: function () {
                    CommonJs.LazyLoadAjaxPro();
                },
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.SetMessage(rs);
                        return false;
                    };
                    $(target).html(rs);
                    CommonJs.UpdateFormState($(target));
                    CommonJs.UpdateInputMoney($(target));
                    CommonJs.UpdateIsNumber($(target));

                    //Tìm modal đang hiện
                    CommonJs.UpEvent($(target));

                    CommonJs.OpenModal(jQuery(target));
                }
            });
            return false;
        });

        jQuery(document).on("click", ".addBookmark", function () {
            let target = $(this);
            let id = $(this).attr("data-idModule");
            let url = '';
            if ($(this).hasClass('far')) {
                url = '/Home/AddBookMark';
            } else {
                url = '/Home/RemoveBookMark';
            }
            jQuery.ajax({
                type: 'POST',
                async: true,
                url: url,
                data: { id: id },
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.ShowNotifyMsg(rs.type, rs.message);
                        if (rs.data == 0) {
                            target.removeClass();
                            target.addClass('far fa-star btn-bookmark text-secondary addBookmark');
                            target.attr('data-original-title', 'Thêm lối tắt trang chủ');
                            target.attr('title', 'Thêm lối tắt trang chủ');
                        } else {
                            target.removeClass();
                            target.addClass('fas fa-star btn-bookmark text-yellow addBookmark');
                            target.attr('data-original-title', 'Bỏ lối tắt trang chủ');
                            target.attr('title', 'Bỏ lối tắt trang chủ');
                        }

                        return false;
                    };
                }
            });
            return false;
        });

        jQuery(document).on("click", ".quickDelete", function () {
            let id = $(this).attr("data-id");
            let name = $(this).attr("data-name");
            let url = $(this).attr("href");
            let confirmStr = jQuery(this).attr("data-comfirm-message") || 'Bạn có muốn xóa: ' + (name || 'bản ghi');
            let confirmHeader = confirmStr.substring(0, confirmStr.indexOf(':') + 1).trim();
            confirmMessage = confirmStr.substring(confirmStr.indexOf(':') + 1).trim();
            if (confirmHeader === '') {
                confirmHeader = confirmMessage;
                confirmMessage = '';
            }
            var data = jQuery(this).getDataUppername();
            delete data.ComfirmMessage;
            delete data.Target;

            CommonJs.ShowConfirmMsg(
                confirmHeader,
                confirmMessage,
                //'Bạn không thể phục hồi dữ liệu đã xóa',
                'Xóa',
                'Hủy',
                function () {
                    let method = "POST";
                    let ssCallBack = function (res) {
                        CommonJs.SetMessage(res);
                    };
                    CommonJs.CustAjaxCall(data, method, url, "json", ssCallBack, "");
                });
            return false;
        });

        jQuery(document).on("click", ".quickDeletes", function () {
            let table = jQuery(".dataTables_wrapper").find("table");
            let ids = [];
            let names = '';
            table.find(".checkboxes").each(function () {
                if (jQuery(this).prop("checked")) {
                    var id = jQuery(this).data("id");
                    var name = jQuery(this).data("item-name");
                    if (CommonJs.IsInteger(id)) {
                        ids.push(id);
                        names += name + ', '
                    }
                }
            });
            let url = jQuery(this).data("url");
            let name = $(this).data("name");
            if (names.indexOf(',') > -1) {
                names = names.substr(0, names.length - 2);
            }
            let confirmStr = jQuery(this).data("comfirm-message") || 'Bạn có muốn xóa ' + (name || 'bản ghi') + ': ' + names;
            if (confirmStr != undefined) {
                confirmStr = confirmStr.replaceAll("{n}", ids.length);
            }
            var data = jQuery(this).getDataUppername();
            delete data.ComfirmMessage;
            delete data.Target;

            if (ids == undefined) {
                return false;
            } else {
                let confirmHeader = confirmStr.substring(0, confirmStr.indexOf(':') + 1).trim();
                confirmMessage = confirmStr.substring(confirmStr.indexOf(':') + 1).trim();
                if (confirmHeader === '') {
                    confirmHeader = confirmMessage;
                    confirmMessage = '';
                }
                CommonJs.ShowConfirmMsg(
                    confirmHeader,
                    confirmMessage,
                    'Xóa',
                    'Hủy',
                    function () {
                        data.ids = ids;
                        let method = "POST";
                        let ssCallBack = function (res) {
                            CommonJs.SetMessage(res);
                        };
                        CommonJs.CustAjaxCall(data, method, url, "json", ssCallBack, "");
                    });
            }
            return false;
        });

        jQuery(document).on("click", ".quickApprove", function () {
            let table = jQuery(this).closest(".dataTables_wrapper").find("table");
            let ids = [];
            let names = '';
            table.find(".checkboxes").each(function () {
                if (jQuery(this).prop("checked")) {
                    var id = jQuery(this).data("id");
                    var name = jQuery(this).data("item-name");
                    if (CommonJs.IsInteger(id)) {
                        ids.push(id);
                        names += name + ', '
                    }
                }
            });
            let url = jQuery(this).data("url");
            let name = $(this).data("name");
            if (names.indexOf(',') > -1) {
                names = names.substr(0, names.length - 2);
            }
            let confirmStr = jQuery(this).data("comfirm-message") || 'Bạn có muốn duyệt lưu kho ' + (name || 'hồ sơ') + ': ' + names;
            if (confirmStr != undefined) {
                confirmStr = confirmStr.replaceAll("{n}", ids.length);
            }

            if (ids == undefined) {
                return false;
            } else {
                let confirmHeader = confirmStr.substring(0, confirmStr.indexOf(':') + 1).trim();
                confirmMessage = confirmStr.substring(confirmStr.indexOf(':') + 1).trim();
                if (confirmHeader === '') {
                    confirmHeader = confirmMessage;
                    confirmMessage = '';
                }
                CommonJs.ShowConfirmMsg2(
                    confirmHeader,
                    confirmMessage,
                    'Phê duyệt',
                    'Hủy',
                    function () {
                        let someData = { ids: ids };
                        let method = "POST";
                        let ssCallBack = function (res) {
                            CommonJs.SetMessage(res);
                        };
                        CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                    });
            }
            return false;
        });

        jQuery(document).on("click", ".quickRestoreDB", function () {
            let table = jQuery(this).closest(".dataTables_wrapper").find("table");
            let ids = [];
            let names = '';
            table.find(".checkboxes").each(function () {
                if (jQuery(this).prop("checked")) {
                    var id = jQuery(this).data("id");
                    var name = jQuery(this).data("item-name");
                    if (CommonJs.IsInteger(id)) {
                        ids.push(id);
                        names += name + ', '
                    }
                }
            });
            let url = jQuery(this).data("url");
            let name = $(this).data("name");
            if (names.indexOf(',') > -1) {
                names = names.substr(0, names.length - 2);
            }
            let confirmStr = jQuery(this).data("comfirm-message") || 'Bạn có muốn khôi phục dữ liệu ';
            if (confirmStr != undefined) {
                confirmStr = confirmStr.replaceAll("{n}", ids.length);
            }

            if (ids == undefined) {
                return false;
            } else {
                let confirmHeader = confirmStr.substring(0, confirmStr.indexOf(':') + 1).trim();
                confirmMessage = confirmStr.substring(confirmStr.indexOf(':') + 1).trim();
                if (confirmHeader === '') {
                    confirmHeader = confirmMessage;
                    confirmMessage = '';
                }
                CommonJs.ShowConfirmMsg2(
                    confirmHeader,
                    confirmMessage,
                    'Đống ý',
                    'Hủy',
                    function () {
                        let someData = { ids: ids };
                        let method = "POST";
                        let ssCallBack = function (res) {
                            CommonJs.SetMessage(res);
                        };
                        CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                    });
            }
            return false;
        });

        jQuery(document).on("click", ".quickSendDelivery", function () {
            let id = $(this).attr("data-id");
            let name = $(this).attr("data-name");
            let url = $(this).attr("href");

            let confirmStr = jQuery(this).attr("data-comfirm-message") || 'Bạn có muốn gửi bàn giao: ' + (name || 'biên bản');
            let confirmHeader = confirmStr.substring(0, confirmStr.indexOf(':') + 1).trim();
            confirmMessage = confirmStr.substring(confirmStr.indexOf(':') + 1).trim();
            if (confirmHeader === '') {
                confirmHeader = confirmMessage;
                confirmMessage = '';
            }
            CommonJs.ShowConfirmMsg2(
                confirmHeader,
                confirmMessage,
                //'Bạn không thể phục hồi dữ liệu đã xóa',
                'Bàn giao',
                'Hủy',
                function () {
                    let someData = {};
                    let method = "POST";
                    let ssCallBack = function (res) {
                        CommonJs.SetMessage(res);
                    };
                    CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                });
            return false;
        });

        jQuery(document).on("click", ".quickSendListDelivery", function () {
            let table = jQuery(this).closest(".dataTables_wrapper").find("table");
            let ids = [];
            let names = '';
            table.find(".checkboxes").each(function () {
                if (jQuery(this).prop("checked")) {
                    var id = jQuery(this).data("id");
                    var name = jQuery(this).data("item-name");
                    if (CommonJs.IsInteger(id)) {
                        ids.push(id);
                        names += name + ', '
                    }
                }
            });
            let url = jQuery(this).data("url");
            let name = $(this).data("name");
            if (names.indexOf(',') > -1) {
                names = names.substr(0, names.length - 2);
            }
            let confirmStr = jQuery(this).data("comfirm-message") || 'Bạn có muốn gửi bàn giao biên bản' + (name || 'hồ sơ') + ': ' + names;
            if (confirmStr != undefined) {
                confirmStr = confirmStr.replaceAll("{n}", ids.length);
            }

            if (ids == undefined) {
                return false;
            } else {
                let confirmHeader = confirmStr.substring(0, confirmStr.indexOf(':') + 1).trim();
                confirmMessage = confirmStr.substring(confirmStr.indexOf(':') + 1).trim();
                if (confirmHeader === '') {
                    confirmHeader = confirmMessage;
                    confirmMessage = '';
                }
                CommonJs.ShowConfirmMsg2(
                    confirmHeader,
                    confirmMessage,
                    'Bàn giao',
                    'Hủy',
                    function () {
                        let someData = { ids: ids };
                        let method = "POST";
                        let ssCallBack = function (res) {
                            CommonJs.SetMessage(res);
                        };
                        CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                    });
            }
            return false;
        });

        jQuery(document).on("submit", ".onUpdateInputState", function (e) {
            e.preventDefault()
            return false;
        });

        jQuery(document).on("click", ".quickSprint", function () {
            const self = $(this);
            let idAreaTarget = self.attr("data-target");
            let callback = "";
            const canvas = document.querySelector('#archive_chart');
            if (canvas) {
                callback = self.data("callback");
                const tagImgChart = document.querySelector('#imgChart');

                let img = canvas.toDataURL();
                tagImgChart.src = img;
                tagImgChart.style = "width:100%";
            }

            let printContents = $(idAreaTarget).html();
            setTimeout(function () {
                let originalContents = document.body.innerHTML;
                document.body.innerHTML = printContents;
                window.print();
                document.body.innerHTML = originalContents;
                if (callback)
                    eval(callback);
            }, 100);
        });

        jQuery(document).on("click", ".quickSubmit", function () {
            var self = $(this);
            var form = self.closest("form");

            if (form.hasClass("validateForm")) {
                var bootstrapValidator = form.data('bootstrapValidator');
                bootstrapValidator.validate();
                if (!bootstrapValidator.isValid(true)) {
                    return false;
                }
            }

            ////validate special charater
            //var strInputs = form.find("input[type=text]:not(.hasSpecialChar,.datetimepicker-input)"); //Namnp: nếu input có class hasSpecialChar thì ko xét (Datepicker)
            //var strTextAreas = form.find("textarea");
            //var format = /[`!#$%^&*+\=\[\]{};':"\\|<>\/~]/;
            //var isValidateSuccess = true;
            //if (strInputs != null && strInputs != undefined && strInputs.length > 0) {
            //    for (var i = 0; i < strInputs.length; i++) {
            //        if (format.test(strInputs[i].value)) {
            //            var name = strInputs[i].getAttribute("name");
            //            jQuery(".field-validation-valid[data-valmsg-for='" + name + "']").html("Nội dung không được chứa ký tự đặc biệt");
            //            isValidateSuccess = false;
            //        }
            //    }
            //}
            //if (strTextAreas != null && strTextAreas != undefined && strTextAreas.length > 0) {
            //    for (var i = 0; i < strTextAreas.length; i++) {
            //        if (format.test(strTextAreas[i].value)) {
            //            var name = strTextAreas[i].getAttribute("name");
            //            jQuery(".field-validation-valid[data-valmsg-for='" + name + "']").html("Nội dung không được chứa ký tự đặc biệt");
            //            isValidateSuccess = false;
            //        }
            //    }
            //}
            //if (!isValidateSuccess)
            //    return false;

            //Clear old message
            jQuery(".field-validation-valid").html("");

            let data = CommonJs.GetSerialize(form); //trimspace
            let url = self.attr("data-url");
            let resUrl = self.attr("data-resurl");
            let dataType = "json"
            let ssCallBack = function (res) {
                if (res.type === "Success") {
                    if (form.attr("data-is-append-select") == "true") {
                        form.closest('.modal').modal('toggle');//Close model when success
                        var slTarget = jQuery(form.attr("data-select-target"));
                        slTarget.append('<option value="' + res.data.id + '" >' + res.data.name + '</option>');
                        slTarget.val(res.data.id);
                        CommonJs.UpdateSelect2(slTarget);
                        CommonJs.SetMessage(res, true, true);
                        return false;
                    }


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

        jQuery(document).on("change", ".checkboxes", function () {
            CommonJs.ToggleMultiTicks(jQuery(this).closest('table'));
        });

        jQuery(document).on('change', '.group-checkable', function () {

            var table = jQuery(this).closest("table");
            //console.log(jQuery(this).attr('data-isgroup'));
            if (jQuery(this).attr('data-group') != undefined) {
                var group = jQuery(this).attr('data-group');
                var set = table.find('.checkboxes[data-group="' + group + '"]');
                //var set = table.find('.checkboxes');
            } else {
                var set = table.find(".checkboxes, .sumChecked");
            }
            var checked = jQuery(this).is(":checked");
            jQuery(set).each(function () {
                if (checked) {
                    if (!$(this).prop('disabled')) {
                        jQuery(this).prop("checked", true);
                        jQuery(this).closest('tr').addClass("active");
                    }
                } else {
                    jQuery(this).prop("checked", false);
                    jQuery(this).closest('tr').removeClass("active");
                }
            });
            CommonJs.ToggleMultiTicks(table);
        });

        jQuery(document).on("click", ".onSetPageIndex", function () {

            let btn = $(this);
            let form = jQuery(btn.attr("data-form"));
            let data = CommonJs.GetSerialize2(form);
            let url = form.attr("action");
            let target = form.attr("data-target");
            data.pageIndex = btn.attr("data-page");
            data.pageSize = btn.attr("data-page-size");

            let callback = form.attr("data-callback");

            jQuery.ajax({
                type: "POST",
                async: false,
                url: url,
                data: data,
                beforeSend: function () {
                    $(target).html('');
                },
                success: function (rs) {
                    try {
                        if (form.attr("data-state") != "0") {
                            window.history.pushState(null, rs.title, CommonJs.FormBuilderQString(form, data));
                            //jQuery(document).prop("title", rs.title);
                        }

                    } catch (e) {
                        console.log(e);
                    }
                    $(target).html(rs);
                    CommonJs.Select2Init();

                    if (callback)
                        eval(callback);
                }
            });
            return false;
        });

        jQuery(document).on("change", ".onChangePageSize", function () {

            let select = $(this);
            let form = jQuery(select.attr("data-form"));
            let data = CommonJs.GetSerialize2(form);
            let url = form.attr("action");
            let target = form.attr("data-target");
            data.pageIndex = 1;
            data.pageSize = select.val();
            jQuery.ajax({
                type: "POST",
                async: false,
                url: url,
                data: data,
                beforeSend: function () {
                    $(target).html('');
                },
                success: function (rs) {
                    try {
                        if (form.attr("data-state") != "0") {
                            window.history.pushState(null, rs.title, CommonJs.FormBuilderQString(form, data));
                            //jQuery(document).prop("title", rs.title);
                        }
                    } catch (e) {
                        console.log(e);
                    }
                    $(target).html(rs);
                    CommonJs.Select2Init();
                }
            });
            return false;
        });
        jQuery(document).on("change", ".select_soucreinfo", function () {

            let select = $(this);
            let val = parseInt(select.val());
            if (isNaN(val))
                val = 0;

            let isCsql = [1, 2, 3, 4].includes(val);
            let isApi = [5].includes(val);
            let isFile = [6, 7].includes(val);
            let form = select.closest("form");
            if (isCsql) {
                form.find(".divCSDLApi").removeClass("hidden");
                form.find(".divCSDL").removeClass("hidden");
                form.find(".divApi").addClass("hidden");
                form.find(".divFile").addClass("hidden");
            }
            else if (isApi) {
                form.find(".divCSDLApi").removeClass("hidden");
                form.find(".divCSDL").addClass("hidden");
                form.find(".divApi").removeClass("hidden");
                form.find(".divFile").addClass("hidden");
            }
            else if (isFile) {
                form.find(".divFile").removeClass("hidden");
                form.find(".divCSDLApi").addClass("hidden");
                form.find(".divCSDL").addClass("hidden");
                form.find(".divApi").addClass("hidden");
            }
            return false;
        });
        jQuery.fn.ForceNumericOnly = function () {
            return this.each(function () {
                $(this).keydown(function (e) {
                    var key = e.charCode || e.keyCode || 0;
                    // allow backspace, tab, delete, enter, arrows, numbers and keypad numbers ONLY
                    // home, end, period, and numpad decimal
                    return (
                        key == 8 ||
                        key == 9 ||
                        key == 13 ||
                        key == 46 ||
                        key == 110 ||
                        key == 190 ||
                        (key >= 35 && key <= 40) ||
                        (key >= 48 && key <= 57) ||
                        (key >= 96 && key <= 105));
                });
            });
        };

        jQuery(document).on("click", ".onAppendTemplate", function (e) {
            e.preventDefault();
            var obj = jQuery(this);
            var target = jQuery(obj.attr("data-target"));
            var temp = jQuery(obj.attr("data-template"));
            var cRowIndex = target.children().length;

            var maxWeight = 0;
            target.find("input[data-name='Priority']").each(function () {
                var cWeight = parseInt(jQuery(this).val());
                if (!isNaN(cWeight) && cWeight > maxWeight)
                    maxWeight = cWeight;
            });
            //Append and replace index
            CommonJs.Select2Init(cRow);
            target.append(temp.html().replaceAll("[n]", "[" + cRowIndex + "]"));
            var cRow = target.children(":nth(" + cRowIndex + ")");
            //Fix checkbox rename id 
            cRow.find(".custom-control.custom-checkbox").each(function () {
                let cb = jQuery(this).find(".checkboxes.custom-control-input");
                // let lb = jQuery(this).find(".custom-control-label");
                cb.attr("id", cb.attr("id") + cRowIndex);
                cb.next().attr("for", cb.attr("id"));
            });
            //Fix datetime rename id
            cRow.find(".input-group.date_input").each(function () {
                let tid = $(this).attr("id") + cRowIndex;
                $(this).find(".input-group-prepend").attr("data-target", `#${tid}`);
                $(this).find(".datetimepicker-input").attr("data-target", `#${tid}`);
                $(this).attr("id", tid);
            });

            //Fix radio rename id 
            cRow.find(".custom-control.custom-radio").each(function () {
                let cb = jQuery(this).find(".custom-control-input");
                cb.attr("id", cb.attr("id") + cRowIndex);
                cb.next().attr("for", cb.attr("id"));
            });

            cRow.find("input[data-name='Priority']").val(maxWeight + 1);
            CommonJs.Select2Init(cRow);
            CommonJs.Select2InitAjax(cRow);
            CommonJs.UpdateIsNumber(cRow);
            CommonJs.DataTableSizing(cRow);
            CommonJs.DateTimeFormat();
            return false;
        });

        jQuery(document).on("click", ".onDeleteItem", function () {
            var obj = jQuery(this);
            var isReindex = obj.attr("data-reindex") === "1";

            var tr = obj.closest(".item");
            var tbody = obj.closest("tbody");
            var cIndex = tr.index();

            var friends = tr.siblings("tr");
            if (tr.length > 0)
                tr.remove();

            if (isReindex && friends.length > 0) {
                //Đánh lại index
                const regex = /\[[0-9]+\]/gm;
                jQuery(tbody).find("select.select2-hidden-accessible").removeClass("innited").select2('destroy');
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
                CommonJs.Select2InitAjax(jQuery(tbody));
            }
            return false;
        });

        jQuery(document).on("click", ".quickExport", function (e) {
            e.preventDefault();

            let target = $(this).attr("data-target");
            let form = $(target);
            let datainput = {};
            let data = CommonJs.GetSerialize3(form, datainput);
            let queries = [];
            for (var i in datainput) {
                if (i == "__RequestVerificationToken")
                    continue;
                if (!CommonJs.IsEmpty(datainput[i])) {

                    queries.push(i + "=" + datainput[i]);
                }
            }
            let isPaging = $(this).attr("data-export-by-page") == "true";
            if (isPaging) {
                try {
                    var curl = new URL(window.location.href);
                    var pageSize = curl.searchParams.get("pageSize");
                    var pageIndex = curl.searchParams.get("pageIndex");
                    if (pageSize <= 0)
                        pageSize = 10;
                    if (pageIndex <= 0)
                        pageIndex = 1;

                    data["PageSize"] = pageSize;
                    data["PageIndex"] = pageIndex;
                } catch (e) {
                    console.log(e);
                }
            }
            let url = $(this).attr("action");
            let exportLink = url + CommonJs.BuilderQString(data);
            exportLink += (queries.length > 0 ? (exportLink.indexOf("?") > -1 ? "&" : "?") + queries.join("&") : "");
            window.location = exportLink;

            return false;
        });
        jQuery(document).on("click", ".exportByByte", function () {
            let btn = $(this);
            let form = jQuery(btn.attr("data-target"));
            let data = CommonJs.GetSerialize2(form);
            let url = $(this).attr("action");
            function Base64ToBytes(base64) {
                var s = window.atob(base64);
                var bytes = new Uint8Array(s.length);
                for (var i = 0; i < s.length; i++) {
                    bytes[i] = s.charCodeAt(i);
                }
                return bytes;
            };
            jQuery.ajax({
                type: "POST",
                async: false,
                url: url,
                data: data,
                success: function (rs, status, xhr) {
                    try {
                        if (rs.type != undefined) {
                            CommonJs.SetMessage(rs);
                            return false;
                        };
                        var filename = rs.fileName;
                        var type = rs.mimeType;
                        var bytes = Base64ToBytes(rs.fileContents);
                        var blob = new Blob([bytes], { type: type });
                        var downloadUrl = URL.createObjectURL(blob);
                        //window.location = downloadUrl;
                        var a = document.createElement("a");
                        a.href = downloadUrl;
                        a.download = filename;
                        document.body.appendChild(a);
                        a.click();
                        setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100);

                    } catch (e) {
                        console.log(e);
                    }
                }
            });
            return false;
        });
        jQuery(document).on("click", ".exportFile", function () {

            let url = $(this).attr("action");
            function Base64ToBytes(base64) {
                var s = window.atob(base64);
                var bytes = new Uint8Array(s.length);
                for (var i = 0; i < s.length; i++) {
                    bytes[i] = s.charCodeAt(i);
                }
                return bytes;
            };
            jQuery.ajax({
                type: "POST",
                async: false,
                url: url,
                data: {},
                success: function (rs, status, xhr) {
                    try {
                        if (rs.type != undefined) {
                            CommonJs.SetMessage(rs);
                            return false;
                        };
                        var filename = rs.fileName;
                        var type = rs.mimeType;
                        var bytes = Base64ToBytes(rs.fileContents);
                        var blob = new Blob([bytes], { type: type });
                        var downloadUrl = URL.createObjectURL(blob);
                        //window.location = downloadUrl;
                        var a = document.createElement("a");
                        a.href = downloadUrl;
                        a.download = filename;
                        document.body.appendChild(a);
                        a.click();
                        setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100);

                    } catch (e) {
                        console.log(e);
                    }
                }
            });
            return false;
        });

        //Remove error when typing input in form
        jQuery(document).on("keyup", ".onUpdateInputState :input", function () {
            var obj = jQuery(this);
            obj.parent().find(".field-validation-valid").html("");
        });

        jQuery(document).on("change", ".onUpdateInputState :input", function () {
            var obj = jQuery(this);
            obj.parent().find(".field-validation-valid").html("");
        });

        jQuery(document).on("paste", ".onUpdateInputState :input", function () {
            var obj = jQuery(this);
            obj.parent().find(".field-validation-valid").html("");
        });

        jQuery(document).on("change.datetimepicker", ".onUpdateInputState .date_input", function () {
            var obj = jQuery(this);
            obj.parent().find(".field-validation-valid").html("");
        });

        jQuery(document).on("click", ".quickComfirm", function () {
            let id = $(this).attr("data-id");
            let url = $(this).attr("data-url") || $(this).attr("href");
            let comfimTitle = $(this).attr("data-comfim-lable");
            let closeTitle = $(this).attr("data-close-lable");

            let comfirmMessage = jQuery(this).attr("data-comfirm-message") || 'Xác nhận';
            let confirmHeader = comfirmMessage.substring(0, comfirmMessage.indexOf(':') + 1).trim();
            confirmMessage = comfirmMessage.substring(comfirmMessage.indexOf(':') + 1).trim();
            var hasReason = $(this).attr("data-has-reason") == "true";
            if (hasReason) {
                CommonJs.ShowConfirmMsgNote(
                    confirmHeader,
                    confirmMessage,
                    //'Bạn không thể phục hồi dữ liệu đã xóa',
                    comfimTitle || 'Đồng ý',
                    closeTitle || 'Hủy',
                    function (reason) {
                        let someData = {
                            note: reason
                        };
                        let method = "POST";
                        let ssCallBack = function (res) {
                            CommonJs.SetMessage(res);
                        };
                        CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                    });
                return false;
            } else {
                CommonJs.ShowConfirmMsg2(
                    confirmHeader,
                    confirmMessage,
                    //'Bạn không thể phục hồi dữ liệu đã xóa',
                    comfimTitle || 'Đồng ý',
                    closeTitle || 'Hủy',
                    function () {
                        let someData = {};
                        let method = "POST";
                        let ssCallBack = function (res) {
                            CommonJs.SetMessage(res);
                        };
                        CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                    }, url);
                return false;
            }


        });

        jQuery(document).on("click", ".quickComfirms", function () {
            let table = jQuery(this).closest(".dataTables_wrapper").find("table");
            let ids = [];
            let comfimTitle = $(this).attr("data-comfim-lable");
            let closeTitle = $(this).attr("data-close-lable");

            table.find(".checkboxes").each(function () {
                if (jQuery(this).prop("checked")) {
                    var id = jQuery(this).data("id");
                    if (CommonJs.IsInteger(id)) {
                        ids.push(id);
                    }
                }
            });
            let url = jQuery(this).data("url");

            let confirmStr = jQuery(this).attr("data-comfirm-message") || 'Xác nhận';
            if (confirmStr != undefined) {
                confirmStr = confirmStr.replaceAll("{n}", ids.length);
            }
            if (ids == undefined) {
                return false;
            } else {
                let confirmHeader = confirmStr.substring(0, confirmStr.indexOf(':') + 1).trim();
                confirmMessage = confirmStr.substring(confirmStr.indexOf(':') + 1).trim();
                if (confirmHeader === '') {
                    confirmHeader = confirmMessage;
                    confirmMessage = '';
                }
                var hasReason = $(this).attr("data-has-reason") == "true";
                if (hasReason) {
                    CommonJs.ShowConfirmMsgNote(
                        confirmHeader,
                        confirmMessage,
                        comfimTitle || 'Đồng ý',
                        closeTitle || 'Hủy',
                        function (reason) {
                            let someData = {
                                ids: ids,
                                note: reason
                            };
                            let method = "POST";
                            let ssCallBack = function (res) {
                                CommonJs.SetMessage(res);
                            };
                            CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                        });
                    return false;
                } else {
                    CommonJs.ShowConfirmMsg2(
                        confirmHeader,
                        confirmMessage,
                        comfimTitle || 'Đồng ý',
                        closeTitle || 'Hủy',
                        function () {
                            let someData = {
                                ids: ids
                            };
                            let method = "POST";
                            let ssCallBack = function (res) {
                                CommonJs.SetMessage(res);
                            };
                            CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                        }, url);
                    return false;
                }
            }
            return false;
        });

        jQuery(document).on("click", ".quickConfirmRestore", function () {
            let id = $("#ID").val();
            let url = $(this).attr("data-url") || $(this).attr("href");
            let comfimTitle = $(this).attr("data-comfim-lable");
            let closeTitle = $(this).attr("data-close-lable");

            let comfirmMessage = jQuery(this).attr("data-comfirm-message") || 'Xác nhận';
            let confirmHeader = comfirmMessage.substring(0, comfirmMessage.indexOf(':') + 1).trim();
            confirmMessage = comfirmMessage.substring(comfirmMessage.indexOf(':') + 1).trim();
            var hasReason = $(this).attr("data-has-reason") == "true";

            CommonJs.ShowConfirmMsg2(
                confirmHeader,
                confirmMessage,
                //'Bạn không thể phục hồi dữ liệu đã xóa',
                comfimTitle || 'Đồng ý',
                closeTitle || 'Hủy',
                function () {
                    let someData = { id: id };
                    let method = "POST";
                    let ssCallBack = function (res) {
                        CommonJs.SetMessage(res);
                    };
                    CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                }, url);
            return false;
        });

        jQuery(document).on("click", ".quickComfirmNoReload", function () {
            let url = $(this).attr("data-url") || $(this).attr("href");
            let redirectUrl = $(this).attr("redirect-url");
            let comfimTitle = $(this).attr("data-comfim-lable");
            let closeTitle = $(this).attr("data-close-lable");

            let comfirmMessage = jQuery(this).attr("data-comfirm-message") || 'Xác nhận';
            let confirmHeader = comfirmMessage.substring(0, comfirmMessage.indexOf(':') + 1).trim();
            confirmMessage = comfirmMessage.substring(comfirmMessage.indexOf(':') + 1).trim();
            var hasReason = $(this).attr("data-has-reason") == "true";
            if (hasReason) {
                CommonJs.ShowConfirmMsgNote(
                    confirmHeader,
                    confirmMessage,
                    //'Bạn không thể phục hồi dữ liệu đã xóa',
                    comfimTitle || 'Đồng ý',
                    closeTitle || 'Hủy',
                    function (reason) {
                        let someData = {
                            note: reason
                        };
                        let method = "POST";
                        let ssCallBack = function (res) {
                            CommonJs.SetMessageNotRefresh(res);
                            if (redirectUrl != null && redirectUrl != undefined && redirectUrl != "") {
                                if (res.type === "success") {
                                    setTimeout(function () {
                                        window.location.href = redirectUrl;
                                    }, 1000);
                                }
                            }
                        };
                        CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                    });
                return false;
            } else {
                CommonJs.ShowConfirmMsg2(
                    confirmHeader,
                    confirmMessage,
                    //'Bạn không thể phục hồi dữ liệu đã xóa',
                    comfimTitle || 'Đồng ý',
                    closeTitle || 'Hủy',
                    function () {
                        let someData = {};
                        let method = "POST";
                        let ssCallBack = function (res) {
                            CommonJs.SetMessageNotRefresh(res);
                            if (redirectUrl != null && redirectUrl != undefined && redirectUrl != "") {
                                if (res.type === "success") {
                                    setTimeout(function () {
                                        window.location.href = redirectUrl;
                                    }, 1000);
                                }
                            }
                        };
                        CommonJs.CustAjaxCall(someData, method, url, "json", ssCallBack, "");
                    }, url);
                return false;
            }


        });

        jQuery(document).on("click", ".quickView", function () {
            let url = $(this).attr("href");
            let directUrl = $(this).attr("redirect-url");
            $.ajax(
                {
                    url: url,
                    type: "POST",
                    success: function (res) {
                        window.location.href = directUrl;
                    }
                }
            );

            //CommonJs.CustAjaxCall(formData, "POST", url, dataType, ssCallBack, errCallback);
            return false;
        });

        jQuery(document).on("change", ".onSelectChange, .onSelectChangeRow, .onSelectChangeForm", function () {

            var obj = jQuery(this);
            var container = $(document);
            var target = obj.attr("data-target");
            if (obj.hasClass("onSelectChangeRow")) {
                //Nếu sk trên 1 dòng chỉ tìm target trên dòng đó
                container = obj.closest("tr");
            }
            else if (obj.hasClass("onSelectChangeForm")) {
                //Nếu sk trên 1 form chỉ tìm target trên form đó
                container = obj.closest("form");
            }

            function onSelectChange(_target) {
                let ssCallBack = function (res) {

                    if (obj.hasClass("isHtml"))
                        _target.html(res);
                    else
                        _target.html(res.data);
                    CommonJs.UpdateSelect2(_target);

                    if (_target.val() > 0 && _target.hasClass("onSelectChange")) {
                        _target.trigger("change");
                    }
                }
                var data = {
                    SelectedID: obj.val(),
                    TargetSelectedID: _target.val(),
                    ID: _target.attr("data-id"),
                    DefaultText: _target.attr("data-default-text"),
                    Name: _target.attr("data-name")
                };

                var addDatas = container.find(obj.attr("data-addition"));
                if (addDatas.length > 0) {
                    addDatas.each(function () {
                        let k = $(this).attr("name");
                        let v = $(this).val();
                        if (k && v) {
                            data[k] = v.trim();
                        }
                    });
                }

                CommonJs.CustAjaxCall(data, "POST", obj.attr("data-url"), null, ssCallBack, null);
            };

            var targets = target.split(',');
            if (targets.length > 1) {
                //TH nhiều target
                for (var i = 0; i < targets.length; i++) {
                    onSelectChange(container.find(targets[i].trim()));
                }
            }
            else {
                //TH 1 target
                onSelectChange(container.find(target));
            }
            CommonJs.UpdateTreeGrid();
        });

        jQuery(document).on("click", ".onOpenModal", function () {
            var obj = jQuery(this);
            var target = obj.attr("data-target");
            var content = obj.attr("data-detach");
            $(target).html($(content).html());
            CommonJs.OpenModal(jQuery(target));
        });

        jQuery(document).on('shown.bs.modal', '.modal', function () {
            CommonJs.Select2InitAjax();
        });
        jQuery(document).on('click', '#dataFilter_Dropdown', function () {
            jQuery(this).parents(".dataFilter_Dropdown").toggleClass("open");
            jQuery(this).parents(".dataTables_wrapper").find(".dataFilter_Dropdown_target").toggleClass("open");
        });
        jQuery(document).on('click', '.dataFilter_Dropdown_close', function (e) {
            e.preventDefault();
            jQuery(this).parents(".dataTables_wrapper").find(".dataFilter_Dropdown").toggleClass("open");
            jQuery(this).parents(".dataTables_wrapper").find(".dataFilter_Dropdown_target").toggleClass("open");
        });

        $(document).on('select2:open', () => {
            document.querySelector('.select2-search__field').focus();
        });
    },

    ToggleMultiTicks: function (table) {
        var flag = false;
        var fullcheck = true;
        var wrapper = table.closest(".dataTables_wrapper");
        var actions = jQuery(".actMultiTicks");
        var buttons = actions.find(".btn");
        var grouper = wrapper.find(".group-checkable");
        table.find(".checkboxes").each(function () {
            if (jQuery(this).prop("checked")) {
                actions.removeClass("hidden");
                buttons.removeClass("disabled").prop("disabled", false);
                flag = true;
            }
            if (!jQuery(this).prop("checked")) {
                fullcheck = false;
            }
        });
        if (!flag) {
            actions.addClass("hidden");
            buttons.addClass("disabled").prop("disabled", true);
            if (grouper.prop("checked"))
                grouper.prop("checked", false);
        }
        if (fullcheck) {
            grouper.prop("checked", true);
        } else {
            grouper.prop("checked", false);
        }
    },
    UpdateFormState: function (container) {

        var flag = false;
        if (container.hasClass("validateForm")) {
            flag = true;
            CommonJs.BootstrapValidator(container);
        }
        container.find(".validateForm").each(function () {
            flag = true;
            CommonJs.BootstrapValidator(jQuery(this));
        });

        container.find(".form-control textarea:visible, .form-control input[type='text']:visible").each(function () {
            if (CommonJs.IsEmpty(jQuery(this).val())) {
                jQuery(this).focus();
                return false;
            }
        });
        container.find("select").unbind("mousewheel")
            .bind("mousewheel", "select", function (e) {
                e.stopPropagation();
            });


        if (flag == false) {
            var form = container.closest("form");
            if (form.hasClass("bootstrapValidator")) {
                container.find("[data-bv-field]").each(function () {
                    var name = jQuery(this).attr("data-bv-field");
                    var options = container.find('[name="' + name + '"]');
                    form.bootstrapValidator('addField', options);
                });
            }
        }
    },
    ValidateForm: function (form) {
        if (form.hasClass("validateForm")) {
            CommonJs.BootstrapValidator(form);
        }
    },
    BootstrapValidator: function (obj) {
        if (!obj.hasClass("bootstrapValidator")) {
            obj.addClass("bootstrapValidator").bootstrapValidator();
        }
    },
    DestroyValidator: function (container) {
        try {
            if (container.hasClass("bootstrapValidator")) {
                container.removeClass("bootstrapValidator").bootstrapValidator('destroy');
            }
        }
        catch (e) {
        }
    },
    ValidateDataForm: function (form) {

        form.find("input[type='text'], input[type='password'], textarea,input[type='time'], select").each(function () {

            var num = jQuery(this).removeClass("error").val();
            num = CommonJs.RemoveSpace(num);

            if (jQuery(this).hasClass('checkngay')) {
                if (!CommonJs.IsEmpty(num)) {
                    if (!jQuery.isNumeric(num) || parseInt(num) > 31 || parseInt(num) < 1) {
                        jQuery(this).addClass("error");
                    } else {
                        if (num.length == 1) {
                            num = "0" + num;
                        }
                        jQuery(this).val(num);
                    }
                }
            }
            else if (jQuery(this).hasClass('checkthang')) {
                if (!CommonJs.IsEmpty(num)) {
                    if (!jQuery.isNumeric(num) || parseInt(num) > 12 || parseInt(num) < 1) {
                        jQuery(this).addClass("error");
                    } else {
                        if (num.length == 1) {
                            num = "0" + num;
                        }
                        jQuery(this).val(num);
                    }
                }
            }
            else if (jQuery(this).hasClass('checknam')) {
                if (!CommonJs.IsEmpty(num)) {
                    if (!jQuery.isNumeric(num) || parseInt(num) < 1900 || parseInt(num) > 2015) {
                        jQuery(this).addClass("error");
                    } else {
                        jQuery(this).val(num);
                    }
                }
            }
            else if (jQuery(this).hasClass('checkint')) {
                if (!CommonJs.IsEmpty(num)) {
                    if (!jQuery.isNumeric(num) || num.indexOf(".") != -1 || num.indexOf(",") != -1) {
                        jQuery(this).addClass("error");
                    } else {
                        jQuery(this).val(num);
                    }
                }
            }

            if (jQuery(this).hasClass('checkrequired')) {
                if (CommonJs.IsEmpty(num)) {
                    jQuery(this).addClass("error");
                }
                else if (jQuery(this).prop("tagName") == "SELECT" && num == "0") {
                    jQuery(this).addClass("error");
                }
            }

            if (jQuery(this).hasClass('checkcompare')) {

                var comparator = jQuery(jQuery(this).attr("data-compare"));
                var valCompare = comparator.val();
                if (valCompare != num) {
                    jQuery(this).addClass("error");
                    comparator.addClass("error");
                }
            }
        });

        var elErrors = form.find(".error");
        if (elErrors.length > 0) {
            var messages = [];
            elErrors.each(function () {
                var data = jQuery(this).getDataUppername();
                if (!CommonJs.IsEmpty(data.MessageNotEmpty))
                    messages.push(data.MessageNotEmpty);
            });
            if (messages.length > 0) {
                alert(messages.join("\n"));
            }
            var elError = elErrors.first().focus();
            if (!elError.is(":visible")) {
                elError.closest(".group-tab").find(".tab-data").addClass("hidden");
                var idTab = elError.closest(".tab-data").removeClass("hidden").attr("id");

                elError.closest(".group-tab").find(".tabitem").removeClass("active");
                elError.closest(".group-tab").find(".tabitem[data-tab='#" + idTab + "']").addClass("active");
            }
            return false;
        }
        return true;
    },
    RemoveSpace: function (val) {
        try {
            while (val.indexOf(" ") !== -1) {
                val = val.replace(" ", "");
            }
        } catch (e) { }
        return val;
    },
    UpdateTreeGrid: function (container) {

        if (CommonJs.IsEmpty(container))
            container = jQuery(document);

        container.find(".useTreegrid").each(function () {
            var number = $(this).attr("data-tree");
            var initState = 'expanded';
            if (jQuery(this).attr('data-initcollapsed') == 'true') {
                initState = 'collapsed';
            }
            jQuery(this).treegrid({
                treeColumn: number,
                initialState: initState
            });
        });
    },

    BuilderQString: function (data) {
        var queries = [];
        for (var i in data) {
            if (i == "__RequestVerificationToken")
                continue;
            if (!CommonJs.IsEmpty(data[i]) && (data[i] > -1 || data[i].indexOf(',') > -1)) {
                queries.push(i + "=" + data[i]);
            }
        }
        if (queries.length > 0)
            return ("?" + queries.join("&"));

        return "";
    },
    FormBuilderQString: function (form, fdata) {
        var data = {};
        form.find("input, select, textarea,button").each(function () {
            var el = jQuery(this);
            var name = el.prop("name");
            if (!CommonJs.IsEmpty(name)) {
                var tagName = el.prop("tagName").toLowerCase();
                if (tagName == "input") {
                    var type = el.prop("type").toLowerCase();
                    if (type == "text" || type == "password" || type == "hidden" || type == "number") {
                        if (!data.hasOwnProperty(name)) {
                            data[name] = [];
                        }
                        if (!CommonJs.IsEmpty(el.val()))
                            el.val(el.val().trim());
                        data[name].push(el.val());
                    } else if (type == "checkbox") {
                        if (!data.hasOwnProperty(name)) {
                            data[name] = [];
                        }
                        if (el.prop("checked")) {
                            data[name].push(el.val());
                        }
                        else {
                            data[name].push(0);
                        }
                        //if (!checkboxs.hasOwnProperty(name)) {
                        //    checkboxs[name] = 0;
                        //}
                        //checkboxs[name]++;
                    } else if (type == "radio") {
                        if (!data.hasOwnProperty(name)) {
                            data[name] = [];
                        }
                        if (el.prop("checked")) {
                            data[name].push(el.val());
                        }
                    }
                }
                else if (tagName == "select") {
                    if (!CommonJs.IsEmpty(el.val()) && (parseInt(el.val()) > -1 || el.val().indexOf(",") > -1)) {
                        data[name] = [];
                        data[name].push(el.val());
                    }
                }
                else if (tagName != "button") {
                    if (!data.hasOwnProperty(name)) {
                        data[name] = [];
                    }
                    data[name].push(el.val());
                }
            }
        });
        for (var k in data) {
            var vals = data[k];
            if (vals.length == 1) { //|| !checkboxs.hasOwnProperty(k)
                data[k] = vals.join(",");
            } else {
                data[k] = JSON.stringify(vals);
            }
        }
        if (fdata != undefined) {
            //add paging params
            if (fdata["pageIndex"])
                data["pageIndex"] = fdata["pageIndex"];
            if (fdata["pageSize"])
                data["pageSize"] = fdata["pageSize"];
        }

        var queries = [];
        for (var i in data) {
            if (i == "__RequestVerificationToken")
                continue;
            if (!CommonJs.IsEmpty(data[i])) {
                queries.push(i + "=" + data[i]);
            }
        }
        return ("?" + queries.join("&"));
    },
    DateTimeFormat: function () {
        var datePicker = $('.date_input');
        var format = datePicker.attr("data-format");
        if (CommonJs.IsEmpty(format)) {
            format = 'DD/MM/yyyy';
        }
        format = format.replace("dd", "DD");
        //Date picker
        datePicker.length > 0 && datePicker.datetimepicker({
            format: format,
            locale: moment.locale('vi'),
            dayViewHeaderFormat: 'MMM YYYY',
            widgetPositioning: {
                horizontal: 'right',
                vertical: 'auto'
            },
            tooltips: {
                today: 'Tới hôm nay',
                clear: 'Xóa',
                close: 'Đóng',
                selectMonth: 'Chọn tháng',
                prevMonth: 'Tháng trước',
                nextMonth: 'Tháng sau',
                selectYear: 'Chọn năm',
                prevYear: 'Năm trước',
                nextYear: 'Năm sau',
                selectDecade: 'Chọn thập kỷ',
                prevDecade: 'Thập kỷ trước',
                nextDecade: 'Thập kỷ sau',
                prevCentury: 'Thế kỷ trước',
                nextCentury: 'Thế kỷ sau',
                pickHour: 'Chọn giờ',
                incrementHour: 'Tăng giờ',
                decrementHour: 'Giảm giờ',
                pickMinute: 'Chọn phút',
                incrementMinute: 'Tăng phút',
                decrementMinute: 'Giảm phút',
                pickSecond: 'Chọn giây',
                incrementSecond: 'Tămg giây',
                decrementSecond: 'Giảm giây',
                togglePeriod: 'Toggle Period',
                selectTime: 'Chọn thời gian',
                selectDate: 'Chọn ngày'
            },
            container: '.wrapper'
        });
        var dateRange = $('.date_range');
        //Date range picker
        dateRange.length > 0 && dateRange.daterangepicker({
            locale: {
                format: 'DD/MM/yyyy',
                applyLabel: 'Chọn',
                cancelLabel: 'Hủy'
            }
        });
    },
    UpdateInputMoney: function (container) {
        try {
            if (container == undefined)
                container = jQuery(document);
            container.find('.useMoney').simpleMoneyFormat();
            container.find(".useMoney").each(function () {
                $(this).keyup(function () {
                    this.value = CommonJs.FormatMoney(this.value);
                });
            });
        } catch (e) {

        }
    },
    FormatMoney: function (n) {
        var i, f;
        if (n != null) {
            var t = n.toString().replace(".", ""), r = !1, u = [], e = 1, o = null;
            for (t.indexOf(".") > 0 && (r = t.split("."), t = r[0]), t = t.split("").reverse(), i = 0, f = t.length; i < f; i++) t[i] != "," && (u.push(t[i]), e % 3 == 0 && i < f - 1 && u.push(","), e++);
            return o = u.reverse().join(""), o + (r ? "." + r[1].substr(0, 2) : "");
        }
        return n;
    },
    UpdateIsNumber: function (container) {
        if (container == undefined)
            container = jQuery(document);
        //$('.isNumberInteger, .isNumber').on("paste", function (e) {
        //    e.preventDefault(); 
        //});
        $('.isNumber').keypress(function (event) {
            var charCode = (event.which) ? event.which : event.keyCode;
            if (
                (charCode != 45 && charCode != 8 || $(this).val().indexOf('-') != -1) && // “-” CHECK MINUS, AND ONLY ONE.
                (charCode != 46 || $(this).val().indexOf('.') != -1) && // “.” CHECK DOT, AND ONLY ONE.
                (charCode < 48 || charCode > 57))
                return false;
            if ($(this).val().indexOf('0') == 0 && $(this).val() != "") { //Remove 0 first
                $(this).val($(this).val().replace("0", ""));
            }
            return true;
        });
        $('.isNumberInteger').keypress(function (e) {
            var charCode = (event.which) ? event.which : event.keyCode;
            if (
                (charCode != 45 && charCode != 8 || jQuery(this).hasClass("isPositiveNumber") || $(this).val().indexOf('-') != -1) // “-” CHECK MINUS, AND ONLY ONE, IS Positive Number
                && (charCode < 48 || charCode > 57))
                return false;

            if ($(this).val().indexOf('-') > 0) { //Move - to first index
                $(this).val("-" + $(this).val().replace("-", ""));
            }
            if ($(this).val().indexOf('0') == 0 && $(this).val() != "") { //Remove 0 first
                $(this).val($(this).val().replace("0", ""));
            }
            return true;
        });
        $('.isNumberInteger').change(function (e) {
            if (jQuery(this).hasClass("useMoney"))
                $(this).val($(this).val().replace(/[^0-9\-,]/g, ""));
            else
                $(this).val($(this).val().replace(/[^0-9\-]/g, ""));
            //Move - to first index
            if ($(this).val().indexOf('-') > 0) {
                $(this).val("-" + $(this).val().replace("-", ""));
            }

        });
        $('.isPositiveNumber').change(function (e) {
            $(this).val($(this).val().replace(/[^0-9]/g, ""));
        });
        $('.isInputOnlyNumber').keypress(function (event) {
            var charCode = (event.which) ? event.which : event.keyCode;
            if ((charCode < 48 || charCode > 57))
                return false;
            return true;
        });
        $('.isInputOnlyNumber').change(function (e) {
            $(this).val($(this).val().replace(/[^0-9]/g, ""));
        });
    },
    /**
     * activeSidebarMenu
     */
    activeSidebarMenu: function () {
        /**
         * Remove trail of url
         * @param {string} url
         * @returns {string}
         */
        const _removeTrail = function (url) {
            //Get the trailer position of url
            let i = url.indexOf('?');
            let j = url.indexOf('#');
            const trailer = (i > 0 && j > 0) ? Math.min(i, j) : Math.max(i, j);
            //Remove the trailer if exist
            return (trailer !== -1) ? url.substring(0, trailer) : url;
        }
        const sidebar = document.getElementsByClassName('nav-sidebar')[0];
        //Check if sidebar exist
        if (!sidebar || sidebar.length == 0) {
            return false;
        }
        //Check if sidebar is multilevel
        const treeViews = sidebar.getElementsByClassName('nav-treeview');
        if (treeViews.length == 0) {
            return false;
        }
        //update by BreadCrumb
        let url = _removeTrail(window.location.href.toLowerCase());
        let check = true;
        //const breadcrumb = document.getElementById('breadcrumb').getElementsByTagName("li");
        const breadcrumb = document.getElementById('breadcrumb');
        if (breadcrumb != null) {
            var listUrl = [];
            for (let liItem of breadcrumb) {
                let item = liItem.firstElementChild;
                if (item != null && item.href) {
                    let href = item.href.toLowerCase();
                    listUrl.push(href);
                }
            }
            while ((a = listUrl.pop()) != null) {
                if (!check) break;
                for (let tree of treeViews) {
                    let items = tree.children;
                    let toggler = tree.previousElementSibling;
                    toggler.classList.remove('active');
                    for (let item of items) {
                        let link = item.firstElementChild;
                        let list = item.parentElement.parentElement;
                        let href = link.href.toLowerCase();
                        link.classList.remove('active');
                        if (a === href) {
                            link.classList.add('active');
                            toggler.classList.add('active');
                            list.classList.add('menu-open');
                            check = false;
                        }
                    }
                }
            }
        }
        while (check) {
            for (let tree of treeViews) {
                let items = tree.children;
                let toggler = tree.previousElementSibling;
                toggler.classList.remove('active');
                for (let item of items) {
                    let link = item.firstElementChild;
                    let list = item.parentElement.parentElement;
                    let href = link.href.toLowerCase();
                    link.classList.remove('active');
                    if (url === href) {
                        link.classList.add('active');
                        toggler.classList.add('active');
                        list.classList.add('menu-open');
                        check = false;
                    }
                }
            }
            if (check) {
                if (url.lastIndexOf('/') < 0) check = false;
                else
                    url = url.substr(0, url.lastIndexOf('/'));
            }
        };
    },
    /**
     * DataTableSizing
     */
    DataTableSizing: function () {
        const $tables = $('.data_table');
        $tables.length !== 0 && $tables.each(function () {
            const $table = $(this);
            $table.find('th').each(function (index) {
                $table.find(` > tbody > tr:not(.expandable-body) > td:nth-child(${index + 1})`).attr('class', $(this).attr('class'));
            });
        });
    },

    FrontEndFunctionsNeedToReRun: function () {
        CommonJs.DataTableSizing();
        $('.breadcrumb [data-toggle="tooltip"]').tooltip({
            html: true,
            //container: '.content-wrapper',
            placement: 'right',
            boundary: 'window',
            trigger: 'hover'
        });
        $('.dashboard [data-toggle="tooltip"]').tooltip();
        $('.data_table [data-toggle="tooltip"]').tooltip({
            html: true,
            container: '.content-wrapper',
            placement: 'top',
            boundary: 'window',
            trigger: 'hover'
        });
        $('.documenting .card-body').overlayScrollbars({
            className: 'os-theme-dark',
            sizeAutoCapable: true,
            scrollbars: {
                clickScrolling: true
            },
            overflowBehavior: {
                x: 'hidden'
            }
        });
    },
    LazyLoadAjax: function (container) {
        var lazyload = new CommonJs.lazyload($(container || '.content-wrapper'));
        lazyload.show();
        $(document).on('ajaxComplete', function () {
            lazyload.hide();
        });
    },
    /**
     * Lazyload
     * @param {object} container jQuery Element
     */
    lazyload: function (container) {
        //default container for backdrop
        container = container || $('.content-wrapper');
        //backdrop template
        var template = $('<div />', { class: 'lazyload-overlay', }).append($('<i />', { class: 'fas fa-3x fa-pulse fa-spinner' }));
        return {
            show: function () { container.append(template) },
            hide: function () { template.remove() }
        };
    },
    LazyLoadAjaxPro: function (container) {
        var lazyload = new CommonJs.lazyloadPro($("body"));
        lazyload.show();
        $(document).on('ajaxComplete', function () {
            lazyload.hide();
        });
    },
    /**
     * Lazyload
     * @param {object} container jQuery Element
     */
    lazyloadPro: function (container) {
        //default container for backdrop
        container = container || $('.content-wrapper');
        //backdrop template
        var template = $('<div />', { id: 'loader-wrapper', }).append($('<div />', { id: 'loader' }));
        return {
            show: function () { container.append(template) },
            hide: function () { template.remove() }
        };
    },
    UpdateQueryStringParameter: function (uri, key, value) {
        var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
        var separator = uri.indexOf('?') !== -1 ? "&" : "?";
        if (uri.match(re)) {
            return uri.replace(re, '$1' + key + "=" + value + '$2');
        } else {
            return uri + separator + key + "=" + value;
        }
    },

    //Nhấn Esc close modal đang mở
    PressEscapeCloseModal: function () {
        jQuery(document).on("keydown", function (e) {
            let keyCode = e.keyCode;
            if (keyCode === 27) {//keycode is an Integer, not a String
                jQuery(".modal:visible").modal('toggle');
            }
        });
    },
    GetDomain: function () {

        var domain = window.location.protocol
        domain += "//";
        domain += window.location.hostname;
        if (window.location.port !== "") {
            domain += ":";
            domain += window.location.port;
        }
        return domain;
    },

    FormatPassword: function () {
        $("input[type=password]").keydown(function (e) {
            if (e.keyCode == 32)
                return false;
        })
    },

    OpenModal: function (md) {
        var cmodals = jQuery(".modal:visible:not('#" + md.attr("id") + "')");
        if (cmodals.length > 0) {
            //var zIndex = cmodals.css("z-index");
            //cmodals.css("z-index", 1000).attr("data-z-index", zIndex); //Cho zIndex nhỏ hơn
            cmodals.addClass("hide-modal").hide();
        }
        md.modal({
            backdrop: 'static', //Click outside
            //keyboard: true, //Esc
            appendTo: 'body',
            show: true
        }).on('hidden.bs.modal', function () {

            if (md.hasClass("closeOnResetForm"))
                md.find("form").reset();

            var hideModal = jQuery(".modal.hide-modal:hidden:first");
            if (hideModal.length > 0)
                hideModal.removeClass("hide-modal").show();
        });
    },
    InitSelect2SearchCs: function () {
        const $jsMultiSearch = $('.jsMultiSearch');
        if ($('.jsMultiSearch').length !== 0) {

            $jsMultiSearch.each(function (index, item) {
                const dataUrl = $(item).data("search-url");
                $(item).select2({
                    ajax: {
                        url: `/${dataUrl}/`,
                        dataType: 'json',
                        data: function (params) {
                            return {
                                term: params.term, // search term
                                page: params.page
                            };
                        },
                        processResults: function (data, params) {
                            params.page = params.page || 1;
                            let dataSelect = [];
                            $.each(data, function (i, el) {
                                let item = { id: el.id, text: Object.values(el)[1] };
                                dataSelect.push(item);
                            });

                            return {
                                results: dataSelect,
                                pagination: {
                                    more: (params.page * 30) < data.total_count
                                }
                            };
                        },
                        cache: true
                    },
                    escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                    minimumInputLength: 1,
                    placeholder: function () {
                        $(this).data('placeholder');
                    },
                    width: function () {
                        $(this).data('width');
                    },
                    language: "vi",
                    closeOnSelect: this.hasOwnProperty('hasAttribute') ? this.hasAttribute('multiple') : false
                })
                //scroll left ul selected
                $(item).on('select2:close', function (e) {
                    const ul = $(this).next().find('ul.select2-selection__rendered ');
                    ul.scrollLeft(0);
                })
            })
        }
    },

    MessageBlock: function () {
        jQuery(document).on("click", ".onCloseMsg", function () {
            jQuery(this).closest(".toastsContainer").remove();
        });

        setTimeout(function () {
            $(".toastsContainer").each(function () {
                $(this).remove();
            });
        }, 3000);

    },

    InitBeautifyJson: function () {
        jQuery(".useBeautifyJson").each(function () {
            var text = $(this).text();

            $(this).html(JSON.stringify(JSON.parse(text), null, 4));
        });
    },

    UpperFirstLetter: function (string) {
        if (!string)
            return string;

        if (string.indexOf("id") == 0)
            string = string.replace("id", "ID");

        return string.charAt(0).toUpperCase() + string.slice(1);
    },

    UpEvent: function (container) {
        if (!container)
            container = jQuery(document);

        CommonJs.DateTimeFormat();
        CommonJs.Select2Init();
        CommonJs.UpdateTreeGrid();
        Uploader.upEvent(container);
    },

    ScrollToDiv: function (container, target, smooth) {
        if (!container)
            container = jQuery(document);

        if (target.length > 0) {
            container.animate({
                scrollTop: target.position().top
            }, smooth ?? 1000);
        }
    },

}
String.prototype.capitalize = function () {
    return this.charAt(0).toUpperCase() + this.slice(1);
}
jQuery.fn.extend({
    reset: function () {
        try {
            this.each(function () {
                this.reset();
            });
            jQuery(jQuery(this).attr("data-html-reset")).html("");
        } catch (e) {
        }
        jQuery(this).find(".isNew").remove();
    },
    getData: function () {
        var data = {};
        try {
            this.each(function () {
                jQuery.each(this.attributes, function () {
                    var name = this.name.toLowerCase();
                    if (name.indexOf("data-") === 0) {
                        var k = "";
                        var args = name.split("-");
                        for (var n = 0; n < args.length; n++) {
                            if (n == 0) continue;
                            if (n == 1) {
                                k += args[n];
                            }
                            else {
                                k += args[n].capitalize()
                            }
                        }
                        data[k] = this.value;
                    }
                });
            });
        } catch (e) {
        }
        return data;
    },
    getDataUppername: function () {
        var data = {};
        try {
            this.each(function () {
                jQuery.each(this.attributes, function () {
                    var name = this.name.toLowerCase();
                    if (name.indexOf("data-") === 0) {
                        var k = "";
                        var args = name.split("-");
                        for (var n = 0; n < args.length; n++) {
                            if (n == 0) continue;
                            var v = args[n];
                            if (v == "id") {
                                k += v.toUpperCase();
                            }
                            else {
                                k += v.capitalize()
                            }
                        }
                        data[k] = this.value;
                    }
                });
            });
        } catch (e) {
        }
        return data;
    },
    replaceAll: function (str, find, replace) {
        return str.replace(new RegExp(find, 'g'), replace);
    },

});
var InitCommonJs = function () {
    CommonJs.ShowNotifyMsg();
    CommonJs.QuickSubmitJS();
    CommonJs.Select2Init();
    CommonJs.QuickActions();
    CommonJs.UpdateTreeGrid();
    CommonJs.DateTimeFormat();
    CommonJs.UpdateInputMoney();
    CommonJs.UpdateIsNumber();
    CommonJs.UpdateFormState(jQuery(document));
    CommonJs.PressEscapeCloseModal();
    jQuery(document).ready(function ($) {
        //Fix bug nhấn back trên trình duyệt thì load lại trang
        if (window.history && window.history.pushState) {
            $(window).on('popstate', function () {
                window.location.reload();
            });
        }
    });
    CommonJs.FrontEndFunctionsNeedToReRun();
    CommonJs.activeSidebarMenu();
    CommonJs.MessageBlock();
    CommonJs.InitBeautifyJson();
}

jQuery(window).on('ajaxComplete', function () {
    CommonJs.FrontEndFunctionsNeedToReRun();
});

jQuery(document).on("click", "#btnSeeMore", function () {
    debugger;
    const tagCurent = $(this);
    const tr = tagCurent.closest('tr');
    let clss = tr.attr("class");
    let attrPageIndex = tagCurent.attr("data-pagetarget");
    let url = tagCurent.attr("data-url");
    var PageIndex = parseInt($(attrPageIndex).val()) + 1;
    let target = tagCurent.attr("data-target");
    const bodyTarget = $(target).find("tbody");
    const form = $("#frmInformationLevelSearch");
    var data = CommonJs.GetSerialize2(form);
    data.PageIndex = PageIndex;
    $.ajax({
        type: "POST",
        async: true,
        url: url,
        data: data,
        beforeSend: function () {
            CommonJs.LazyLoadAjaxPro();
        },
        success: function (rs) {
            $(pageIndex).val(PageIndex);
            tr.before(rs);
            tr.remove();
            $('.isParent').addClass(clss).removeClass('isParent');
        }
    });
})