(function($) {
    $.fn.ajaxUploader = function(options) {
        var crlf = "\r\n";
        var boundary = "iloveigloo";
        var dashes = "--";
        var settings = {
            "name": "UploadFile",
            "postUrl": "",
            "elTarget": null,
            "onDrop": null,
            "onBegin": null,
            "onChange": null,
            "onClientAbort": null,
            "onClientError": null,
            "onClientLoad": null,
            "onClientLoadEnd": null,
            "onClientLoadStart": null,
            "onClientProgress": null,
            "onServerAbort": null,
            "onServerError": null,
            "onServerLoad": null,
            "onServerLoadStart": null,
            "onServerProgress": null,
            "onServerReadyStateChange": null,
            "onSuccess": null
        };
        if (options) {
            $.extend(settings, options);
        }
        return this.each(function(options) {
            var $this = $(this);
            if ($this.is("[type='file']")) {
                $this.unbind();
                $this.bind("change", function (event) {
                    event.stopImmediatePropagation();

                    var files = this.files;
                    var dragbox = jQuery(settings.elTarget);
                    if (settings.onBegin) {
                        if (settings.onBegin(this, dragbox)) {
                            for (var i = 0; i < files.length; i++) {
                                if (settings.onChange) {
                                    if (settings.onChange(this, files, dragbox)) {
                                        fileHandler(files[i], dragbox);
                                    } else {
                                        break;
                                    }
                                } else {
                                    fileHandler(files[i], dragbox);
                                }
                            }
                        }
                    } else {
                        for (var i = 0; i < files.length; i++) {
                            if (settings.onChange) {
                                if (settings.onChange(this, files, dragbox)) {
                                    fileHandler(files[i], dragbox);
                                } else {
                                    break;
                                }
                            } else {
                                fileHandler(files[i], dragbox);
                            }
                        }
                    }
                    return false;
                });
            } else {
                $this.unbind();
                $this.bind("dragenter dragover", function (e) {
                    e.stopImmediatePropagation();
                    $(this).addClass("hover");
                    return false;
                }).bind("dragleave", function (e) {
                    e.stopImmediatePropagation();
                    $(this).removeClass("hover");
                    return false;
                }).bind("drop", function (e) {
                    e.stopImmediatePropagation();
                    $(this).removeClass("hover");
                    var dragbox = jQuery(settings.elTarget);

                    try {
                        var files = e.originalEvent.dataTransfer.files;
                        if (settings.onBegin) {
                            if (settings.onBegin(this, dragbox)) {
                                for (var i = 0; i < files.length; i++) {
                                    if (settings.onDrop) {
                                        if (settings.onDrop(this, files, dragbox)) {
                                            fileHandler(files[i], dragbox);
                                        } else {
                                            break;
                                        }
                                    } else {
                                        fileHandler(files[i], dragbox);
                                    }
                                }
                            }
                        } else {
                            for (var i = 0; i < files.length; i++) {
                                if (settings.onDrop) {
                                    if (settings.onDrop(this, files, dragbox)) {
                                        fileHandler(files[i], dragbox);
                                    } else {
                                        break;
                                    }
                                } else {
                                    fileHandler(files[i], dragbox);
                                }
                            }
                        }
                    } catch (ex) {
                        console.log(ex);
                    }
                    return false;
                });
            }
        });

        function fileHandler(file, dragbox) {

            file.id = Math.floor((Math.random() * 100) + 1).toString() + (new Date().getTime()).toString();

            var fileReader = new FileReader();
            fileReader.onabort = function(e) {
                if (settings.onClientAbort) {
                    settings.onClientAbort(e, file, dragbox);
                }
            };
            fileReader.onerror = function(e) {
                if (settings.onClientError) {
                    settings.onClientError(e, file, dragbox);
                }
            };
            fileReader.onload = function(e) {
                if (settings.onClientLoad) {
                    settings.onClientLoad(e, file, dragbox);
                }
            };
            fileReader.onloadend = function(e) {
                if (settings.onClientLoadEnd) {
                    settings.onClientLoadEnd(e, file, dragbox);
                }
            };
            fileReader.onloadstart = function(e) {
                if (settings.onClientLoadStart) {
                    settings.onClientLoadStart(e, file, dragbox);
                }
            };
            fileReader.onprogress = function(e) {
                if (settings.onClientProgress) {
                    settings.onClientProgress(e, file, dragbox);
                }
            };
            fileReader.readAsDataURL(file);
            var xmlHttpRequest = new XMLHttpRequest();
            xmlHttpRequest.upload.onabort = function(e) {
                if (settings.onServerAbort) {
                    settings.onServerAbort(e, file, dragbox);
                }
            };
            xmlHttpRequest.upload.onerror = function(e) {
                if (settings.onServerError) {
                    settings.onServerError(e, file, dragbox);
                }
            };
            xmlHttpRequest.upload.onload = function(e) {
                if (settings.onServerLoad) {
                    settings.onServerLoad(e, file, dragbox);
                }
            };
            xmlHttpRequest.upload.onloadstart = function(e) {
                if (settings.onServerLoadStart) {
                    settings.onServerLoadStart(e, file, dragbox);
                }
            };
            xmlHttpRequest.upload.onprogress = function(e) {
                if (settings.onServerProgress) {
                    settings.onServerProgress(e, file, dragbox);
                }
            };
            xmlHttpRequest.onreadystatechange = function(e) {
                if (settings.onServerReadyStateChange) {
                    settings.onServerReadyStateChange(e, file, dragbox, xmlHttpRequest.readyState);
                }
                if (settings.onSuccess && xmlHttpRequest.readyState == 4 && xmlHttpRequest.status == 200) {
                    try {
                        settings.onSuccess(e, file, dragbox, JSON.parse(xmlHttpRequest.responseText));
                    } catch (e) {
                        settings.onClientError(e, file, dragbox);
                        var message = jQuery(xmlHttpRequest.responseText).find("#MessageError");
                        if (message.length > 0) {
                            alert(message.text());
                        }
                    }
                }
            };
            
            var postUrl = settings.postUrl;
            if (typeof settings.postUrl == "function") {
                postUrl = settings.postUrl();
            }

            xmlHttpRequest.open("POST", postUrl, true);
            if (file.getAsBinary) {
                var data = dashes + boundary + crlf
                    + "Content-Disposition: form-data; name=\""
                    + settings.name + "\";" + "filename=\""
                    + unescape(encodeURIComponent(file.name)) + "\"" + crlf
                    + "Content-Type: application/octet-stream" + crlf
                    + crlf + file.getAsBinary() + crlf + dashes + boundary
                    + dashes;
                xmlHttpRequest.setRequestHeader("Content-Type", "multipart/form-data;boundary=" + boundary);
                xmlHttpRequest.sendAsBinary(data);
            } else if (window.FormData) {
                var formData = new FormData();
                formData.append(settings.name, file);
                xmlHttpRequest.send(formData);
            }
        }
    };
})(jQuery);