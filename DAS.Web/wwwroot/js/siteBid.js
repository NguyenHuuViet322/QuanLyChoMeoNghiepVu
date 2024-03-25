var mainBid = {
    AdapterMobile: function (isMobile) {
        const url = "/Account/SetCookieMobile";
        jQuery.ajax({
            type: "POST",
            async: true,
            data: { "isMobile": isMobile },
            url: url
        });
    },
    SetStgMobile: function (checkMedia) {
        localStorage.setItem('isMobile', checkMedia);
        return localStorage.getItem('isMobile');
    },
    //Initialize TraCuuThongTinJs Elements
    TraCuuThongTinModalTree: function () {
        const modalDhdt = $('.modal-searchInfoDHDT');
        if (modalDhdt.length !== 0) {
            const selectSingle = modalDhdt.find('.modal-body__control-single');
            const selectDouble = modalDhdt.find('.modal-body__control-double');
            const checkList = modalDhdt.find('.modal-body__company-check-list');
            const btnSave = modalDhdt.find('.btn-company-save');
            const btnDelete = modalDhdt.find('.btn-company-delete');
            const companyListOutModal = $('.company-bid-list');
            let inputCheckBox = modalDhdt.find('.form-check-input[type="checkbox"]');

            //Khi xảy ra sự chuyển đổi trạng thái input checkbox
            inputCheckBox.on('change', function (event) {
                const current = event.currentTarget;

                //trạng thái checked
                if ($(current).is(':checked')) {
                    $(current).closest('.form-check-cs').addClass('form-check--bg');
                }
                //trạng thái uncheck
                else {
                    $(current).closest('.form-check-cs').removeClass('form-check--bg');
                    $(current).removeClass('is-active');
                }
            });

            //Xử lí click vào nút chọn đơn, chọn kép
            function handleClick(event) {
                const $current = $(event.currentTarget);
                const dataStatusClick = $current.data('click-status');
                const inputCheck = modalDhdt.find('.form-check-input:checked');
                const input = modalDhdt.find('.form-check-input');
                const labelCheck = inputCheck.next();

                let htmlAppend = '';
                checkList.html('');
                inputCheck.addClass('is-active');

                //kiểm tra nếu là nút chọn kép, addClass 'is-active' cho các nút con bên trong
                if (dataStatusClick === 'double') {
                    labelCheck.each(function (index, item) {
                        //addClass is-active cho các input:not(:checked) là con chưa được active
                        if ($(item).closest('ul').closest('li').length !== 0 && $(item).closest('.form-check-cs').next().find('ul').length !== 0) {
                            $(item).closest('ul').closest('li').find('.form-check-input:not(:checked)').addClass('is-active');
                        };
                    });
                };

                //kiểm tra nút chọn đơn, add lại class 'is-active'
                if (dataStatusClick === 'single') {
                    input.removeClass('is-active');
                    inputCheck.addClass('is-active');
                }

                //render html khi nhấn click nút chọn đơn, chọn kép
                $('.is-active').next().each(function (index, item) {
                    htmlAppend += `<div class="modal-body__company-check-item d-flex mb-5-px" data-company = ${$(item).data('check')}>
                                                <p class="modal-body__company-check-name mb-0">
                                                    ${item.innerHTML}
                                                </p>
                                                <div class="modal-body__company-icon-exit ml-5-px d-flex align-items-center justify-content-center">
                                                    <span class="icon-exit mr-0">
                                                    </span>
                                                </div>
                                            </div>`;
                });

                checkList.append(htmlAppend);

                //Click X, trong modal tra cứu thông tin
                const companyExit = modalDhdt.find('.modal-body__company-icon-exit');

                if (companyExit.length !== 0) {
                    companyExit.on('click', function () {
                        const self = $(this);
                        const eleParent = self.closest('.modal-body__company-check-item');
                        const dataComapny = eleParent.data('company');

                        //đặt lại trạng thái checked của input
                        const formCheckCs = modalDhdt.find(`.form-check-label[data-check = '${dataComapny}']`).prev().prop('checked', false);
                        formCheckCs.change();
                        eleParent.remove();
                    });
                };
            };

            //add sự kiện click cho 2 nút chọn đơn, kép
            selectSingle.on('click', handleClick);
            selectDouble.on('click', handleClick);

            //click Hủy trong modal tra cứu thông tin
            btnDelete.on('click', function () {
                const formCheckInput = modalDhdt.find('.form-check-input');
                formCheckInput.prop('checked', false);
                formCheckInput.change();

                //remove dom các công ty được lựa chọn trong Modal
                if (checkList.children().length !== 0) {
                    checkList.children().remove();
                }

                //remove dom các công ty được lựa chọn ngoài Modal
                if (companyListOutModal.children().length !== 0) {
                    companyListOutModal.children().remove();
                    companyListOutModal.css({ 'padding': 0 });
                }

            });

            //click Lưu trong modal tra cứu thông tin
            btnSave.on('click', function () {
                const modalCompanyCheckItem = $('.modal-body__company-check-item');
                let htmlAppend = '';

                if (modalCompanyCheckItem.length === 0) {
                    return;
                };

                companyListOutModal.html('');
                companyListOutModal.css({ 'padding': 0 });

                //reder html ra màn hình ngoài modal
                modalCompanyCheckItem.each(function (index, item) {

                    htmlAppend += `<li class="col-auto m-w-259 company-bid-item" data-company-out=${$(item).data('company')}>
                            <div class=" d-flex align-items-center">
                                <span class="icon-building"></span>
                                <p class="company-bid-name mb-0 text-primary font-s-14 pl-10 pr-5">${item.innerText}</p>
                                <span class="company-icon-exit w-h-18 d-flex justify-content-center align-items-center">
                                    <span class="icon-exit mr-0"></span>
                                </span>
                            </div>
                        </li>`;
                });

                companyListOutModal.append(htmlAppend);
                companyListOutModal.css({ 'padding': '10px 0 10px 30px' });

                //click icon X ngoài modal tra cứu thông tin
                const iconExitOutModal = $('.company-icon-exit');
                iconExitOutModal.on('click', function () {
                    const self = $(this);
                    const companyOut = self.closest('.company-bid-item');
                    const dataCompanyOut = companyOut.data('company-out');
                    const companyCheckItem = $(`.modal-body__company-check-item[data-company=${dataCompanyOut}]`);
                    const formCheckCs = $(`.form-check-label[data-check = '${dataCompanyOut}']`).prev().prop('checked', false);

                    //check lại sự thay đổi input, remove dom;
                    formCheckCs.change();
                    companyCheckItem.remove();
                    companyOut.remove();

                    //set padding khi không có công ty nào được hiển thị bên ngoài modal tra cứu thông tin
                    if ($('.modal-body__company-check-item').length === 0) {
                        companyListOutModal.css({ 'padding': 0 });
                    };
                });
            });

        };
    }
}


jQuery(function () {
    $('.nav-link').on('click', function () {
        $(this).find('i.bid-menu-plus.jsIconDown').toggleClass('active-menu');
    });

    const $navItem = $('.nav-pills > .nav-item');
    if ($navItem.length !== 0) {
        $navItem.each(function (i, item) {
            let clLv = `nav-lv-${i}`;

            const groups = $(item).find(".nav-item-group");
            if (groups.length > 0) {
                navTreeView.each(function (j, item) {
                    const navTreeView = $(item).find(".nav-treeview");
                    navTreeView.each(function (index, item) {
                        let mrleft = 10 * index + 'px';
                        $(item).css("padding-left", mrleft);
                        $(item).addClass(clLv);
                    });
                });
            } else {
                const navTreeView = $(item).find(".nav-treeview");
                navTreeView.each(function (index, item) {
                    let mrleft = 10 * index + 'px';
                    $(item).css("padding-left", mrleft);
                    $(item).addClass(clLv);
                });
            }

        });
    };
    //Initialize Select2 Elements
    //$('.select2').each(function () {
    //    var dropdownParent = $(document.body);
    //    if ($(this).parents('.select2-container').length !== 0) dropdownParent = $(this).parents('.select2-container');
    //    $(this).select2({
    //        dropdownParent: dropdownParent,
    //        minimumResultsForSearch: 10,
    //    });
    //});
    CommonJs.Select2Init();
    CommonJs.Select2InitAjax();
    //CommonJs.Select2InitAjaxModal();


    //Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    });

    //Date range picker
    $('#reservation').daterangepicker({
        locale: {
            format: 'DD/MM/yyyy',
            applyLabel: 'Chọn',
            cancelLabel: 'Hủy'
        }
    });

    //Initialize select2-cs Elements
    const $select2Cs = $('.select2-cs');
    if ($select2Cs.length > 0) {
        $select2Cs.select2({
            closeOnSelect: false
        });

        $select2Cs.on('select2:open', function (e) {
            e.preventDefault();
            e.stopPropagation();
            $('.btn.btn-sm.dropdown-toggle').dropdown('dispose');
        });

        $select2Cs.on('select2:close', function (e) {
            e.preventDefault();
            e.stopPropagation();
            $('.btn.btn-sm.dropdown-toggle').dropdown('update');
        });

    };

    //Initialize select2-cs-v2 Elements
    const $select2CsV2 = $('.select2-cs-v2');
    if ($select2CsV2.length !== 0) {

        //create template result, selection
        function formatResult(item) {
            if (!item.id) {
                return item.text;
            }
            let level = 0;
            if (item.element !== undefined) {
                level = (item.element.className);
                if (level.trim() !== '') {
                    level = (parseInt(level.match(/\d+/)[0])) - 1;
                }
            }
            let $result = $(`<span class = "select2Nested" data-level = ${level}>${item.text}</span>`);
            return $result;
        };
        $select2CsV2.select2({
            closeOnSelect: false,
            templateResult: formatResult,
            templateSelection: formatResult
        });
        //style dropdown select2
        function styleDropDownSelect2V2($select2Nested) {
            const widthContainerSelect2 = $select2CsV2.next().width();
            const translateXValue = (320 - parseInt(widthContainerSelect2)) / 2;
            const $dropdown = $select2Nested.closest('.select2-dropdown');
            const $select2Results = $select2Nested.closest('.select2-results');

            if ($dropdown.find('.div-pseudo').length == 0) {
                $select2Results.before('<div class="div-pseudo"></div>');
            };

            $dropdown.css({
                'width': '320px',
                'transform': `translateX(${-translateXValue}px)`,
                'border': 'none',
                'border-radius': '4px',
                'margin-top': '10px',
                'box-shadow': '0 1px 2px 1px var(--color-gray-ccc)'
            });
        };
        //nested group multiple follow level
        $select2CsV2.on("select2:open", function (e) {
            setTimeout(function () {
                const $select2Nested = $('.select2Nested');

                $select2Nested.each(function (index, item) {
                    const $item = $(item);
                    let level = $item.data('level');
                    const select2ResultOption = $item.closest('.select2-results__option');
                    let paddingLeft = 16 * parseInt(level) + 12;
                    select2ResultOption.css({ 'padding-left': `${paddingLeft}px` })
                });
                styleDropDownSelect2V2($select2Nested);
            }, 0);
        });
        $select2CsV2.on("select2:select", function () {
            let $select2Nested = $('.select2Nested');
            styleDropDownSelect2V2($select2Nested);
        });
        $select2CsV2.on("select2:unselect", function () {
            let $select2Nested = $('.select2Nested');
            styleDropDownSelect2V2($select2Nested);
        });
    };

    //Set line-dot cho form chi tiết
    let $containerFluid = $('.container-fluid div.d-flex.align-items-center.justify-content-between');
    $containerFluid.each(function () {
        let isTitle = $(this).find(".is-title-moi-thau").width();
        let isSearch = $(this).find('form, .form-inline').width();
        $(this).find(".line-dot").css({ 'width': $(this).width() - (isTitle ? isTitle : 0) - (isSearch ? isSearch + 45 : 0) + 'px' });
    });

    //Set event reset input in form
    $('button[type="reset"]').on('click', function () {
        let form = $(this).parents('form:eq(0)');
        form.find('select.select2').val(null).trigger("change");
        form.find('.date_input').datetimepicker('clear');
        setTimeout(function () {
            // Chờ reset form, thực hiện action tiếp theo
            form.find('button[type="submit"]:eq(0)').trigger("click");
        }, 1);
    });

    //Set active menu
    $('#nav-left-menu-bid').find('a[data-menu-active="active"]').addClass('active').parents('li').addClass('menu-open');

    //Thông tin tra cứu khai thác thông tin định hướng
    mainBid.TraCuuThongTinModalTree();

    //Check box ở các trang chi tiết
    const divTableCusDa = $('.div-table-cus-da');
    if (divTableCusDa.length !== 0) {
        let inputCheckBox = $('.form-check-input[type="checkbox"]');
        if (inputCheckBox.length > 0) {
            inputCheckBox.on('change', function (event) {
                const current = event.currentTarget;
                $(current).closest('.form-check-cs').toggleClass('form-check--bg');
            });
        }
    };

    //Initialize js-format-money Elements
    const jsFormatNumber = $(".js-format-money");
    if (jsFormatNumber.length !== 0) {
        //format number input
        function formatNumber(num) {
            return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
        }

        //handle Input Number khi người dùng nhập
        function handleInputNumber(event) {
            const currentEle = event.currentTarget;
            let valueInput = $(currentEle).val();
            valueInput = valueInput.replaceAll(',', '');
            console.log(valueInput);
            let formatMoney = formatNumber(valueInput);
            $(currentEle).val(formatMoney);
        };
        jsFormatNumber.on('keyup change', handleInputNumber);
    }

    //click active item dropdown
    const jsDropdownCh = $('.dropdown-chuanhoa');

    if (jsDropdownCh.length !== 0) {
        const dropdownItem = jsDropdownCh.find('.dropdown-item');
        const dropdownTextShow = jsDropdownCh.find('.dropdown-chuanhoa__text');
        dropdownItem.on('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            const current = $(e.currentTarget);
            const dataIndex = current.data('index');
            console.log(dataIndex);
            current.toggleClass('active');
            if (current.hasClass('active')) {
                let htmlCurrent = `<span class="item-choosen" data-index-sp = "${dataIndex}">${current.text()}</span>`;
                dropdownTextShow.append(htmlCurrent);
            } else {
                $(`span[data-index-sp = "${dataIndex}"]`).remove();
            }

            let isFlag = true;
            const itemActive = jsDropdownCh.find('.dropdown-item.active');
            isFlag = itemActive.length !== 0 ? true : false;
            let spanTextInit = jsDropdownCh.find('.choosen-init');
            if (isFlag) {
                spanTextInit.addClass('hide-text');
            } else {
                spanTextInit.removeClass('hide-text');
            }
        });

    };

    if ($('.is-info-home').length > 0) {
        $('.chart-block.chart-bar').hide();
    }

    //menu 
    const $mainSideBar = $('.main-sidebar');
    if ($mainSideBar.length !== 0) {

        const menuLeftBid = $mainSideBar.find('#nav-left-menu-bid');
        const $jsPushMenu = $('.jsPushMenu');
        const body = $('body');
        //update width main-sidebar
        //mouse out element main-sidebar
        $mainSideBar.on('mouseout', function (e) {
            $mainSideBar.removeClass('sidebar-focused');
        });

        //handlePushMenu
        function handleClickPushMenu() {
            if ($mainSideBar.width() > 200) {
                const navItemOpen = menuLeftBid.find('.nav-item.menu-open');
                if (navItemOpen.length !== 0) {
                    navItemOpen.each(function (index, item) {
                        $(item).find('ul.nav.nav-treeview').addClass('is-height-0');
                    });
                }

            } else {
                const navItemOpen = menuLeftBid.find('.nav-item.menu-open');
                if (navItemOpen.length !== 0) {
                    navItemOpen.each(function (index, item) {
                        $(item).find('ul.nav.nav-treeview').removeClass('is-height-0');
                    });
                }
            }
        };
        $mainSideBar.on('mouseleave', function (e) {
            if (body.hasClass('sidebar-collapse')) {
                const navItemOpen = menuLeftBid.find('.nav-item.menu-open');
                if (navItemOpen.length !== 0) {
                    navItemOpen.each(function (index, item) {
                        $(item).find('ul.nav.nav-treeview').addClass('is-height-0');
                    });
                }
            }
        });

        $mainSideBar.on('mouseenter', function () {
            if (body.hasClass('sidebar-collapse')) {
                const navItemOpen = menuLeftBid.find('.nav-item.menu-open');
                if (navItemOpen.length !== 0) {
                    navItemOpen.each(function (index, item) {
                        $(item).find('ul.nav.nav-treeview').removeClass('is-height-0');
                    });
                }
            }

        });

        $jsPushMenu.on('click', function (e) {
            handleClickPushMenu();
            const outerWidthWindow = $(window).outerWidth();
            if (outerWidthWindow < 1366) {
                body.find('ul.nav.nav-treeview').removeClass('is-height-0');
            }
        });
    }
    const body = $('body');
    if (body.length !== 0) {
        function addClassSidebarCollapse() {
            const outerWidthWindow = $(window).outerWidth();
            if (outerWidthWindow < 1366) {
                if (!body.hasClass('sidebar-collapse')) {
                    body.addClass('sidebar-collapse');
                }
            } else {
                body.removeClass('sidebar-collapse');
            }
        }
        addClassSidebarCollapse();
        $(window).on('resize', function () {
            addClassSidebarCollapse();
        })
    }

    const $jsDateAdv = $('.jsDateAdv');
    if ($jsDateAdv.length !== 0) {
        $jsDateAdv.on('change change.datetimepicker',
            function () {
                const dataTarget = $(this).find('.datetimepicker-input').data('target');
                const valueDate = $(this).find('.datetimepicker-input').val();
                let dataTargetDateNotIp = dataTarget.slice(0, dataTarget.length - 4);
                const inputDateSearch = $(`input[data-target = "${dataTargetDateNotIp}"]`);
                inputDateSearch.val(valueDate);
            });
    }
    //call Ajax search select2 multiple
    window.CommonJs.InitSelect2SearchCs();
    //set margin colfix
    function handleMarginTop() {
        const boxtopCsFixRow = $('.boxtop-cs-fix-row');

        if (boxtopCsFixRow.length !== 0) {
            const boxTopCsFix = $('.boxtop-cs-fix');
            let outerHeightBoxTop = boxTopCsFix.outerHeight();
            boxtopCsFixRow.css("margin-top", `${outerHeightBoxTop}px`);
        };
    }
    handleMarginTop();
    $(window).on('resize', function () {
        handleMarginTop();
    });
    const searchBoxExpand = $('.search-box__expand');


    if (searchBoxExpand.length !== 0) {
        const $jsResetValue = searchBoxExpand.find('.btn-reset-search-new');
        $jsResetValue.on('click', function (e) {
            e.preventDefault();
            const input = searchBoxExpand.find('input[type="text"]');
            const inputMoney = searchBoxExpand.find('.js-format-money');
            const $jsInputDate = $('input.datetimepicker-input');
            const select2 = searchBoxExpand.find('.select2');
            select2.each(function (index, item) {
                $(item).val(null).trigger('change');
            });

            input.val('');
            inputMoney.val('');
            if ($(window).width() <= 1366) {
                var now = moment();
                var year = moment(now).subtract(1, 'years').format('DD/MM/YYYY');
                $jsInputDate.each(function (i, el) {
                    if (i % 2 === 0) {
                        $(el).val(year);
                    } else {
                        $(el).val(now.format('DD/MM/YYYY'));
                    }

                });
            }
        })
    }
    const btnCollapseChart = $('.btn-chart__box');
    if (btnCollapseChart.length !== 0) {
        //hide show chart 
        $('.jsHideChart').on('click', function (e) {
            const dataTarget = $(this).data('target');
            $(`${dataTarget}`).toggleClass('hideChart');
            $(`${dataTarget}`).slideToggle('slow');
        });
    };

    // Set localStorage màn hình mobile
    const wMedia = $(window).width();
    let checkMedia = wMedia < 768 || wMedia === 812;
    let isMobile = localStorage.getItem('isMobile');
    if (isMobile === null) {
        localStorage.setItem('isMobile', checkMedia);
        isMobile = mainBid.SetStgMobile(checkMedia);
    }

    mainBid.AdapterMobile(isMobile);
    $(window).resize(function () {
        checkMedia = $(window).width() < 768 || $(window).width() === 812;
        localStorage.setItem('isMobile', checkMedia);
        isMobile = mainBid.SetStgMobile(checkMedia);
        mainBid.AdapterMobile(isMobile);
        //setTimeout(function () { window.location.reload(); }, 1000);
    });

    // Js for mobile
    if (checkMedia) {
        $('#is-search').on('click', function () {
            $('.navbar-search').toggleClass('active');
        });

        if ($('.container').hasClass('has-menu-left')) {
            let html = '<div class="btn-menu-left"><i class="fas fa-chevron-left"></i></div>';
            $('.menu-right').append(html);

            $('.btn-menu-left').on('click', function () {
                $(this).toggleClass('active');
                $('.menu-right').toggleClass('active');
                $('html').toggleClass('overflow-hidden');
            });
        }

    }

    const $jsDropdownMenuFix = $('.jsDropdownMenuFix');
    if ($jsDropdownMenuFix.length !== 0) {
        $jsDropdownMenuFix.on('click', function (e) {
            e.stopPropagation();
        });
    }
});