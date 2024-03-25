var CollectionManagementConfig = {
    Init: function () {
        $('#IDDocType').change(function () {
            CollectionManagementConfig.ChangeDocType($(this).val(), $("#divDocTypes").attr("data-profile"), $(this));
            CommonJs.UpdateFormState(jQuery(document));
        });

        jQuery(document).on("click", ".onSelectFile", function () {
            var obj = jQuery(this);
            var rel = jQuery(obj.attr("data-rel"));
            rel.attr("data-target", obj.attr("data-target"));
            rel.attr("data-form-meta", obj.attr("data-form-meta"));
            rel.trigger('click')
            return false;
        });

        jQuery(document).on("change", ".inputUpfile", function () {
            var obj = jQuery(this);
            var target = obj.attr("data-target")

            const data = JSON.parse($('#myFile').val());
            let myDropzone = jQuery("#dropzoneForm");
            $.each(data, function (key, value) {
                const mockFile = { name: value.name, size: value.size };
                myDropzone.files[key] = value.name;
                myDropzone.options.addedfile.call(myDropzone, mockFile);
                myDropzone.options.thumbnail.call(myDropzone, mockFile, value.url);
                mockFile.previewElement.classList.add('dz-success');
                mockFile.previewElement.classList.add('dz-complete');
            });


            $(target).modal({
                backdrop: 'static', //Click outside
                //keyboard: true, //Esc
                show: true
            });


            return false;
        });

        jQuery(document).on("click", ".quickSubmitFile", function () {
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
            let url = self.attr("data-url");
            var formData = new FormData();
            var data = CommonJs.GetSerialize2(form);

            var formTarget = jQuery(self.attr("data-form-meta"));
            for (var key in data) {
                if (data.hasOwnProperty(key)) {
                    formData.append(key, data[key]);
                }
            }
            form.find("input[type='file']").each(function () {
                var name = jQuery(this).attr("name");
                if (name) {
                    formData.append(name, $(this)[0].files[0]); // myFile is the input type="file" control
                }
            });
            $.ajax({
                url: url,
                type: 'POST',
                data: formData,
                processData: false,  // tell jQuery not to process the data
                contentType: false,  // tell jQuery not to set contentType
                success: function (result) {
                },
                error: function (jqXHR) {
                },
                complete: function (jqXHR, status) {

                },
                success: function (res) {
                    if (res.type === "Success")
                        form.closest('.modal').modal('toggle');//Close model when success
                    CommonJs.SetMessage(res, true, true);

                    if (res.data) {
                        form.reset();

                        jQuery(".onAutoOCR").attr("href", CommonJs.UpdateQueryStringParameter(jQuery(".onAutoOCR").attr("href"), "IDFile", res.data.idFile));
                        //Set url Ocr
                        formTarget.find("[name='IDFile']").val(res.data.idFile);

                        jQuery("#DEFAULT_URL").val(res.data.urlFile);
                        webViewerLoad();
                        PDFViewerApplication.setScale(DEFAULT_SCALE, true);
                    }

                },
            });
            return false;
        });

        jQuery(document).on("click", ".onAutoOCR", function () {
            var self = $(this);
            var formTarget = jQuery(self.attr("data-form-meta"));
            var loading = Swal.fire({
                title: self.attr("data-title"),
                html: self.attr("data-mss-loading"),
                timerProgressBar: true,
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            $.ajax({
                url: self.attr("href"),
                type: 'POST',
                data: self.getDataUppername(),
                beforesend: function () {

                },
                error: function (jqXHR) {
                    loading.close();
                },
                complete: function (jqXHR, status) {
                    loading.close();
                },
                success: function (res) {
                    loading.close();
                    //if (res.type === "Success")
                    //    form.closest('.modal').modal('toggle');//Close model when success
                    CommonJs.SetMessage(res, true, true);
                    var isBsValidator = formTarget.hasClass("bootstrapValidator");
                    var data = res.data;
                    if (data) {
                        //Get số trnag vb từ pdf viewer//OCR chưa có
                        var el = formTarget.find("[data-code='PageAmount']");
                        el.val(jQuery(".pdfViewer").find(".page").length);

                        if (isBsValidator && el.attr("data-bv-notempty") == "true") { //revalidate
                            formTarget.bootstrapValidator('revalidateField', el.attr("name"));
                        }
                        function capitalizeFirstLetter(string) {
                            return string.charAt(0).toUpperCase() + string.slice(1);
                        };
                        for (var k in data) {
                            var el = formTarget.find("[data-code='" + capitalizeFirstLetter(k) + "']");
                            el.val(data[k]);
                            if (isBsValidator && el.attr("data-bv-notempty") == "true") { //revalidate
                                formTarget.bootstrapValidator('revalidateField', el.attr("name"));
                            }
                        }
                    }
                },
            });
            return false;
        });

        //Front-end function
        $(function () {
            /**
             * jQuery UI Resizeable created callback
             * @param {event} event
             * @param {object} ui
             */
            const createResizeEvent = (event, ui) => {
                $('.ui-resizable-handle').css({ height: event.target.firstElementChild.clientHeight });
            };
            /**
             * jQuery UI Resizeable resize callback
             * @param {event} event
             * @param {object} ui
             */
            const resizeEvent = (event, ui) => {
                $(event.target).css({ 'max-width': 'unset', flex: ui.size.width });
                $(event.target.previousElementSibling).css({ 'max-width': 'unset', flex: '1 1 auto' });
                $('.ui-resizable-handle').css({ height: event.target.firstElementChild.clientHeight });
                $('#sidebarTab').scrollingTabs('refresh');
            };

            const resizeOptions = {
                maxWidth: 960,
                minWidth: 360,
                handles: 'w',
                alsoResizeReverse: '.storage--documenting',
                create: createResizeEvent,
                resize: resizeEvent
            };

            const scrollTabsOptions = {
                cssClassLeftArrow: 'fas fa-angle-left',
                cssClassRightArrow: 'fas fa-angle-right',
                bootstrapVersion: 4,
                scrollToTabEdge: true,
                disableScrollArrowsOnFullyScrolled: true,
                ignoreTabPanes: true,
                enableSwiping: true
            };
            $('.storage--file').resizable(resizeOptions);
            $('#sidebarTab').scrollingTabs(scrollTabsOptions);
        });

        CollectionManagementConfig.Dropzoneinit();
    },
    ChangeDocType: function (idDoctype, idProfile, obj) {
        let method = 'POST';
        let url = "/CollectionManagement/ChangeDocType";
        let someData = { IDProfile: idProfile, IDDocType: idDoctype };
        let target = "#divDocTypes";
        let ssCallback = function (data) {
            //bind data
            CommonJs.DestroyValidator(obj.closest("form"));
            $(target).html(data);
            CommonJs.BootstrapValidator(obj.closest("form"));
            CommonJs.Select2Init();

        }
        CommonJs.CustAjaxCall(someData, method, url, "", ssCallback, "");
    },
    Dropzoneinit: function () {

        jQuery(".useDropzone").each(function () {
            var obj = jQuery(this);
            var currentFile = null;
            // DropzoneJS Demo Code Start
            Dropzone.autoDiscover = false;
            Dropzone.prototype.defaultOptions.dictDefaultMessage = "Thả file vào đây để tải lên";
            Dropzone.prototype.defaultOptions.dictFallbackMessage = "Trình duyệt không hỗ trợ kéo thả file để tải lên";
            Dropzone.prototype.defaultOptions.dictFallbackText = "Vui lòng sử dụng form dự phòng dưới đây để tải file lên bằng cách cũ";
            Dropzone.prototype.defaultOptions.dictFileTooBig = "File quá lớn ({{filesize}}MiB). Dung lượng tối đa: {{maxFilesize}}MiB.";
            Dropzone.prototype.defaultOptions.dictInvalidFileType = "Bạn không thể tải lên file loại này.";
            Dropzone.prototype.defaultOptions.dictResponseError = "Máy chủ phản hồi mã: {{statusCode}}.";
            Dropzone.prototype.defaultOptions.dictCancelUpload = "Không thể tải lên";
            Dropzone.prototype.defaultOptions.dictCancelUploadConfirmation = "Bạn có chắc hủy bỏ việc tải lên?";
            Dropzone.prototype.defaultOptions.dictRemoveFile = "Bỏ file";
            Dropzone.prototype.defaultOptions.dictMaxFilesExceeded = "Bạn không thể tải lên bất kì file nào nữa.";
            var formTarget = jQuery(obj.attr("data-form-meta"));
            // Get the template HTML and remove it from the doumenthe template HTML and remove it from the doument
            var previewNode = document.querySelector(obj.attr("data-template"));
            if (previewNode != undefined) {
                previewNode.id = '';
                var previewTemplate = previewNode.parentNode.innerHTML;
                previewNode.parentNode.removeChild(previewNode);

                var myDropzone = new Dropzone(document.body, {
                    // Make the whole body a dropzone
                    url: obj.attr("data-url"), // Set the url
                    thumbnailWidth: 35,
                    thumbnailHeight: 35,
                    parallelUploads: 20,
                    timeout: 180000,
                    previewTemplate: previewTemplate,
                    autoQueue: false, // Make sure the files aren't queued until manually added
                    previewsContainer: '#previews', // Define the container to display the previews
                    clickable: '.fileinput-button', // Define the element that should be used as click trigger to select files.
                    acceptedFiles: ".pdf"
                })

                myDropzone.on('addedfile', function (file) {
                    if (currentFile) {
                        this.removeFile(currentFile);
                    }
                    currentFile = file;
                    // Hookup the start button
                    // myDropzone.enqueueFile(file);
                    var modal = obj.closest('.modal');
                    //jQuery(".error.text-danger").addClass("hidden");
                    if (!modal.hasClass('in'))
                        obj.closest('.modal').modal({
                            backdrop: 'static', //Click outside
                            //keyboard: true, //Esc
                            show: true
                        });
                });
                // Setup the buttons for all transfers
                // The "add files" button doesn't need to be setup because the config
                // `clickable` has already been specified.
                document.querySelector("#dz-start").onclick = function () {
                    myDropzone.enqueueFiles(myDropzone.getFilesWithStatus(Dropzone.ADDED));
                };
                document.querySelector("#dz-cancel").onclick = function () {
                    myDropzone.removeAllFiles(true);
                };
                myDropzone.on('sending', function (file, xhr, formData) {
                    //Execute on case of timeout only
                    xhr.ontimeout = function (e) {
                        var errorDisplay = document.querySelectorAll('[data-dz-errormessage]');
                        errorDisplay[errorDisplay.length - 1].innerHTML = 'Quá thời gian phản hồi';
                    };
                });
                myDropzone.on('success', function (file, res) {
                    // CommonJs.SetMessage(response, false, true);

                    if (res.type === "Success") {
                        setTimeout(function () {
                            obj.closest('.modal').modal('toggle');//Close model when success
                        }, 1000);
                    }
                    CommonJs.SetMessage(res, true, true);

                    if (res.data) {
                        // form.reset();
                        jQuery(".onAutoOCR").attr("href", CommonJs.UpdateQueryStringParameter(jQuery(".onAutoOCR").attr("href"), "IDFile", res.data.idFile));
                        //Set url Ocr
                        formTarget.find("[name='IDFile']").val(res.data.idFile);

                        jQuery("#DEFAULT_URL").val(res.data.urlFile);
                        webViewerLoad();
                        PDFViewerApplication.setScale(DEFAULT_SCALE, true);
                    }

                });
            }
            // DropzoneJS Demo Code End
        });

    }
};