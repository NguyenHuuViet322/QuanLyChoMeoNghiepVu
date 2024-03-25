var Destruction = {
    Init: function () {
        jQuery(document).on("click", ".removeticketitem", function () {
            let id = Number(jQuery(this).attr('data-id'));
            let url = '/Destruction/RemoveProfileToTicket';
            let tr = jQuery(this).closest('tr');
            let data = { id: id };
            function AutoNumber() {
                $('#tableDetail').find('tr.autonumber').each(function (i) {                    
                    $(this).find('td:first').text(i+1);
                });
            }
            $(document).find('input:checkbox.addticketitem').each(function () {
                if (Number($(this).attr('data-id')) == id) {
                    $(this).prop("checked", false);
                }                
            });
            jQuery.ajax({
                type: 'POST',
                async: false,
                url: url,
                data: data,
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.ShowNotifyMsg(rs.type, rs.message);        
                        tr.remove();
                        AutoNumber();
                        return false;
                    };
                }
            });
        });
        jQuery(document).on("change", ".addticketitem", function () {
            Destruction.ToggleMultiTicks(jQuery(this).closest('table'));
            $(this).trigger("updateTicket");
        });
        
        jQuery(document).on('change', '.group-checkableds', function () {

            var table = jQuery(this).closest("table");
            if (jQuery(this).attr('data-group') != undefined) {
                var group = jQuery(this).attr('data-group');
                var set = table.find('.checkboxes[data-group="' + group + '"]');
                //var set = table.find('.checkboxes');
            } else {
                var set = table.find(".checkboxes, .sumChecked");
            }
            var checked = jQuery(this).is(":checked");
            jQuery(set).each(function () {
                let trigger = false;
                if (checked) {                    
                    if (jQuery(this).prop("checked") != true) {
                        trigger = true;
                    }
                    if (!$(this).prop('disabled')) {
                        jQuery(this).prop("checked", true);
                        jQuery(this).closest('tr').addClass("active");
                    }
                    if (trigger) {
                        $(this).trigger("updateTicket");
                    }
                } else {
                    if (jQuery(this).prop("checked") == true) {
                        trigger = true;
                    }
                    jQuery(this).prop("checked", false);
                    jQuery(this).closest('tr').removeClass("active");
                    if (trigger) {
                        $(this).trigger("updateTicket");
                    }
                }
            });
            Destruction.ToggleMultiTicks(table);
        });
        Destruction.PreLoadCheckbox($('#data_table'));
    },
    ToggleMultiTicks: function (table) {
        var flag = false;
        var fullcheck = true;
        var wrapper = table.closest(".dataTables_wrapper");
        var grouper = wrapper.find(".group-checkableds");
        table.find(".checkboxes").each(function () {
            if (jQuery(this).prop("checked")) {               
                flag = true;               
            }
            if (!jQuery(this).prop("checked")) {
                fullcheck = false;
            }
        });
        if (!flag) {          
            if (grouper.prop("checked"))
                grouper.prop("checked", false);
        }
        if (fullcheck) {
            grouper.prop("checked", true);
        } else {
            grouper.prop("checked", false);
        }
    },
    PreLoadCheckbox: function (table) {
        var flag = false;
        var fullcheck = true;
        var wrapper = table.closest(".dataTables_wrapper");
        var actions = wrapper.find(".multiTicketItem");
        var buttons = actions.find(".btn");
        var grouper = wrapper.find(".group-checkableds");
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
            //buttons.addClass("disabled").prop("disabled", true);
            if (grouper.prop("checked"))
                grouper.prop("checked", false);
        }
        if (fullcheck) {
            grouper.prop("checked", true);
        } else {
            grouper.prop("checked", false);
        }
    },
    BindEvent: function () {
        $('.addticketitem').bind("updateTicket", function (e) {
            let id = Number(jQuery(this).attr('data-id'));
            let url = '';
            if (jQuery(this).prop("checked") == true) {
                url = '/Destruction/AddProfileToTicket';
            } else {
                url = '/Destruction/RemoveProfileToTicket';
            }
            let data = { id: id };
            jQuery.ajax({
                type: 'POST',
                async: false,
                url: url,
                data: data,
                success: function (rs) {
                    if (rs.type != undefined) {
                        CommonJs.ShowNotifyMsg(rs.type, rs.message);
                        var numberProfile = rs.data.tkvalue.length;
                        var action = $('#actApprove');
                        var btn = $('#btnApprove');
                        if (numberProfile > 0 ) {
                            action.removeClass("hidden");
                            btn.removeClass("disabled").prop("disabled", false);
                            btn.html(`<span><i class="fas fa-plus mr-2"></i>Thêm quyết định tiêu hủy (${numberProfile})</span>`);
                        } else {
                            action.addClass("hidden");
                            btn.addClass("disabled").prop("disabled", true);
                        }
                        return false;
                    };
                }
            });
        });
    },
};